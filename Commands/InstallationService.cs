using LoggingLibrary;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Build_Installer.Models;
using SharpAdbClient;
using System.Linq;
using SharpAdbClient.DeviceCommands;
using System.IO;
using System.Net;

namespace Build_Installer.Commands
{
    class ProgressChangedEventArgs : EventArgs
    {
        public int Progress;
        public string Description;

        public ProgressChangedEventArgs(int progress, string description)
        {
            Progress = progress;
            Description = description;
        }
    }
    class InstallationService : IProgress<int>
    {
        public event EventHandler<ProgressChangedEventArgs> ProgressChanged;
        private Func<string, Task<bool>> _userPromptHandler;
        

        public InstallationService(Func<string, Task<bool>> userPromptHandler)
        {
            _userPromptHandler = userPromptHandler;
        }

        public async Task InstallBuild(string buildPath)
        {
            if (buildPath == null || !buildPath.EndsWith(".apk"))
                throw new Exception("Only Installing APK builds are supported");

            // Get the package name 
            CMDCommand command = new CMDCommand($"aapt dump badging \"{buildPath}\" | findstr -i \"package: name\"");
            command.Execute(null);
            InstallationPackageInfo installationPackage = InstallationPackageInfo.ParseAndroid(command.Output);

            // check if this package is installed and is the same version or lower

            //var installApkCommand = new InstallAPK(buildPath);
            //installApkCommand.Execute(buildPath);
            var adbClient = new AdbClient();
            List<DeviceData> connectedDevices = adbClient.GetDevices();
            
            if (connectedDevices.Count < 1)
                throw new Exception("No connected devices found");

            ProgressChanged.Invoke(this, new ProgressChangedEventArgs(20, "Checking If build is already installed"));
            DeviceData device = connectedDevices.First();
            var packageManager = new PackageManager(adbClient, device);
            bool reinstall = false;
            if(packageManager.Packages.ContainsKey(installationPackage.PackageName))
            {
                if (_userPromptHandler != null)
                {
                    reinstall = await _userPromptHandler.Invoke("Build Already Present on Android Device, Reinstall?");
                    if (!reinstall)
                        return;
                }
            }

            ProgressChanged.Invoke(this, new ProgressChangedEventArgs(40, "Installing Apk"));
            packageManager.InstallPackage(buildPath, reinstall);


            ProgressChanged.Invoke(this, new ProgressChangedEventArgs(60, "Looking for expansin files"));
            // Look for obb files
            DirectoryInfo buildDirectory = new DirectoryInfo(Path.GetDirectoryName(buildPath));
            FileInfo[] buildDirectoryFiles = buildDirectory.GetFiles();
            IEnumerable<FileInfo> obbFiles = buildDirectoryFiles.Where(fileinfo => fileinfo.Extension == ".obb");
            bool copyObbFiles = false;
            if(obbFiles.Count() > 0)
            {
                var expansionFilesFoundMessage = new StringBuilder();
                expansionFilesFoundMessage.AppendLine("Apk expansion files found: ");
                foreach (FileInfo obbFile in obbFiles)
                {
                    expansionFilesFoundMessage.AppendLine($"{obbFile}");
                }
                expansionFilesFoundMessage.Append("Attempt to copy files to the device? Build might not work without expansion files.");
                copyObbFiles = await _userPromptHandler?.Invoke($"Apk Expansion Files Found: {expansionFilesFoundMessage}");
            }

            if(copyObbFiles)
            {
                foreach (var obbFile in obbFiles)
                {
                    using (var syncService = new SyncService(new AdbSocket(new IPEndPoint(IPAddress.Loopback, AdbClient.AdbServerPort)), device))
                    {
                        using(var obbStream = File.OpenRead(obbFile.FullName))
                        {
                            // OBB file name is build by {main/patch}.{versioncode}.{packagename}.obb
                            string fileNamePrefix = obbFile.Name.Split('.').First();
                            string packageName = installationPackage.PackageName;
                            string versionCode = installationPackage.VersionCode;
                            string targetDirectory = $"sdcard/Android/obb/{packageName}";
                            string targetFileName = $"{fileNamePrefix}.{versionCode}.{packageName}.obb";
                            // Create direcotry on android device if it doesn't exist
                            var adbShellOutputReciever = new ConsoleOutputReceiver();
                            adbClient.ExecuteShellCommand(device, $"mkdir -p {targetDirectory}", adbShellOutputReciever);
                            LoggingService.Logger.Info($"Device mkdir output: {adbShellOutputReciever}");
                            syncService.Push(obbStream, $"{targetDirectory}/{targetFileName}", 666, DateTime.Now, this, CancellationToken.None);
                        }
                    }
                }
            }
            ProgressChanged.Invoke(this, new ProgressChangedEventArgs(100, "Installing Apk"));
        }

        public void Report(int value)
        {
            ProgressChanged.Invoke(this, new ProgressChangedEventArgs(value, "Copying expansion files"));
        }
    }
}

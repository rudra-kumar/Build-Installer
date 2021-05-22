using Build_Installer.Commands;
using Build_Installer.Models;
using LoggingLibrary;
using SharpAdbClient;
using SharpAdbClient.DeviceCommands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Build_Installer
{
    class AndroidBuildInstaller : IProgress<int>
    {
        public event EventHandler<ProgressChangedEventArgs> ProgressChanged;
        private Func<string, Task<bool>> _userPromptHandler;
        private IAdbClient _adbClient;

        public AndroidBuildInstaller(Func<string, Task<bool>> userPromptHandler)
        {
            _adbClient = InstallationService.AdbClient;
            _userPromptHandler = userPromptHandler;
        }

        public async Task InstallBuild(string buildPath, DeviceData device)
        {
            if (buildPath == null || !buildPath.EndsWith(".apk"))
                throw new Exception("Only Installing APK builds are supported");

            // Get the package name 
            CMDCommand command = new CMDCommand($"aapt dump badging \"{buildPath}\" | findstr -i \"package: name\"");
            command.Execute(null);
            InstallationPackageInfo installationPackage = InstallationPackageInfo.ParseAndroid(command.Output);

            ProgressChanged.Invoke(this, new ProgressChangedEventArgs(20, "Checking If build is already installed"));
            var packageManager = new PackageManager(_adbClient, device);
            bool reinstall = false;
            if (packageManager.Packages.ContainsKey(installationPackage.PackageName))
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
            if (obbFiles.Count() > 0)
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

            if (copyObbFiles)
            {
                foreach (var obbFile in obbFiles)
                {
                    using (var syncService = new SyncService(new AdbSocket(new IPEndPoint(IPAddress.Loopback, AdbClient.AdbServerPort)), device))
                    {
                        using (var obbStream = File.OpenRead(obbFile.FullName))
                        {
                            // OBB file name is build by {main/patch}.{versioncode}.{packagename}.obb
                            string fileNamePrefix = obbFile.Name.Split('.').First();
                            string packageName = installationPackage.PackageName;
                            string versionCode = installationPackage.VersionCode;
                            string targetDirectory = $"sdcard/Android/obb/{packageName}";
                            string targetFileName = $"{fileNamePrefix}.{versionCode}.{packageName}.obb";
                            // Create direcotry on android device if it doesn't exist
                            var adbShellOutputReciever = new ConsoleOutputReceiver();
                            _adbClient.ExecuteShellCommand(device, $"mkdir -p {targetDirectory}", adbShellOutputReciever);
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

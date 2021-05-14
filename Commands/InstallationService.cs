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
    class InstallationService
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

            ProgressChanged.Invoke(this, new ProgressChangedEventArgs(20, "Checking If app exists"));
            Thread.Sleep(1000);
            ProgressChanged.Invoke(this, new ProgressChangedEventArgs(40, "Installing Apk"));
            //var installApkCommand = new InstallAPK(buildPath);
            //installApkCommand.Execute(buildPath);
            var adbClient = new AdbClient();
            DeviceData device = adbClient.GetDevices().First();
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

            packageManager.InstallPackage(buildPath, reinstall);
            ProgressChanged.Invoke(this, new ProgressChangedEventArgs(100, "Installing Apk"));
        }
    }
}

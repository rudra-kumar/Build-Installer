using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

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
    class InstallBuild : StatefullCommand
    {
        public event EventHandler<ProgressChangedEventArgs> ProgressChanged;
        private string _buildPath;
        

        public InstallBuild(string path)
        {
            _buildPath = path;
        }

        protected override void OnExecute(object parameter)
        {
           
            if (parameter != null)
                _buildPath = parameter as string;

            if (_buildPath == null || !_buildPath.EndsWith(".apk"))
                throw new Exception("Only Installing APK builds are supported");
            ProgressChanged.Invoke(this, new ProgressChangedEventArgs(20, "Checking If app exists"));
            Thread.Sleep(1000);
            ProgressChanged.Invoke(this, new ProgressChangedEventArgs(40, "Installing Apk"));
            
            // Get the name of the android package to be installed from the build path 
            // var cmdCommand = new CommandLine("Get the package name from build path");
            // cmdCommand.Execute();
            // if(string.IsNullOrEmpty(cmdCommand.Output))
            // {
            //     throw new Exception($"Unable to get the package from android package {_buildPath}");
            // }
            // 
            // // Add commands to Uninstall this package 
            // ChildCommands.Add(new UninstallAndroidBuild(cmdCommand.Output));

            // Add commands to install this new build from the path
            var installApkCommand = new InstallAPK(_buildPath);
            installApkCommand.Execute(parameter);
            ProgressChanged.Invoke(this, new ProgressChangedEventArgs(100, "Installing Apk"));
            
            
        }

    }
}

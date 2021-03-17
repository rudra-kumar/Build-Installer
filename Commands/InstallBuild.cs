using System;
using System.Collections.Generic;
using System.Text;

namespace Build_Installer.Commands
{
    class InstallBuild : Command
    {
        private string buildPath;
        public InstallBuild(string path)
        {
            if (!path.EndsWith(".apk"))
                throw new Exception("Only Installing APK builds are supported");
            buildPath = path;
        }

        protected override void OnExecute()
        {
            throw new NotImplementedException();
            // Get the name of the android package to be installed from the build path 
            var cmdCommand = new CommandLine("Get the package name from build path");
            cmdCommand.Execute();
            if(string.IsNullOrEmpty(cmdCommand.Output))
            {
                throw new Exception($"Unable to get the package from android package {buildPath}");
            }

            // Add commands to Uninstall this package 
            ChildCommands.Add(new UninstallAndroidBuild(cmdCommand.Output));

            // Add commands to install this new build from the path
            ChildCommands.Add(new InstallAPK(buildPath));
        }
    }
}

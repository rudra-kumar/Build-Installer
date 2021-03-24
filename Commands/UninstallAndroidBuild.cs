using System;
using System.Collections.Generic;
using System.Text;

namespace Build_Installer.Commands
{
    class UninstallAndroidBuild : Command
    {
        private string _appID;

        public UninstallAndroidBuild(string applicationID)
        {
            _appID = applicationID;
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        protected override void OnExecute(object parameter)
        {
            var commandLine = new CommandLine($"adb uninstall {_appID}");
            commandLine.Execute();
        }
    }
}

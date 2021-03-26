using System;
using System.Collections.Generic;
using System.Text;

namespace Build_Installer.Commands
{
    class UninstallAndroidBuild : StatelessCommand
    {
        private string _appID;

        public UninstallAndroidBuild(string applicationID)
        {
            _appID = applicationID;
        }

        protected override void OnExecute(object parameter)
        {
            var commandLine = new CMDCommand($"adb uninstall {_appID}");
            commandLine.Execute(parameter);
        }
    }
}

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

        protected override void OnExecute()
        {
            throw new NotImplementedException();
        }
    }
}

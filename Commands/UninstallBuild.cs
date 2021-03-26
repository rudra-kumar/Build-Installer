using System;
using System.Collections.Generic;
using System.Text;

namespace Build_Installer.Commands
{
    class UninstallBuild : StatelessCommand // This can be later on used to determine if it's an iOS / Android build that we're trying to isntall 
    {
        protected override void OnExecute(object parameter)
        {
            throw new NotImplementedException();
        }
    }
}

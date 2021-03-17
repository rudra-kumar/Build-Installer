using System;
using System.Collections.Generic;
using System.Text;

namespace Build_Installer.Commands
{
    class InstallAPK : Command
    {
        private string path;

        public InstallAPK(string path)
        {
            if (path.EndsWith(".apk"))
                throw new Exception("Path should end with .apk extension");
            ChildCommands.Add(new CommandLine($"adb install \"{path}\""));
        }

        protected override void OnExecute()
        {
        }
    }
}

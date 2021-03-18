using System;
using System.Reflection;
using System.Collections.Specialized;
using System.IO;

namespace Build_Installer.Commands
{
    class InstallAPK : Command
    {
        private string _path;

        public InstallAPK(string path)
        {
            if (!path.EndsWith(".apk"))
                throw new Exception("Path should end with .apk extension");
            _path = path;
            if (!File.Exists(path))
                throw new FileNotFoundException($"File at path {path} not found");
        }

        protected override void OnExecute()
        {
            string platformToolsPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "platform-tools");
            // #TODO - Perhaps this should be done when the application starts
            var adbEnvironmentVariables = new StringDictionary();
            adbEnvironmentVariables.Add("Path", platformToolsPath);

            ChildCommands.Add(new CommandLine($"adb install \"{_path}\"", adbEnvironmentVariables));
        }
    }
}

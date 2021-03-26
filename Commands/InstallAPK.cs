﻿using System;
using System.Reflection;
using System.Collections.Specialized;
using System.IO;

namespace Build_Installer.Commands
{
    class InstallAPK : StatelessCommand
    {
        private string _path;

        public InstallAPK(string path)
        {
            _path = path;
        }

        protected override void OnExecute(object parameter)
        {
            if (_path == null || !_path.EndsWith(".apk"))
                throw new Exception("Path should end with .apk extension");
            if (!File.Exists(_path))
                throw new FileNotFoundException($"File at path {_path} not found");
            string platformToolsPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "platform-tools");
            // #TODO - Perhaps this should be done when the application starts
            var adbEnvironmentVariables = new StringDictionary();
            adbEnvironmentVariables.Add("Path", platformToolsPath);

            // #TODO - Remove the -r, that should happen outside this function 
            var installCommand = new CMDCommand($"adb install -r \"{_path}\"", adbEnvironmentVariables);
            installCommand.Execute(parameter);
        }
    }
}

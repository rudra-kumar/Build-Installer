using System.Diagnostics;
using LoggingLibrary;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Build_Installer.Commands
{
    class CMDCommand : StatelessCommand
    {
        private string _cmdCommand;
        public string Output { get; private set; }
        private const string ERROR_LEVEL = "& if ERRORLEVEL 1 echo error";

        public CMDCommand(string cmdCommand)
        {
            _cmdCommand = cmdCommand;
        }

        protected override void OnExecute(object parameter)
        {
            var process = new Process();
            var processStartInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                FileName = "cmd.exe",
                Arguments = $"/C {_cmdCommand} {ERROR_LEVEL}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
            };
            StringDictionary currentEnvVariables = processStartInfo.EnvironmentVariables;
            process.StartInfo = processStartInfo;
            process.Start();
            process.WaitForExit();

            Output = process.StandardOutput.ReadToEnd();
            string errors = process.StandardError.ReadToEnd();

            if(Output.Contains("error", StringComparison.OrdinalIgnoreCase))
            {
                string errorMessage = $"Failed to execute {_cmdCommand}. Output: {Output} {errors}";
                LoggingService.Logger.Error(errorMessage);
                throw new Exception(errorMessage);
            }
        }
    }
}

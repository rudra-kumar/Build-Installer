using System.Diagnostics;
using LoggingLibrary;
using System;

namespace Build_Installer.Commands
{
    class CommandLine : Command
    {
        private string _cmdCommand;

        public string Output;

        public CommandLine(string cmdCommand)
        {
            _cmdCommand = cmdCommand;
        }

        protected override void OnExecute()
        {
            var process = new Process();
            var processStartInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                FileName = "cmd.exe",
                Arguments = $"/C {_cmdCommand}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };
            process.StartInfo = processStartInfo;
            process.Start();
            process.WaitForExit();

            Output = process.StandardOutput.ReadToEnd();
            string errors = process.StandardError.ReadToEnd();

            if(!string.IsNullOrEmpty(errors))
            {
                string errorMessage = $"Failed to execute {_cmdCommand}. Error {errors}";
                Log.Error(errorMessage);
                throw new Exception(errorMessage);
            }

            if(Output.Contains("error", StringComparison.OrdinalIgnoreCase))
            {
                string errorMessage = Output.Substring(Output.IndexOf("error", StringComparison.OrdinalIgnoreCase));
                Log.Error(errorMessage);
                throw new Exception(errorMessage);
            }
        }
    }
}

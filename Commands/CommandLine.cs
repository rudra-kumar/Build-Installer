using System.Diagnostics;
using LoggingLibrary;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Build_Installer.Commands
{
    class CommandLine : Command
    {
        private string _cmdCommand;
        private StringDictionary _additionalEnvVariables;

        public string Output;

        public CommandLine(string cmdCommand, StringDictionary environmentVariables = null)
        {
            _cmdCommand = cmdCommand;
            _additionalEnvVariables = environmentVariables ?? new StringDictionary();
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
                UseShellExecute = false,
            };
            StringDictionary currentEnvVariables = processStartInfo.EnvironmentVariables;
            AppendAdditionalEvnVariables(ref currentEnvVariables);
            process.StartInfo = processStartInfo;
            process.Start();
            process.WaitForExit();

            Output = process.StandardOutput.ReadToEnd();
            string errors = process.StandardError.ReadToEnd();

            if(!string.IsNullOrEmpty(errors))
            {
                string errorMessage = $"Failed to execute {_cmdCommand}. Error {errors}";
                Log.Error(errorMessage);
            }

            // #TODO - Now since the eorr handling only happens in this place, the error message can contain the full command output
            // Also add cmd command error handling to the execute method itself
            if(Output.Contains("error", StringComparison.OrdinalIgnoreCase))
            {
                string errorMessage = $"Failed to execute {_cmdCommand}. Error {errors}";
                Log.Error(errorMessage);
                throw new Exception(errorMessage);
            }
        }

        private void AppendAdditionalEvnVariables(ref StringDictionary currentVariables)
        {
            if (currentVariables == null) return;
            foreach (string newKey in _additionalEnvVariables.Keys)
            {
                if (currentVariables.ContainsKey(newKey))
                {
                    string currentVariableValue = currentVariables[newKey];
                    if (currentVariableValue.Contains(_additionalEnvVariables[newKey]))
                        continue;
                    currentVariables[newKey] = currentVariables[newKey] + $";{_additionalEnvVariables[newKey]}";
                }
                else
                    currentVariables.Add(newKey, _additionalEnvVariables[newKey]);
            }
        }
    }
}

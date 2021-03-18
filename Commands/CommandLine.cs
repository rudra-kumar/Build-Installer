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
        public string Output { get; private set; }
        private StringDictionary _additionalEnvVariables;
        private const string ERROR_LEVEL = "& if ERRORLEVEL 1 echo error";

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
                Arguments = $"/C {_cmdCommand} {ERROR_LEVEL}",
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
                Log.Error(errors);
            }

            if(Output.Contains("error", StringComparison.OrdinalIgnoreCase))
            {
                string errorMessage = $"Failed to execute {_cmdCommand}. Output: {Output}";
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

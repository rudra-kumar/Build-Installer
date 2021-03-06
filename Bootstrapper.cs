using System;
using System.Collections.Generic;
using System.Text;
using LoggingLibrary;
using Build_Installer.Loggers;
using System.Diagnostics;
using System.Collections;
using SharpAdbClient;
using System.IO;

namespace Build_Installer
{
    static class Bootstrapper
    {
        // Create a new data structure that inherits from tuple

        // There could be two folders pointing to different different versions of JRE
        // Find out why setting the environment variables from here doesn't work
        private static List<EnvironmentVariable> _environmentVariables = new List<EnvironmentVariable>
        {
            new EnvironmentVariable( "Path",  Constants.JrePath),
            new EnvironmentVariable( "Path", Constants.PlatformToolsPath),
            new EnvironmentVariable( "Path", Constants.BuildToolsPath)
        };

        private static bool _isInitialized = false;
        public static void Init()
        {
            Debug.Assert(!_isInitialized, "Bootstrapper already initialized");
            LoggingService.Provide(new TraceLogger());
            SetupEnvironmentVariables();
            var adb = new AdbServer();
            var result = adb.StartServer(Constants.AdbPath, restartServerIfNewer: false);
            _isInitialized = true;
        }

        private static void SetupEnvironmentVariables()
        {
            
            foreach (EnvironmentVariable environmentVariable in _environmentVariables)
            {
                IDictionary currentVariables = Environment.GetEnvironmentVariables();
                if (currentVariables.Contains(environmentVariable.Key))
                {
                    string currentVariableValue = currentVariables[environmentVariable.Key] as string;
                    if (currentVariableValue.Contains(environmentVariable.Value))
                        continue;

                    string newEnvironmentVariableValue = currentVariableValue + $";{environmentVariable.Value}";

                    Environment.SetEnvironmentVariable(environmentVariable.Key, newEnvironmentVariableValue);
                }
                else
                    Environment.SetEnvironmentVariable(environmentVariable.Key, environmentVariable.Value);
            }
        }
    }
}

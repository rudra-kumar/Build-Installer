using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Build_Installer.Commands;
using System.Collections.Specialized;
using System.IO;
using LoggingLibrary;
using System.Reflection;

namespace BuildInstallerTests
{
    [TestClass]
    public class CommandTests
    {
        [TestMethod]
        public void CommandLineExecuteTest()
        {
            CommandLine cmdCommand = new CommandLine("echo hello world");
            cmdCommand.Execute();
            Assert.IsTrue(cmdCommand.Output.Contains("hello world"));
        }

        [TestMethod]
        public void AdbCommandTest()
        {
            // #TODO - Find a way to get the path of executing directory for the build installer project
            string platformToolsPath = @"C:\dev\wpf\Build Installer\bin\Debug\netcoreapp3.1\platform-tools";
            StringDictionary additionalEnvironmentVariables = new StringDictionary
            {
                { "Path", platformToolsPath }
            };
            CommandLine adbDevicesCommand = new CommandLine("adb devices", additionalEnvironmentVariables);
            adbDevicesCommand.Execute();
            Log.Info(adbDevicesCommand.Output);
        }

        [TestMethod]
        public void InstallApkTest()
        {
            InstallAPK installAPKCommand = new InstallAPK(@"C:\Users\rudra\Downloads\QuickShortcutMaker_v2.4.0_apkpure.com.apk");
            installAPKCommand.Execute();
        }

        [TestMethod]
        public void JreTest()
        {
            string jrePath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "jre", "bin");
            StringDictionary javaHomeVariable = new StringDictionary()
            {
                {"Path", jrePath }
            };
            // #TODO - Environment variables can be set from here aswell 

            System.Environment.SetEnvironmentVariable("path", jrePath);
            CommandLine javaVersionTest = new CommandLine("java -version");
            bool hasJava = true;
            try
            {
                javaVersionTest.Execute();
            }
            catch (Exception e)
            {
                hasJava = false;
            }
            Assert.IsTrue(hasJava);
        }


    }
}

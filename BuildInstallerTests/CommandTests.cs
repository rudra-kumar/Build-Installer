using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Build_Installer.Commands;
using System.Collections.Specialized;
using System.IO;
using LoggingLibrary;
using System.Reflection;
using Build_Installer;

namespace BuildInstallerTests
{
    [TestClass]
    public class CommandTests
    {
        [TestMethod]
        public void CommandLineExecuteTest()
        {
            CMDCommand cmdCommand = new CMDCommand("echo hello world");
            cmdCommand.Execute(null);
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
            CMDCommand adbDevicesCommand = new CMDCommand("adb devices", additionalEnvironmentVariables);
            adbDevicesCommand.Execute(null);
            LoggingService.Logger.Info(adbDevicesCommand.Output);
        }

        [TestMethod]
        public void InstallApkTest()
        {
            InstallAPK installAPKCommand = new InstallAPK(@"C:\Users\rudra\Downloads\QuickShortcutMaker_v2.4.0_apkpure.com.apk");
            installAPKCommand.Execute(null);
        }

        [TestMethod]
        public void JreTest()
        {
            Bootstrapper.Init();
            string jrePath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "jre", "bin");
            StringDictionary javaHomeVariable = new StringDictionary()
            {
                {"Path", jrePath }
            };
            // #TODO - Environment variables can be set from here aswell 

            // System.Environment.SetEnvironmentVariable("path", jrePath);
            CMDCommand javaVersionTest = new CMDCommand("java -version");
            bool hasJava = true;
            try
            {
                javaVersionTest.Execute(null);
            }
            catch (Exception)
            {
                hasJava = false;
            }
            Assert.IsTrue(hasJava);
        }


    }
}

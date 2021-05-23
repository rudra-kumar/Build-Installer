using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Build_Installer.Commands;
using LoggingLibrary;
using Build_Installer;

namespace BuildInstallerTests
{
    [TestClass]
    public class CommandTests
    {
        [ClassInitialize]
        public static void Init(TestContext context)
        {
            Bootstrapper.Init();
        }

        [TestMethod]
        public void AdbCommandTest()
        {
            CMDCommand adbDevicesCommand = new CMDCommand("adb devices");
            adbDevicesCommand.Execute(null);
            LoggingService.Logger.Info(adbDevicesCommand.Output);
        }

        [TestMethod]
        public void CommandLineExecuteTest()
        {
            CMDCommand cmdCommand = new CMDCommand("echo hello world");
            cmdCommand.Execute(null);
            Assert.IsTrue(cmdCommand.Output.Contains("hello world"));
        }

        [TestMethod]
        public void JreTest()
        {
            CMDCommand javaVersionTest = new CMDCommand("java -version");
            string errorMessage = string.Empty;
            bool hasJava = true;
            try
            {
                javaVersionTest.Execute(null);
            }
            catch (Exception e)
            {
                hasJava = false;
                errorMessage = e.Message;
            }
            Assert.IsTrue(hasJava, $"Output: {javaVersionTest.Output} {errorMessage}");
        }


    }
}

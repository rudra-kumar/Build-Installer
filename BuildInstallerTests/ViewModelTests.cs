using Build_Installer.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace BuildInstallerTests
{
    [TestClass]
    public class ViewModelTests
    {
        [TestMethod]
        public void MessageDialogTest()
        {
            var messageDialogViewModel = new MessageDialogVM(string.Empty);
            Assert.IsFalse(messageDialogViewModel.IsVisible, "Message dialog box should not be visible when initialised");
            messageDialogViewModel.Message = "Some message";
            Assert.IsTrue(messageDialogViewModel.IsVisible, "Message dialog box should become visibile when there is a message to show");
            messageDialogViewModel.ClearMessageCommand.Execute(null);
            Assert.IsFalse(messageDialogViewModel.IsVisible, "Message dialog box should not be visible when clear message command is run");
            Assert.IsTrue(string.IsNullOrEmpty(messageDialogViewModel.Message), "Message should be cleared after running clear message command");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Build_Installer.Commands;

namespace Build_Installer.ViewModels
{
    class MessageDialogViewModel : DependencyObject
    {
        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register(nameof(Message), typeof(string), typeof(MessageDialogViewModel));

        public ICommand ClearMessageCommand { get; private set; }

        public MessageDialogViewModel()
        {
            ClearMessageCommand = new ClearMessageCommand(() =>
            {
                Message = string.Empty;
            });
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Build_Installer.Commands;

namespace Build_Installer.ViewModels
{
    class MessageDialogVM : DialogVM
    {
        private string _message;
        public string Message {
            
            get => _message;
            set
            {
                if (string.Equals(value, _message))
                    return;
                _message = value;

                NotifyPropertyChanged(nameof(Message));

                if (string.IsNullOrEmpty(_message))
                    IsVisible = false;
                else
                    IsVisible = true;
            }
        }

        public ICommand ClearMessageCommand { get; private set; }

        public MessageDialogVM(string message)
        {
            ClearMessageCommand = new RelayCommand(() =>
            {
                Message = string.Empty;
            });
            Message = message;
        }
    }
}

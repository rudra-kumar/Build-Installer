using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Build_Installer.Commands;

namespace Build_Installer.ViewModels
{
    class MessageDialogViewModel : INotifyPropertyChanged
    {
        private string _message;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Message {
            get => _message;
            
            set
            {
                if (string.Equals(value, _message))
                    return;
                _message = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Message)));

                if (string.IsNullOrEmpty(_message))
                    IsVisible = false;
                else
                    IsVisible = true;
            }
        }

        private bool _isVisible;
        public bool IsVisible {
            get => _isVisible;
            set
            {
                if (_isVisible == value)
                    return;
                _isVisible = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsVisible)));
            }
        }

        public ICommand ClearMessageCommand { get; private set; }

        public MessageDialogViewModel()
        {
            ClearMessageCommand = new RelayCommand(() =>
            {
                Message = string.Empty;
            });
        }
    }
}

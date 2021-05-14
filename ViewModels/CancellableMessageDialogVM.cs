using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Build_Installer.Commands;

namespace Build_Installer.ViewModels
{
    class CancellableMessageDialogVM : DialogVM
    {
        public string OkButtonTitle { get; set; } = "Ok";
        public string CancelButtonTitle { get; set; } = "Cancel";
        public string Title { get; set; }
        public string Message { get; set; }

        private Action _onOKPRessed;
        private Action _onCancelPressed;

        public ICommand OkButtonCommand { get; set; }
        public ICommand CancelButtonCommand { get; set; }

        public CancellableMessageDialogVM(string title, string message, Action onOKPressed, Action OnCancelPressed)
        {
            _onOKPRessed = onOKPressed;
            _onCancelPressed = OnCancelPressed;
            Title = title;
            Message = message;
            IsVisible = true;

            SetupButtonCommands();
        }

        public async Task AwaitResponse()
        {
            while(IsVisible)
            {
                await Task.Delay(25);
            }
        }

        private void SetupButtonCommands()
        {
            OkButtonCommand = new RelayCommand(() =>
            {
                _onOKPRessed?.Invoke();
                IsVisible = false;
            });

            CancelButtonCommand = new RelayCommand(() =>
            {
                _onCancelPressed?.Invoke();
                IsVisible = false;
            });
        }
    }
}

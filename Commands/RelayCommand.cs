using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Build_Installer.Commands
{
    class RelayCommand : ICommand
    {
        private Action _commandAction;
        private Func<object, bool> _canExecuteAction;

#pragma warning disable CS0067
        public event EventHandler CanExecuteChanged;
#pragma warning restore CS0067
        public bool CanExecute(object parameter)
        {
            if (_canExecuteAction != null)
                return _canExecuteAction.Invoke(parameter);
            return true;
        }

        public void Execute(object parameter)
        {
            if (_commandAction == null)
                throw new Exception("Command Action is null");
            _commandAction.Invoke();
        }

        public RelayCommand(Action commandAction, Func<object, bool> canExecuteAction)
        {
            _commandAction = commandAction;
            _canExecuteAction = canExecuteAction;
        }

        public void RaiseOnExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }
    }
}

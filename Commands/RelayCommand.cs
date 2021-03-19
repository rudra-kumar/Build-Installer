using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Build_Installer.Commands
{
    class RelayCommand : ICommand
    {
        private Action _commandAction;

#pragma warning disable CS0067
        public event EventHandler CanExecuteChanged;
#pragma warning restore CS0067
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (_commandAction == null)
                throw new Exception("Command Action is null");
            _commandAction.Invoke();
        }

        public RelayCommand(Action commandAction)
        {
            _commandAction = commandAction;
        }
    }
}

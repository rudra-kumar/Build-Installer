using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Build_Installer.Commands
{
    class ClearMessageCommand : ICommand
    {
        private Action _clearMessageAction;

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _clearMessageAction?.Invoke();
        }

        public ClearMessageCommand(Action clearMessageAction)
        {
            _clearMessageAction = clearMessageAction;
        }
    }
}

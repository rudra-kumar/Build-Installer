using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Build_Installer.Commands
{
    abstract class StatelessCommand : ICommand
    {
#pragma warning disable CS0067
        public event EventHandler CanExecuteChanged;
#pragma warning restore CS0067

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            OnExecute(parameter);
        }

        // To ensure stateless and statefull have the same interface
        protected abstract void OnExecute(object parameter);
    }
}

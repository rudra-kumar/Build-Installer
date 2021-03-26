using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Build_Installer.Commands
{
    // For commands that are bound to ui and should return false when execution is in progress
    abstract class StatefullCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private bool _isExecuting;
        private bool IsExecuting
        {
            get => _isExecuting;
            set
            {
                if(IsExecuting != value)
                {
                    _isExecuting = value;
                    ThreadingExtensions.DispatchOnUIThread(() => RaiseCanExecuteChanged());
                }
            }
        }

        private void RaiseCanExecuteChanged()
        {
            CanExecuteChanged.Invoke(this, new EventArgs());
        }

        public bool CanExecute(object parameter)
        {
            return !IsExecuting;
        }

        public void Execute(object parameter)
        {
            try
            {
                IsExecuting = true;
                OnExecute(parameter);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                IsExecuting = false;
            }
        }

        protected abstract void OnExecute(object parameter);
    }
}

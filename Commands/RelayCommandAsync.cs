using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Build_Installer.Commands
{
    class RelayCommandAsync : ICommandAsync
    {
        public event EventHandler CanExecuteChanged;

        private bool _isExecuting;
        private IErrorHandler _errorHandler;
        private Func<Task> _executeAction;
        private Func<bool> _canExecuteAction;

        public RelayCommandAsync(IErrorHandler errorHandler, Func<Task> executeAction, Func<bool> canExecuteAction = null)
        {
            _errorHandler = errorHandler;
            _executeAction = executeAction;
            _canExecuteAction = canExecuteAction;
        }

        public bool CanExecute()
        {
            return !_isExecuting && (_canExecuteAction?.Invoke() ?? true);
        }


        public async Task ExecuteAsync()
        {
            if(CanExecute())
            {
                _isExecuting = true;
                RaiseCanExecuteChanged();
                await _executeAction.Invoke();
                _isExecuting = false;
                RaiseCanExecuteChanged();
            }
        }

        private void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        #region ICommand Overrides
        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute();
        }

        void ICommand.Execute(object parameter)
        {
            ExecuteAsync().FireAndForgetSafeAsync(_errorHandler);
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Build_Installer.Commands
{
    interface ICommandAsync : ICommand
    {
        Task ExecuteAsync();
        bool CanExecute();
    }

    interface ICommandAsync<T> : ICommand
    {
        Task ExecuteAsync(T parameter);
        bool CanExecute(T parameter);
    }
}

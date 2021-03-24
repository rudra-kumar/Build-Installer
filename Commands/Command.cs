using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Build_Installer.Commands
{
    abstract class Command : ICommand
    {
        public void Execute(object parameter = null)
        {
            OnExecute(parameter);
            ExecuteChildCommands();
        }


        public event EventHandler CanExecuteChanged;


        public abstract bool CanExecute(object parameter);

        public int ChildCommandCount
        {
            get
            {
                int count = 0;
                foreach (Command command in ChildCommands)
                {
                    count += command.ChildCommandCount;
                }
                count += ChildCommands.Count;
                return count;
            }
        }

        protected abstract void OnExecute(object parameter);

        protected List<Command> ChildCommands = new List<Command>();

        protected void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, new EventArgs());
            CommandManager.InvalidateRequerySuggested();
        }

        

        private void ExecuteChildCommands()
        {
            foreach (Command command in ChildCommands)
            {
                command.Execute();
            }
        }


        
    }
}

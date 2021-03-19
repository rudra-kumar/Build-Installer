using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Build_Installer.Commands
{
    abstract class Command : ICommand
    {
        public void Execute()
        {
            OnExecute();
            ExecuteChildCommands();
        }

        public void Execute(object parameter)
        {
            Execute();
        }
#pragma warning disable CS0067
        public event EventHandler CanExecuteChanged;
#pragma warning restore CS0067

        public bool CanExecute(object parameter)
        {
            return true;
        }

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

        protected abstract void OnExecute();

        protected List<Command> ChildCommands = new List<Command>();

        

        private void ExecuteChildCommands()
        {
            foreach (Command command in ChildCommands)
            {
                command.Execute();
            }
        }


        
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Build_Installer.Commands
{
    abstract class Command
    {
        public void Execute()
        {
            OnExecute();
            ExecuteChildCommands();
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

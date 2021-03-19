using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Microsoft.Win32;

namespace Build_Installer.Commands
{
    public class FileSelectedEventArgs : EventArgs
    {
        public string FilePath;

        public FileSelectedEventArgs(string filePath)
        {
            FilePath = filePath;
        }
    }

    class OpenFileCommand : ICommand
    {
#pragma warning disable CS0067
        public event EventHandler CanExecuteChanged;
#pragma warning restore CS0067
        public event EventHandler FileSelected;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Apk Files (.apk)|*.apk";

            Nullable<bool> result = openFileDialog.ShowDialog();

            if (result == true)
            {
                FileSelected.Invoke(this, new FileSelectedEventArgs(openFileDialog.FileName));
            }
        }
    }
}

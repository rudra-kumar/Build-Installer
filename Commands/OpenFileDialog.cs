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

    class OpenFileDialog : StatefullCommand
    {
        public event EventHandler FileSelected;

        protected override void OnExecute(object parameter)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Apk Files (.apk)|*.apk";

            Nullable<bool> result = openFileDialog.ShowDialog();

            if (result == true)
            {
                FileSelected.Invoke(this, new FileSelectedEventArgs(openFileDialog.FileName));
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Build_Installer.Commands;

namespace Build_Installer.ViewModels
{
    class MainViewModel : DependencyObject
    {
        public ICommand InstallApkCommand { get; set; }
        public static readonly DependencyProperty BuildPathProperty = DependencyProperty.Register(nameof(BuildPath), typeof(string), typeof(MainViewModel));
        public string BuildPath 
        {
            get => (string)GetValue(BuildPathProperty);
            set => SetValue(BuildPathProperty, value);
        }

        public MainViewModel()
        {
            InstallApkCommand = new RelayCommand(InstallApk);
        }

        private void InstallApk()
        {
            try
            {
                InstallAPK installAPK = new InstallAPK(BuildPath);
                installAPK.Execute();
                MessageBox.Show("Installed Successfully");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            
        }
    }
}

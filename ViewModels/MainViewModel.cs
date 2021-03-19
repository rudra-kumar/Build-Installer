using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Build_Installer.Commands;

namespace Build_Installer.ViewModels
{
    class MainViewModel : DependencyObject, IDisposable
    {
        public ICommand InstallBuildCommand { get; private set; }
        public OpenFileCommand OpenFileCommand { get; private set; }
        public static readonly DependencyProperty BuildPathProperty = DependencyProperty.Register(nameof(BuildPath), typeof(string), typeof(MainViewModel));
        public string BuildPath 
        {
            get => (string)GetValue(BuildPathProperty);
            set => SetValue(BuildPathProperty, value);
        }

        public static readonly DependencyProperty BuildProgressProperty = DependencyProperty.Register(nameof(BuildProgress), typeof(int), typeof(MainViewModel));
        public int BuildProgress
        {
            get => (int)GetValue(BuildProgressProperty);
            set => SetValue(BuildProgressProperty, value);
        }



        public string ProgressMessage
        {
            get { return (string)GetValue(ProgressMessageProperty); }
            set { SetValue(ProgressMessageProperty, value); }
        }

        public static readonly DependencyProperty ProgressMessageProperty =
            DependencyProperty.Register("ProgressMessage", typeof(string), typeof(MainViewModel), new PropertyMetadata(null));



        private SynchronizationContext _syncContext;

        public MainViewModel()
        {
            _syncContext = SynchronizationContext.Current;
            InstallBuildCommand = new RelayCommand(InstallBuild);
            OpenFileCommand = new OpenFileCommand();
            OpenFileCommand.FileSelected += OnFileSelected;
        }

        private void OnFileSelected(object sender, EventArgs eventArgs)
        {
            FileSelectedEventArgs fileSelectedArgs = eventArgs as FileSelectedEventArgs;
            BuildPath = fileSelectedArgs.FilePath;
        }

        private async void InstallBuild()
        {
            InstallBuild installBuild = new InstallBuild(BuildPath);
            installBuild.ProgressChanged += OnProgressChanged;
            try
            {
                await Task.Run( () => installBuild.Execute());
                MessageBox.Show("Installed Successfully");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                installBuild.ProgressChanged -= OnProgressChanged;
                BuildProgress = 0;
                ProgressMessage = string.Empty;
            }
        }

        private void OnProgressChanged(object obj, ProgressChangedEventArgs eventArgs)
        {
            // Use the syncronization context to call the method on the main thread (UI thread) 
            _syncContext.Post(o =>
            {
                BuildProgress = eventArgs.Progress;
                ProgressMessage = eventArgs.Description;
            },
            null);
        }

        public void Dispose()
        {
            OpenFileCommand.FileSelected -= OnFileSelected;
        }
    }
}

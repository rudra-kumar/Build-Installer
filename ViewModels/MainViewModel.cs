using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Build_Installer.Commands;
using LoggingLibrary;

namespace Build_Installer.ViewModels
{
    class MainViewModel : DependencyObject, IDisposable
    {
        public RelayCommand InstallBuildCommand { get; private set; }
        public OpenFileDialog OpenFileCommand { get; private set; }
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
        private InstallBuild _installBuild;

        public MainViewModel()
        {
            Bootstrapper.Init();
            _syncContext = SynchronizationContext.Current;
            _installBuild = new InstallBuild(BuildPath);
            InstallBuildCommand = new RelayCommand(InstallBuild, _installBuild.CanExecute);
            _installBuild.CanExecuteChanged += (param, args) => InstallBuildCommand.RaiseOnExecuteChanged();
            OpenFileCommand = new OpenFileDialog();
            OpenFileCommand.FileSelected += OnFileSelected;
        }

        private void OnFileSelected(object sender, EventArgs eventArgs)
        {
            FileSelectedEventArgs fileSelectedArgs = eventArgs as FileSelectedEventArgs;
            BuildPath = fileSelectedArgs.FilePath;
        }

        private async void InstallBuild()
        {
            string buildPath = BuildPath;
            try
            {
                _installBuild.ProgressChanged += OnProgressChanged;
                await Task.Run( () => _installBuild.Execute(buildPath));
                MessageBox.Show("Installed Successfully");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                _installBuild.ProgressChanged -= OnProgressChanged;
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

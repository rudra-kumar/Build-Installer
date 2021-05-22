using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Build_Installer.Commands;
using LoggingLibrary;
using SharpAdbClient;

namespace Build_Installer.ViewModels
{
    // #TODO - Update to View Model Base class
    class MainViewModel : DependencyObject, IDisposable, INotifyPropertyChanged, IErrorHandler
    {
        public RelayCommandAsync InstallBuildCommand { get; private set; }
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

        private AndroidBuildInstaller _installationService;



        public MessageDialogVM Message
        {
            get { return (MessageDialogVM)GetValue(ErrorMessageProperty); }
            set { SetValue(ErrorMessageProperty, value); }
        }
        public static readonly DependencyProperty ErrorMessageProperty =
            DependencyProperty.Register(nameof(Message), typeof(MessageDialogVM), typeof(MainViewModel));


        private CancellableMessageDialogVM _cancellableMessageDialogVM;
        

        public CancellableMessageDialogVM CancellableMessageDialog
        {
            get => _cancellableMessageDialogVM;
            set
            {
                _cancellableMessageDialogVM = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CancellableMessageDialog)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        
        private DevicesViewModel _devicesViewModel;
        public DevicesViewModel DevicesViewModel 
        {
            get => _devicesViewModel;
            set 
            {
                _devicesViewModel = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DevicesViewModel)));
            }
        }

        public MainViewModel()
        {
            Bootstrapper.Init();
            AdbClient adbClient = new AdbClient();
            InstallationService.Provide(adbClient);
            var deviceMonitor = new DeviceMonitor(new AdbSocket(new IPEndPoint(IPAddress.Loopback, AdbClient.AdbServerPort)));
            deviceMonitor.Start();
            _installationService = new AndroidBuildInstaller(OnUserPrompt);
            InstallBuildCommand = new RelayCommandAsync(this, InstallBuild);
            OpenFileCommand = new OpenFileDialog();
            OpenFileCommand.FileSelected += OnFileSelected;
            DevicesViewModel = new DevicesViewModel(adbClient, deviceMonitor);
        }

        private async Task<bool> OnUserPrompt(string message)
        {
            bool userPressedOk = false;
            CancellableMessageDialog = new CancellableMessageDialogVM(
                "Error",
                message,
                () => userPressedOk = true,
                () => userPressedOk = false
                );
            await CancellableMessageDialog.AwaitResponse();
            return userPressedOk;
        }

        private void OnFileSelected(object sender, EventArgs eventArgs)
        {
            FileSelectedEventArgs fileSelectedArgs = eventArgs as FileSelectedEventArgs;
            BuildPath = fileSelectedArgs.FilePath;
        }

        private async Task InstallBuild()
        {
            string buildPath = BuildPath;
            try
            {
                _installationService.ProgressChanged += OnProgressChanged;
                Message.Message = "Installation Complete";
            }
            catch (Exception e)
            {
                Message.Message = e.Message;
            }
            finally
            {
                _installationService.ProgressChanged -= OnProgressChanged;
                BuildProgress = 0;
                ProgressMessage = string.Empty;
            }
        }

        private void OnProgressChanged(object obj, ProgressChangedEventArgs eventArgs)
        {
            ThreadingExtensions.DispatchOnUIThread(() =>
           {
               BuildProgress = eventArgs.Progress;
               ProgressMessage = eventArgs.Description;
           });
        }

        public void Dispose()
        {
            OpenFileCommand.FileSelected -= OnFileSelected;
        }

        public void Handle(Exception exception)
        {
            Message.Message = exception.Message;
        }
    }
}

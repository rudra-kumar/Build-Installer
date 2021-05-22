using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Build_Installer.Commands;
using LoggingLibrary;
using SharpAdbClient;

namespace Build_Installer.ViewModels
{
    class AndroidDeviceViewModel : ViewModel, IErrorHandler, IEquatable<AndroidDeviceViewModel>
    {
        public ICommandAsync<string> InstallBuild { get; set; }
        public ICommandAsync BrowseAndInstallBuild { get; set; }

        private DeviceData _metaData;
        public DeviceData MetaData
        {
            get => _metaData;
            private set
            {
                _metaData = value;
                NotifyPropertyChanged(nameof(MetaData));
            }
        }

        private DialogVM _dialogVM;
        public DialogVM DialogVM
        {
            get => _dialogVM;
            private set
            {
                if (_dialogVM != null && _dialogVM.Equals(value))
                    return;
                _dialogVM = value;
                NotifyPropertyChanged(nameof(DialogVM));
            }
        }

        private int _buildProgress;
        public int BuildProgress 
        { 
            get => _buildProgress; 
            set
            {
                if (_buildProgress == value)
                    return;
                _buildProgress = value;
                NotifyPropertyChanged(nameof(BuildProgress));
            }
        }

        private string _progressMessage;
        public string ProgressMessage
        {
            get => _progressMessage;
            set
            {
                if (_progressMessage == value)
                    return;
                _progressMessage = value;
                NotifyPropertyChanged(nameof(ProgressMessage));
            }
        }

        public AndroidDeviceViewModel(DeviceData metaData)
        {
            InstallBuild = new RelayCommandAsync<string>(this, InstallBuildAction);
            BrowseAndInstallBuild = new RelayCommandAsync(this, BrowseAndInstallBuildAction);
            MetaData = metaData;
        }

        

        private async Task InstallBuildAction(string buildPath)
        {
            var service = new AndroidBuildInstaller(HandleUserPrompt);
            try
            {
                service.ProgressChanged += OnProgressChanged;
                await Task.Run(() => service.InstallBuild(buildPath, MetaData));
                DialogVM = new MessageDialogVM("Installation Complete");
            }
            catch (Exception e)
            {
                DialogVM = new MessageDialogVM(e.Message);
            }
            finally
            {
                service.ProgressChanged -= OnProgressChanged;
                ResetProgressBar();
            }
        }

        private async Task BrowseAndInstallBuildAction()
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Apk Files (.apk)|*.apk";

            Nullable<bool> result = openFileDialog.ShowDialog();

            if (result == true)
                await InstallBuildAction(openFileDialog.FileName);
        }

        private void ResetProgressBar()
        {
            BuildProgress = 0;
            ProgressMessage = string.Empty;
        }

        private void OnProgressChanged(object obj, ProgressChangedEventArgs eventArgs)
        {
            BuildProgress = eventArgs.Progress;
            ProgressMessage = eventArgs.Description;
        }

        private async Task<bool> HandleUserPrompt(string message)
        {
            bool userPressedOk = false;
            DialogVM = new CancellableMessageDialogVM(
                "Error",
                message,
                () => userPressedOk = true,
                () => userPressedOk = false
                );
            await (DialogVM as CancellableMessageDialogVM).AwaitResponse();
            return userPressedOk;
        }

        public void Handle(Exception exception)
        {
            DialogVM = new MessageDialogVM(exception.Message);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as AndroidDeviceViewModel);
        }

        public bool Equals(AndroidDeviceViewModel other)
        {
            return other != null && this.MetaData.Serial == other.MetaData.Serial;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(MetaData);
        }

        public static bool operator ==(AndroidDeviceViewModel left, AndroidDeviceViewModel right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(AndroidDeviceViewModel left, AndroidDeviceViewModel right)
        {
            return !(left == right);
        }
    }
}

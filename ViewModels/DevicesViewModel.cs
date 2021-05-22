using SharpAdbClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using LoggingLibrary;
using System.Threading;

namespace Build_Installer.ViewModels
{
    class DevicesViewModel : IDisposable
    {
        private readonly IDeviceMonitor _deviceMontior;
        private readonly IAdbClient _adbClient;
        
        public ObservableCollection<AndroidDeviceViewModel> AndroidDeviceViewModels { get; set; } = new ObservableCollection<AndroidDeviceViewModel>();

        public DevicesViewModel(IAdbClient adbClient, IDeviceMonitor deviceMonitor)
        {
            _deviceMontior = deviceMonitor;
            _adbClient = adbClient;
            UpdateConnectedDevices();
            _deviceMontior.DeviceConnected += OnDeviceConnected;
            _deviceMontior.DeviceDisconnected += OnDeviceDisconnected;
        }

        private void UpdateConnectedDevices()
        {
            // Here we're using adbclient to get devices instead of DeviceMonitor.GetDevices() because the latter
            // doesn't return the full DeviceData just the serial number
            IEnumerable<DeviceData> connectedDevices = _adbClient.GetDevices();
            foreach (DeviceData device in connectedDevices)
            {
                var deviceViewModel = new AndroidDeviceViewModel(device);
                ThreadingExtensions.DispatchOnUIThread(() => AndroidDeviceViewModels.Add(deviceViewModel));
            }
        }

        private void OnDeviceConnected(object sender, DeviceDataEventArgs e)
        {
            try
            {
                // Sleep for a few seconds otherwise device data not complete, this is a bug
                Thread.Sleep(1000);
                DeviceData connectedDevice = _adbClient.GetDevices().Find(device => device.Serial == e.Device.Serial);
                var connectedDeviceViewModel = new AndroidDeviceViewModel(connectedDevice);
                if (!AndroidDeviceViewModels.Contains(connectedDeviceViewModel))
                {
                    ThreadingExtensions.DispatchOnUIThread(() => AndroidDeviceViewModels.Add(connectedDeviceViewModel));
                }
            }
            catch (Exception exception)
            {
                LoggingService.Logger.Error(exception.Message);
            }
        }

        private void OnDeviceDisconnected(object sender, DeviceDataEventArgs e)
        {
            try
            {
                var disconnectedDeviceViewModel = new AndroidDeviceViewModel(e.Device);
                ThreadingExtensions.DispatchOnUIThread(() => AndroidDeviceViewModels.Remove(disconnectedDeviceViewModel));
            }
            catch (Exception exception)
            {
                LoggingService.Logger.Error(exception.Message);
            }
            
            
        }

        public void Dispose()
        {
            _deviceMontior.DeviceConnected -= OnDeviceConnected;
            _deviceMontior.DeviceDisconnected -= OnDeviceDisconnected;
        }
    }
}

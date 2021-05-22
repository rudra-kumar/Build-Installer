using LoggingLibrary;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Build_Installer.Models;
using SharpAdbClient;
using System.Linq;
using SharpAdbClient.DeviceCommands;
using System.IO;
using System.Net;

namespace Build_Installer
{
    class InstallationService
    {
        public static IAdbClient AdbClient { get; private set; } = new AdbClient();
        // Installation service will provide an adb client
        public static void Provide(IAdbClient adbClient)
        {
            if (adbClient == null)
                AdbClient = new AdbClient();
            else
                AdbClient = adbClient;
        }
    }
}

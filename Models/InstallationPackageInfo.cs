using LoggingLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Build_Installer.Models
{
    enum Platform
    { 
        Android,
        iOS
    }

    class InstallationPackageInfo
    {
        public string PackageName { get; private set; }
        public string VersionCode { get; private set; }
        public string VersionName { get; private set; }
        public string CompileSDKVersion { get; private set; }
        public string CompileSDKVersionCodename { get; private set; }
        public Platform Platform { get; private set; }

        private InstallationPackageInfo()
        {
        }

        public static InstallationPackageInfo ParseAndroid(string aaptCommandOutput)
        {
            var apptCommandReader = new StringReader(aaptCommandOutput);
            string rawPackageInfo = apptCommandReader.ReadLine();
            var installationPackage = new InstallationPackageInfo();
            installationPackage.Platform = Platform.Android;
            string[] splitPackageInfo = rawPackageInfo.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            foreach (string packageInfo in splitPackageInfo)
            {
                string[] keyValuePairs = packageInfo.Split('=');
                if (keyValuePairs.Length < 2)
                    continue;
                string key = keyValuePairs[0];
                string value = keyValuePairs[1].Trim('\'');
                switch (key)
                {
                    case "name":
                        installationPackage.PackageName = value;
                        break;
                    case "versionCode":
                        installationPackage.VersionCode = value;
                        break;
                    case "versionName":
                        installationPackage.VersionName = value;
                        break;
                    case "compileSdkVersion":
                        installationPackage.CompileSDKVersion = value;
                        break;
                    case "compileSdkVersionCodename":
                        installationPackage.CompileSDKVersionCodename = value;
                        break;
                    default:
                        LoggingService.Logger.Info($"Package info not parsed: {key} : {value}");
                        break;
                }
            }
            return installationPackage;
        }
    }
}

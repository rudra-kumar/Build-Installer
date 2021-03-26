﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Build_Installer
{
    static class Constants
    {
        public static readonly string JrePath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "jre", "bin");
        public static readonly string PlatformToolsPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "platform-tools");
    }
}

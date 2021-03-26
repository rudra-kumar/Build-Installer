using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using LoggingLibrary;

namespace Build_Installer.Loggers
{
    class TraceLogger : ILogger
    {
        public void Info(string message)
        {
            Trace.TraceInformation(message);
        }

        public void Warning(string message)
        {
            Trace.TraceWarning(message);
        }

        public void Error(string message)
        {
            Trace.TraceError(message);
        }
    }
}

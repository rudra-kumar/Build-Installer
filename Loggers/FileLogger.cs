using System;
using System.Collections.Generic;
using System.Text;
using LoggingLibrary;

namespace Build_Installer.Loggers
{
    class FileLogger
    {
        private ILogger _wrapped;
        private string _filePath;
        public FileLogger(string filePath, ILogger wrapped = null)
        {
            _wrapped = wrapped ?? new NullLogger();
            _filePath = filePath;
        }
    }
}
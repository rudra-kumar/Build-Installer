using System;
using System.Collections.Generic;
using System.Text;

namespace Build_Installer
{
    class EnvironmentVariable
    {
        private Tuple<string, string> _environmentVariable;

        public EnvironmentVariable(string key, string value) 
        {
            _environmentVariable = new Tuple<string, string>(key, value);
        }

        public string Key => _environmentVariable.Item1;
        public string Value => _environmentVariable.Item2;
    }
}

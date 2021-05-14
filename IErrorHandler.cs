using System;
using System.Collections.Generic;
using System.Text;

namespace Build_Installer
{
    interface IErrorHandler
    {
        void Handle(Exception exception);
    }
}

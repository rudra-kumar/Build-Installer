using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace Build_Installer
{
    static class ThreadingExtensions
    {
        public static void DispatchOnUIThread(this Action action)
        {
            Application.Current.Dispatcher.Invoke(action);
        }
    }
}

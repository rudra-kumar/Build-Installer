using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
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

        public static async void FireAndForgetSafeAsync(this Task task, IErrorHandler errorHandler)
        {
            try
            {
                await task;
            }
            catch (Exception e)
            {
                errorHandler.Handle(e);
            }
        }
    }
}

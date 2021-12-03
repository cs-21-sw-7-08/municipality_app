using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace MunicipalityApp
{
    public class GeneralUtil
    {
        /// <summary>
        /// Run function that finds out by itself to call invoke
        /// </summary>        
        public static void RunOnUIThread(Action action)
        {
            if (Thread.CurrentThread == Application.Current.Dispatcher.Thread)
                action();
            else
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, action);
        }

        public static void ShowMessage(string message)
        {
            RunOnUIThread(() =>
            {
                MessageBox.Show(message);
            });
        }
    }
}

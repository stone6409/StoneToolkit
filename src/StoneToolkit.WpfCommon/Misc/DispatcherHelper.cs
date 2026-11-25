using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace StoneToolkit.WpfCommon.Misc
{
    public static class DispatcherHelper
    {
        public static void BeginInvoke(Delegate method)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, method);
        }

        public static bool CurrentIsUiThread()
        {
            return Thread.CurrentThread == Application.Current.Dispatcher.Thread;
        }

        [SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public static void DoEvents()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(ExitFrame), frame);
            Dispatcher.PushFrame(frame);
        }

        private static object ExitFrame(object frame)
        {
            ((DispatcherFrame)frame).Continue = false;
            return null;
        }
    }
}

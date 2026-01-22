using System;
using System.Collections.Generic;
using System.Text;

namespace EasePass.Helper
{
    internal static class UIThreadInvoker
    {
        public static void Invoke(Action action)
        {
            MainWindow.CurrentInstance.DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, () =>
            {
                action();
            });
        }
    }
}

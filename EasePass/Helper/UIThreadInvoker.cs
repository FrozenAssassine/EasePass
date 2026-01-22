/*
MIT License

Copyright (c) 2023 Julius Kirsch

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.
*/

using System;

namespace EasePass.Helper
{
    internal static class UIThreadInvoker
    {
        /// <summary>
        /// Use to call UI code from non-UI threads.
        /// </summary>
        /// <param name="action">The code to un in UI thread.</param>
        public static void Invoke(Action action)
        {
            MainWindow.CurrentInstance.DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, () =>
            {
                action();
            });
        }
    }
}

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

using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;

namespace EasePass.Helper
{
    internal class WindowStateHelper
    {
        public static OverlappedPresenterState GetWindowState(Window window)
        {
            return (window.AppWindow.Presenter as OverlappedPresenter).State;
        }

        public static void SetWindowState(Window window, OverlappedPresenterState state)
        {
            var presenter = window.AppWindow.Presenter as OverlappedPresenter;
            if (state == OverlappedPresenterState.Maximized)
                presenter.Maximize();
            else if (state == OverlappedPresenterState.Minimized)
                presenter.Minimize();
        }
    }
}

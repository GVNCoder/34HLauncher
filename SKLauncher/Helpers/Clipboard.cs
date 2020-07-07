using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Launcher.Core.Interaction;

namespace Launcher.Helpers
{
    public static class Clipboard
    {
        private const uint CF_UNICODETEXT = 13;
        private const int ERROR_SUCCESS = 0x00000000;

        private static readonly IntPtr _pHnd;

        static Clipboard()
        {
            _pHnd = /*Process.GetCurrentProcess().MainWindowHandle*/ IntPtr.Zero;
        }

        public static void CopyToClipboard(string dataForCopy) => _setTextToClipboard(dataForCopy);

        public static ICommand CopyCommand => new DelegateCommand(obj =>
        {
            if (obj == null) return;
            if (! (obj is string dataForCopy)) throw new InvalidCastException("Cannot cast Clipboard data to string");

            _setTextToClipboard(dataForCopy);
        });

        private static void _setTextToClipboard(string dataForCopy)
        {
            try
            {
                System.Windows.Clipboard.Clear();
                System.Windows.Clipboard.SetDataObject(dataForCopy, true);
            }
            catch (ExternalException ex)
            {
                var app = (App) Application.Current;
                var hWnd = Win32Methods.GetOpenClipboardWindow();
                if (hWnd != IntPtr.Zero)
                {
                    var tLength = Win32Methods.GetWindowTextLength(hWnd);
                    var tBuilder = new StringBuilder(tLength);
                    Win32Methods.GetWindowText(hWnd, tBuilder, tLength);

                    app.Logger.Info($"OpenClipboardWindowTitle {tBuilder}");
                }
                else
                {
                    app.Logger.Error($"Clipboard error 0x{ex.ErrorCode}");
                }
            }

            //var openHandleResult = Win32Methods.OpenClipboard(_pHnd);
            //if (openHandleResult)
            //{
            //    var hData = Marshal.StringToHGlobalUni(dataForCopy);

            //    Win32Methods.EmptyClipboard();
            //    Win32Methods.SetClipboardData(CF_UNICODETEXT, hData);
            //}

            //Win32Methods.CloseClipboard();

            //var lError = Marshal.GetLastWin32Error();
            //if (lError == ERROR_SUCCESS) return;
        }
    }
}
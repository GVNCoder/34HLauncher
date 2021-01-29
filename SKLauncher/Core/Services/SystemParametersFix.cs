using System;
using System.Runtime.InteropServices;
using System.Windows;

// ReSharper disable All

namespace Launcher.Core.Services
{
    public static class SystemParametersFix
    {
        public static Thickness WindowResizeBorderThickness
        {
            get
            {
                var dpix = GetDpi(GetDeviceCapsIndex.LOGPIXELSX);
                var dpiy = GetDpi(GetDeviceCapsIndex.LOGPIXELSY);

                var dx = GetSystemMetrics(GetSystemMetricsIndex.CXFRAME);
                var dy = GetSystemMetrics(GetSystemMetricsIndex.CYFRAME);

                // this adjustment is needed only since .NET 4.5 
                var d = GetSystemMetrics(GetSystemMetricsIndex.SM_CXPADDEDBORDER);

                dx += d;
                dy += d;

                var leftBorder = dx / dpix;
                var topBorder = dy / dpiy;

                return new Thickness(leftBorder, topBorder, leftBorder, topBorder);
            }
        }

        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("gdi32.dll")]
        private static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        private static float GetDpi(GetDeviceCapsIndex index)
        {
            IntPtr desktopWnd = IntPtr.Zero;
            IntPtr dc = GetDC(desktopWnd);
            float dpi;
            try
            {
                dpi = GetDeviceCaps(dc, (int)index);
            }
            finally
            {
                ReleaseDC(desktopWnd, dc);
            }
            return dpi / 96f;
        }

        private enum GetDeviceCapsIndex
        {
            LOGPIXELSX = 88,
            LOGPIXELSY = 90
        }

        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(GetSystemMetricsIndex nIndex);

        private enum GetSystemMetricsIndex
        {
            CXFRAME = 32,
            CYFRAME = 33,
            SM_CXPADDEDBORDER = 92
        }
    }
}
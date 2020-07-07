using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Launcher.Helpers
{
    public static class Win32Methods
    {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool OpenClipboard(IntPtr hWndNewOwner);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool CloseClipboard();

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetClipboardData(uint uFormat, IntPtr hMem);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool EmptyClipboard();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr GetOpenClipboardWindow();
    }
}
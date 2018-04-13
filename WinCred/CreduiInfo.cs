using System;
using System.Runtime.InteropServices;

namespace Advapi32.WinCred
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct CreduiInfo
    {
        public uint cbSize;
        public IntPtr hwndParent;
        public string pszMessageText;
        public string pszCaptionText;
        public IntPtr hbmBanner;
    }
}

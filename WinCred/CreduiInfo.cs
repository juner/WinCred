using System;

namespace Advapi32.WinCred
{
    public struct CreduiInfo
    {
        public uint cbSize;
        public IntPtr hwndParent;
        public string pszMessageText;
        public string pszCaptionText;
        public IntPtr hbmBanner;
    }
}

using System;
using System.Runtime.InteropServices;

namespace Advapi32.WinCred
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct BinaryBlobCredntialInfo
    {
        public uint cbBlob;
        public IntPtr pbBlob;
    }
}

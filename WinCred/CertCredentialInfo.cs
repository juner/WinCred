using System.Runtime.InteropServices;

namespace Advapi32.WinCred
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct CertCredentialInfo
    {
        public uint cbSize;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public byte[] rgbHashOfCert;
    }

}

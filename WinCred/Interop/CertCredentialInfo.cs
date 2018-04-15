using System.Runtime.InteropServices;

namespace Advapi32.WinCred.Unmanaged
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct CertCredentialInfo
    {
        public uint Size;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public byte[] RgbHashOfCert;
    }

}

using System.Runtime.InteropServices;

namespace Advapi32.WinCred.Unmanaged
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct UsernameTargetCredentialInfo
    {
        public string UserName;
    }
}

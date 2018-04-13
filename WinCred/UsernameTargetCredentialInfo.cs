using System.Runtime.InteropServices;

namespace Advapi32.WinCred
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct UsernameTargetCredentialInfo
    {
        public string UserName;
    }
}

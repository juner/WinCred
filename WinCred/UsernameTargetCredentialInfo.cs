using System;
using System.Runtime.InteropServices;

namespace Advapi32.WinCred
{
    public class UsernameTargetCredentialInfo : BaseCredentialInfo
    {
        public string UserName { get; }
        public UsernameTargetCredentialInfo(IntPtr Credential) : base(CredMarshalType.UsernameTargetCredential, Credential)
        {
            var info = Marshal.PtrToStructure<Unmanaged.UsernameTargetCredentialInfo>(Credential);
            UserName = info.UserName;
        }
    }
}

using System;

namespace Advapi32.WinCred
{
    public class BaseCredentialInfo : ICredMarshal
    {
        public CredMarshalType Type { get; }
        public IntPtr Credential { get; }
        public BaseCredentialInfo(CredMarshalType Type, IntPtr Credential)
        {
            this.Type = Type;
            this.Credential = Credential;
        }
    }
}

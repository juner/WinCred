using System;
using System.Runtime.InteropServices;

namespace Advapi32.WinCred
{
    public class CertCredentialInfo : BaseCredentialInfo
    {
        public byte[] RgbHashOfCert { get; }
        public CertCredentialInfo() : base(CredMarshalType.CertCredential, IntPtr.Zero)
        {
            RgbHashOfCert = new byte[20];
        }
        public CertCredentialInfo(IntPtr Credential) : base(CredMarshalType.CertCredential, Credential)
        {
            var info = Marshal.PtrToStructure<Unmanaged.CertCredentialInfo>(Credential);
            RgbHashOfCert = info.RgbHashOfCert;
        }
    }
}

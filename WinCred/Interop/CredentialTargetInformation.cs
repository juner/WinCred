using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Advapi32.WinCred.Unmanaged
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct CredentialTargetInformation :IUnmanaged<WinCred.CredentialTargetInformation>
    {
        public string TargetName;
        public string NetbiosServerName;
        public string DnsServerName;
        public string NetbiosDomainName;
        public string DnsDomainName;
        public string DnsTreeName;
        public string PackageName;
        public CredTiFlags Flags;
        private uint CredTypeCount;
        private IntPtr CredTypesPtr;
        public CredType[] CredTypes
        {
            get
            {
                if (CredTypesPtr == IntPtr.Zero || CredTypeCount <= 0)
                    return null;
                var Self = this;
                return Enumerable.Range(0, (int)CredTypeCount)
                    .Select(i => Marshal.ReadIntPtr(Self.CredTypesPtr, sizeof(CredType)))
                    .Select(p => (CredType)Marshal.PtrToStructure(p, typeof(CredType)))
                    .ToArray();
            }
        }
        public static ICredGetterHandle<CredentialTargetInformation> GetTargetInfo(string TargetName, CredFlags Flags)
        {
            throw new NotImplementedException();
        }
        public void WriteDomainCredentials(Credential Credential, CredFlags Flags)
        {
            throw new NotImplementedException();
        }
        public ICredGetterHandle<IEnumerable<Credential>> ReadDomainCredentials(CredFlags Flags)
        {
            throw new NotImplementedException();
        }

        public WinCred.CredentialTargetInformation ToManaged()
            => new WinCred.CredentialTargetInformation
        {
            TargetName = TargetName,
            NetbiosServerName = NetbiosServerName,
            DnsServerName = DnsServerName,
            NetbiosDomainName = NetbiosDomainName,
            DnsDomainName = DnsDomainName,
            DnsTreeName = DnsTreeName,
            PackageName = PackageName,
            Flags = Flags,
            CredTypes = CredTypes,
        };
    }
}
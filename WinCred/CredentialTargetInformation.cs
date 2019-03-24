using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advapi32.WinCred
{
    public class CredentialTargetInformation
    {
        public string TargetName { get; set; }
        public string NetbiosServerName { get; set; }
        public string DnsServerName { get; set; }
        public string NetbiosDomainName { get; set; }
        public string DnsDomainName { get; set; }
        public string DnsTreeName { get; set; }
        public string PackageName { get; set; }
        public CredTiFlags Flags { get; set; }
        public CredType[] CredTypes { get; set; }
        public static CredentialTargetInformation GetTargetInfo(string TargetName, CredGetTargetInfoFlags Flags = default)
            => Unmanaged.CredentialTargetInformation.GetTargetInfo(TargetName, Flags).Using(t => t.Value.ToManaged());
        public void WriteDomainCredentials(Credential Credential, CredWriteDomainCredentialsFlag Flags)
        {
            using (var cti = UnmanagedDisposableGetter<Unmanaged.CredentialTargetInformation, CredentialTargetInformation>.From(this))
            using (var c = UnmanagedDisposableGetter<Unmanaged.Credential, Credential>.From(Credential))
                cti.Value.WriteDomainCredentials(c.Value, Flags);
        }
        public IEnumerable<Credential> ReadDomainCredentials(CredReadDomainCredentialsFlags Flags = default)
            => UnmanagedDisposableGetter<Unmanaged.CredentialTargetInformation, CredentialTargetInformation>.From(this)
                ?.Using().Select(g => g.Value.ReadDomainCredentials(Flags)).Using(g => g.Value)?.Select(c => c.ToManaged())
            ?? Enumerable.Empty<Credential>();
    }
}


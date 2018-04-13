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
        public static ICredGetterHandle<CredentialTargetInformation> GetTargetInfo(string TargetName, CredFlags Flags)
        {
            throw new NotImplementedException();
        }
        public void WriteDomainCredentials(Credential Credential, CredWriteFlags Flags)
        {
            throw new NotImplementedException();
        }
        public ICredGetterHandle<IEnumerable<Credential>> ReadDomainCredentials(CredFlags Flags)
        {
            throw new NotImplementedException();
        }
    }
}


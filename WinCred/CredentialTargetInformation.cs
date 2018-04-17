﻿using System;
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
            var cti = new Unmanaged.CredentialTargetInformation();
            var c = new Unmanaged.Credential();
            using (cti.Copy(this))
            using (c.Copy(Credential))
                cti.WriteDomainCredentials(c, Flags);
        }
        public IEnumerable<Credential> ReadDomainCredentials(CredReadDomainCredentialsFlags Flags)
        {
            var cti = new Unmanaged.CredentialTargetInformation();
            using (cti.Copy(this))
            using (var getter = cti.ReadDomainCredentials(Flags))
                foreach (var c in getter.Value)
                    yield return c.ToManaged();
        }
    }
}


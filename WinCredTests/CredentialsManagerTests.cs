using Microsoft.VisualStudio.TestTools.UnitTesting;
using Advapi32.WinCred;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advapi32.WinCred.Tests
{
    [TestClass()]
    public class CredentialsManagerTests
    {
        [TestMethod()]
        public void GetSessionTypesTest()
        {
            foreach (var sessionType in CredentialsManager.GetSessionTypes(CredType.Generic))
                System.Diagnostics.Debug.WriteLine($"{sessionType.Key}:{sessionType.Value}");
        }
        [TestMethod()]
        public void MarshalCredentialByCertCredentialTest()
        {
            var RgbHashOfCert = Enumerable.Range(0, 20).Select( i => (byte)i).ToArray();
            System.Diagnostics.Debug.WriteLine($"{nameof(RgbHashOfCert)}:[{string.Join(" ",RgbHashOfCert.Select(b => $"{b:X2}"))}]");
            var MarshalCredential = CredentialsManager.MarshalCredentialByCertCredential(RgbHashOfCert);
            System.Diagnostics.Debug.WriteLine($"{nameof(MarshalCredential)}:{MarshalCredential}");
            using (var getter = CredentialsManager.UnmarshalCredential(MarshalCredential))
                if (getter.Value is CertCredentialInfo info)
                    CollectionAssert.AreEqual(info.RgbHashOfCert, RgbHashOfCert);
                else
                    Assert.Fail($"{getter.Value} is not {nameof(CertCredentialInfo)}");
        }
        [TestMethod()]
        public void MarshalCredentialByUsernameTest()
        {
            var UserName = "TESTUSER";
            System.Diagnostics.Debug.WriteLine($"{nameof(UserName)}:{UserName}");
            var MarshalCredential = CredentialsManager.MarshalCredentialByUsername(UserName);
            System.Diagnostics.Debug.WriteLine($"{nameof(MarshalCredential)}:{MarshalCredential}");
            using (var getter = CredentialsManager.UnmarshalCredential(MarshalCredential))
                if (getter.Value is UsernameTargetCredentialInfo info)
                    Assert.AreEqual(info.UserName, UserName);
                else
                    Assert.Fail($"{getter.Value} is not {nameof(UsernameTargetCredentialInfo)}");
        }
    }
}
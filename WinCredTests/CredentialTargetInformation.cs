using Microsoft.VisualStudio.TestTools.UnitTesting;
using Advapi32.WinCred;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Advapi32.WinCred.Tests
{
    [TestClass()]
    public class CredentialTargetInformationTest
    {
        [TestMethod()]
        public void GetTargetInfoTest()
        {
            try
            {
                var info = CredentialTargetInformation.GetTargetInfo("TEST", CredGetTargetInfoFlags.AllowNameResolution);
                foreach (var Credential in info.ReadDomainCredentials())
                    System.Diagnostics.Debug.WriteLine(Credential);
            }catch(Exception e)
            {
                Assert.Fail(e.ToString());
            }
        }
        [TestMethod()]
        public void WriteAndReadAndDeleteTest()
        {
            var TargetName = "TESTTARGET";
            { 
                var Credential = new Credential();
                Credential.LastWritten = DateTime.Now;
                Credential.Persist = CredPersist.LocalMachine;
                Credential.Flags = 0;
                Credential.Type = CredType.Generic;
                Credential.TargetName = TargetName;
                Credential.UserName = "TESTUSER";
                Credential.Password = "TESTPASSWORD";
                Credential.TargetAlias = "TESTALIAS";
                Credential.Comment = "TESTCOMMENT 🌸";
                Credential.Write();
            }
            {
                var Credential =  WinCred.Credential.Read(TargetName, CredType.Generic);
                Assert.IsNotNull(Credential);
                Credential.Write();
                Credential.Delete();
            }
            {
                try
                {
                    var Credential = WinCred.Credential.Read(TargetName, CredType.Generic);
                    Assert.Fail("not delete.");
                }catch(Exception e)
                {
                    Assert.AreEqual(unchecked((uint)e.HResult), 0x80070490,$"ELEMENT_NOT_FOUND ? -> 0x{unchecked((uint)e.HResult):X8}");
                }
            }
        }
    }
}
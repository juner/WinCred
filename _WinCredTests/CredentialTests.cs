using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Advapi32.WinCred.Tests
{
    [TestClass()]
    public class CredentialTest
    {
        [TestMethod()]
        public void EnumerateTest()
        {
            try
            {
                foreach (var Credential in Credential.Enumerate())
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
                var Credential = new Credential
                {
                    LastWritten = DateTime.Now,
                    Persist = CredPersist.LocalMachine,
                    Flags = 0,
                    Type = CredType.Generic,
                    TargetName = TargetName,
                    UserName = "TESTUSER",
                    Password = "TESTPASSWORD",
                    TargetAlias = "TESTALIAS",
                    Comment = "TESTCOMMENT 🌸"
                };
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
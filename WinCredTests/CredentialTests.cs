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
    public class CredentialTests
    {
        [TestMethod()]
        public void EnumerateTest()
        {
            foreach (var Credential in Credential.Enumerate())
                System.Diagnostics.Debug.WriteLine(Credential);
        }
    }
}
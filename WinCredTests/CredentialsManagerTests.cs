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
    }
}
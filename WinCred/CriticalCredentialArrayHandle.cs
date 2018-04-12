using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Advapi32.WinCred
{
    internal class CriticalCredentialArrayHandle : CriticalHandleZeroOrMinusOneIsInvalid
    {
        public int Count { get; } = 0;
        public CriticalCredentialArrayHandle(int Count, IntPtr preexistingHandle)
        {
            this.Count = Count;
            SetHandle(preexistingHandle);
        }
        public IEnumerable<Credential> GetCredentials()
        {
            if (IsInvalid)
                throw new InvalidOperationException("Invalid CriticalHandle");
            var Size = Marshal.SizeOf(typeof(IntPtr));
            foreach (var Credential in Enumerable.Range(0, Count)
                             .Select(n => Marshal.ReadIntPtr(handle, n * Size))
                             .Select(n => UnmanagedCredential.From(n).ToManaged()))
                yield return Credential;
        }
        protected override bool ReleaseHandle()
        {
            if (IsInvalid)
                return false;
            var result = Interop.CredFree(handle);
            SetHandleAsInvalid();
            return result;
        }
    }
}

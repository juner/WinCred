using Microsoft.Win32.SafeHandles;
using System;

namespace Advapi32.WinCred
{
    internal class CriticalCredentialHandle : CriticalHandleZeroOrMinusOneIsInvalid
    {
        public CriticalCredentialHandle(IntPtr preexistingHandle)
        {
            SetHandle(preexistingHandle);
        }
        public Credential GetCredential()
        {
            if (IsInvalid)
                throw new InvalidOperationException("Invalid CriticalHandle");
            return UnmanagedCredential.From(handle).ToManaged();
        }
        protected override bool ReleaseHandle()
        {
            if (IsInvalid)
                return false;
            Interop.CredFree(handle);
            SetHandleAsInvalid();
            return true;
        }
    }
}

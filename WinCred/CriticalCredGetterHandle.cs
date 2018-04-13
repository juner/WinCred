using Microsoft.Win32.SafeHandles;
using System;

namespace Advapi32.WinCred
{
    public class CriticalCredGetterHandle<T> : CriticalHandleZeroOrMinusOneIsInvalid, ICredGetterHandle<T>
    {
        private Func<IntPtr, T> Action;
        public CriticalCredGetterHandle(IntPtr preexistingHandle, Func<IntPtr, T> Action)
        {
            if (Action == null)
                throw new ArgumentNullException(nameof(Action));
            SetHandle(preexistingHandle);
            this.Action = Action;
        }
        public T Value
        {
            get
            {
                if (IsInvalid)
                    throw new InvalidOperationException(" Invalid Handle.");
                return Action(handle);
            }
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

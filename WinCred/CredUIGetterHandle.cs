using Microsoft.Win32.SafeHandles;
using System;
namespace Credui.WinCred
{
    internal class CredUIGetterHandle<T> : CriticalHandleZeroOrMinusOneIsInvalid, ICredUIGetter<T>
    {
        Func<IntPtr, T> Action;

        public CredUIGetterHandle(IntPtr handle, Func<IntPtr, T> Action)
        {
            if (Action == null)
                throw new ArgumentNullException(nameof(Action));
            this.Action = Action;
            SetHandle(handle);
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
            Kernel32.Interop.LocalFree(handle);
            SetHandleAsInvalid();
            return true;
        }
    }
}

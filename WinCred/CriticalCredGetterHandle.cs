using Microsoft.Win32.SafeHandles;
using System;

namespace Advapi32.WinCred
{
    public class CriticalCredGetterHandle<T> : CriticalHandleZeroOrMinusOneIsInvalid, IDisposableGetter<T>
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
    public class UnmanagedDisposableGetter<T1,T2> : IDisposableGetter<T1>
        where T1 : struct,Unmanaged.IUnmanaged<T2>
        where T2 : class
    {
        IDisposable Disposable;
        public UnmanagedDisposableGetter(IDisposable Disposable, T1 Value) => (this.Disposable, this.Value) = (Disposable, Value);
        public T1 Value { get; }

        public void Dispose()
        {
            Disposable?.Dispose();
        }
        public static UnmanagedDisposableGetter<T1,T2> From(T2 Managed)
        {
            var Unmanaged = new T1();
            return new UnmanagedDisposableGetter<T1,T2>(Unmanaged.Copy(Managed), Unmanaged);
        }
    }
}

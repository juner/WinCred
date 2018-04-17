using System;

namespace Advapi32.WinCred
{
    public interface IDisposableGetter<T> : IDisposable
    {
        T Value { get; }
    }
}

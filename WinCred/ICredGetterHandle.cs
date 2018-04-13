using System;

namespace Advapi32.WinCred
{
    public interface ICredGetterHandle<T> : IDisposable
    {
        T Value { get; }
    }
}

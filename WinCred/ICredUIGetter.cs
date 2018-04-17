using System;
namespace Credui.WinCred
{
    public interface ICredUIGetter<T> : IDisposable
    {
        T Value { get; }
    }
}

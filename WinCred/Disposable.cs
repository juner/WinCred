using System;

namespace Advapi32.WinCred
{
    internal class Disposable : IDisposable
    {
        static Disposable _Noop = null;
        Action Action;
        Disposable(Action action) => Action = action;
        public static Disposable Noop() => _Noop ?? (_Noop = new Disposable(null));
        public static Disposable Create(Action Action) => new Disposable(Action);

        #region IDisposable Support
        public void Dispose()
        {
            try
            {
                Action?.Invoke();
            }
            catch{ }
            Action = null;
        }
        #endregion
    }
}

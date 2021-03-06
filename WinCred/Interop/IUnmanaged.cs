﻿using System;

namespace Advapi32.WinCred.Unmanaged
{
    public interface IUnmanaged<T> where T : class
    {
        T ToManaged();
        IDisposable Copy(T Info);
    }
}

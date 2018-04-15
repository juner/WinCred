using System;

namespace Advapi32.WinCred
{
    public interface ICredMarshal
    {
        CredMarshalType Type { get; }
        IntPtr Credential { get; }
    }
}

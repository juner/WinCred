using System;
namespace Advapi32.WinCred
{
    [Flags]
    public enum CredTiFlags
    {
        ServerFormatUnknown = 1,
        DomainFormatUnknown = 2,
        OnlyPasswordRequired = 4,
    }
}

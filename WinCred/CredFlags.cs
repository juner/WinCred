using System;

namespace Advapi32.WinCred
{
    [Flags]
    public enum CredFlags : uint
    {
        PromptNow = 0x2,
        UsernameTarget = 0x4
    }
}

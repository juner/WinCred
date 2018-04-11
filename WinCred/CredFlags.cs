using System;

namespace Advapi32.WinCred
{
    [Flags]
    public enum CredFlags : uint
    {
        PROMPT_NOW = 0x2,
        USERNAME_TARGET = 0x4
    }
}

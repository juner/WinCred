namespace Advapi32.WinCred
{
    public enum CredProtectionType : uint
    {
        CredUnprotected = 0,
        CredUserProtection = 1,
        CredTrustedProtection = 2,
    }
}

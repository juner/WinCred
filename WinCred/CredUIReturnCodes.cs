namespace Advapi32.WinCred
{
    public enum CredUIReturnCodes : uint
    {
        NoError = 0,
        ErrorCancelled = 1223,
        ErrorNoSuchLogonSession = 1312,
        ErrorNotFound = 1168,
        ErrorInvalidAccountName = 1315,
        ErrorInsufficientBuffer = 122,
        ErrorInvalidParameter = 87,
        ErrorInvalidFlags = 1004,
    }
}

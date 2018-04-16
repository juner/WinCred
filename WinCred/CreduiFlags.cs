using System;

namespace Credui.WinCred
{
    [Flags]
    public enum CreduiFlags : uint
    {
        IncorrectPassword = 0x00001,
        DoNotPersist = 0x00002,
        RequestAdministrator = 0x00004,
        AlwaysShowUI = 0x00080,
        ExcludeCertificates = 0x00008,
        RequireCertificate = 0x00010,
        ShowSaveCheckBox = 0x00040,
        CompleteUsername = 0x00800,
        RequireSmartcard = 0x00100,
        PasswordOnlyOk = 0x00200,
        ValidateUsername = 0x00400,
        Persist = 0x01000,
        ServerCredential = 0x04000,
        KeepUsername = 0x100000,
        ExpectConfirmation = 0x20000,
        GenericCredentials = 0x40000,
        UsernameTargetCredentials = 0x80000,

    }
}

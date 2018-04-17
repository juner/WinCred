using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Advapi32.WinCred
{
    internal static class Interop
    {
        [DllImport("advapi32", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool CredEnumerate(string filter, CredEnumerateFlags flag, out int count, out IntPtr pCredentials);
        [DllImport("advapi32", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool CredRead(string targetName, CredType type, CredReadFlags flags, out IntPtr pCredential);
        [DllImport("advapi32", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool CredGetSessionTypes(CredType MaximumPersistCount, out IntPtr MaximumPersist);
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool CredMarshalCredential(CredMarshalType credType, IntPtr credential, out IntPtr marshaledCredential);
        [DllImport("advapi32", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool CredUnmarshalCredential(string marshaledCredential, out CredMarshalType credType, out IntPtr credential);
        [DllImport("advapi32", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool CredIsMarshaledCredential(string MarshaledCredential);
        [DllImport("advapi32", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool CredWrite(ref Unmanaged.Credential credential, CredWriteFlags flags);
        [DllImport("advapi32", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool CredDelete(string targetName, CredType type, CredDeleteFlags flags);
        //[DllImport("advapi32", SetLastError = true, CharSet = CharSet.Unicode)]
        //public static extern bool CredRename(string OldTargetName, string NewTargetName, CredType Type, CredRenameFlags Flags);
        [DllImport("advapi32", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool CredProtect(bool AsSelf, StringBuilder Credentials, uint CredentialsSize, StringBuilder ProtectedCredentials, out uint ProtectedCredentialsSize, out CredProtectionType ProtectionType);
        [DllImport("advapi32", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool CredUnprotect(bool AsSelf, StringBuilder ProtectedCredentials, uint ProtectedCredentialsSize, StringBuilder Credentials, out uint CredentialsSize, ref uint MaxChars);
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool CredFree(IntPtr buffer);
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool CredGetTargetInfo(string TargetName, CredGetTargetInfoFlags Flags, out IntPtr TargetInfo);
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool CredReadDomainCredentials(ref Unmanaged.CredentialTargetInformation TargetInfo, CredReadDomainCredentialsFlags Flags, out uint Count, out IntPtr Credentials);
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool CredWriteDomainCredentials(ref Unmanaged.CredentialTargetInformation TargetInfo, ref Unmanaged.Credential Credential, CredWriteDomainCredentialsFlag Flags);
    }
}
namespace Credui.WinCred
{
    internal static class Interop
    {
        [DllImport("credui.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern CredUIReturnCodes CredUIPromptForWindowsCredentials(in CreduiInfo CredInfo, int AuthError, ref uint AuthPackage, IntPtr InAuthBuffer, uint InAuthBufferSize, out IntPtr OutAuthBuffer, out uint OutAuthBufferSize, ref bool Save, CredUiWinFlags Flags);
        [DllImport("credui.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool CredUIParseUserName(string UserName, out string User, uint UserMaxChars, out string Domain, uint DomainMaxChars);
        [DllImport("credui.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern CredUIReturnCodes CredUICmdLinePromptForCredentials(string TargetName, IntPtr Reserved, int AuthError, StringBuilder UserName, uint UserNameMaxChars, StringBuilder Password, uint PasswordMaxChars, ref bool Save, CreduiFlags Flags);
        [DllImport("credui.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern CredUIReturnCodes CredUIStoreSSOCred(string Realm, string UserName, string Password, bool Persist);
        [DllImport("credui.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern CredUIReturnCodes CredUIReadSSOCred(string Realm, out IntPtr UserName);
        [DllImport("credui.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool CredUnPackAuthenticationBuffer(CredPackFlags Flags, IntPtr AuthBuffer, uint AuthBufferSize, StringBuilder UserName, ref uint MaxUserName, StringBuilder DomainName, ref uint MaxDomainname, StringBuilder Password, ref uint MaxPassword);
        [DllImport("credui.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern Boolean CredPackAuthenticationBuffer(CredPackFlags Flags, string UserName, string Password, IntPtr PackedCredentials, ref uint PackedCredentialsSize);
    }
    public enum CredPackFlags : uint
    {
        ProtectedCredentials = 0x1,
        WowBuffer = 0x2,
        GenericCredentials = 0x4,
    }
}
namespace Kernel32
{
    internal static class Interop
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr LocalFree(IntPtr hMem);
    }
}

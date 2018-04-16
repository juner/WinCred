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
        [DllImport("advapi32", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool CredRename(string OldTargetName, string NewTargetName, CredType Type, CredRenameFlags Flags);
        [DllImport("advapi32", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool CredProtect(bool AsSelf, StringBuilder Credentials, uint CredentialsSize, StringBuilder ProtectedCredentials, out uint ProtectedCredentialsSize, out CredProtectionType ProtectionType);
        [DllImport("advapi32", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool CredUnprotect(bool AsSelf, StringBuilder ProtectedCredentials, uint ProtectedCredentialsSize, StringBuilder Credentials, out uint CredentialsSize, ref uint MaxChars);
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool CredFree(IntPtr buffer);
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool CredRenam(string OldTargetName, string NewTargetName, CredType Type, CredFlags Flags);
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
        [DllImport("credui.dll", CharSet = CharSet.Unicode)]
        public static extern CredUIReturnCodes CredUIPromptForCredentials(ref CreduiInfo UiInfo, string targetName, IntPtr Reserved, int AuthError, ref string UserName, int MaxUserName, ref string Password, int MaxPassword, [MarshalAs(UnmanagedType.Bool)] ref bool Save, CreduiFlags Flags);
        [DllImport("credui.dll", CharSet = CharSet.Unicode)]
        public static extern bool CredUIParseUserName(string UserName, out string User, uint UserMaxChars, out string Domain, uint DomainMaxChars);
        [DllImport("credui.dll", CharSet = CharSet.Unicode)]
        public static extern CredUIReturnCodes CredUICmdLinePromptForCredentials(string TargetName, IntPtr Reserved, uint AuthError, ref string UserName, uint UserNameMaxChars, ref string Password, uint PasswordMaxChars, ref bool Save, uint Flags);
    }
}

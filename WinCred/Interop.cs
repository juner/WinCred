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
        public static extern bool CredProtect(bool AsSelf, StringBuilder Credentials, uint CredentialsSize, StringBuilder ProtectedCredentials, out uint ProtectedCredentialsSize, out CredProtectionType ProtectionType);
        [DllImport("advapi32", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool CredUnprotect(bool AsSelf, StringBuilder ProtectedCredentials, uint ProtectedCredentialsSize, StringBuilder Credentials, out uint CredentialsSize, ref uint MaxChars);
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool CredFree(IntPtr buffer);
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool CredRenam(string OldTargetName, string NewTargetName, CredType Type, CredFlags Flags);
        [DllImport("credui.dll", CharSet = CharSet.Unicode)]
        private static extern CredUIReturnCodes CredUIPromptForCredentials(ref CreduiInfo creditUR,
            string targetName,
            IntPtr reserved1,
            int iError,
            ref string userName,
            int maxUserName,
            ref string password,
            int maxPassword,
            [MarshalAs(UnmanagedType.Bool)] ref bool pfSave,
            CreduiFlags flags);
    }
}

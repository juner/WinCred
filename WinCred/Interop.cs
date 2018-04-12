using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Advapi32.WinCred
{
    internal static class Interop
    {
        [DllImport("kernel32.dll")]
        public static extern uint FormatMessage(uint dwFlags, IntPtr lpSource, uint dwMessageId, uint dwLanguageId, StringBuilder lpBuffer, int nSize, IntPtr Arguments);

        public static string GetErrorMessage()
        {
            const uint FORMAT_MESSAGE_FROM_SYSTEM = 0x00001000;
            var win32ErrorCode = Marshal.GetLastWin32Error();
            var message = new StringBuilder(255);
            FormatMessage(FORMAT_MESSAGE_FROM_SYSTEM, IntPtr.Zero, (uint)win32ErrorCode, 0, message, message.Capacity, IntPtr.Zero);

            return message.ToString();
        }
        [DllImport("advapi32", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool CredEnumerate(string filter, CredFlags flag, out int count, out IntPtr pCredentials);
        [DllImport("advapi32", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool CredRead(string targetName, CredType type, CredFlags flags, out IntPtr pCredential);
        [DllImport("advapi32", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool CredWrite(ref UnmanagedCredential credential, CredFlags flags);
        [DllImport("advapi32", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool CredDelete(string targetName, CredType type, CredFlags flags);
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool CredFree(IntPtr buffer);
    }
}

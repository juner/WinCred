using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Advapi32.WinCred
{
    /// <summary>
    /// マネージドコードに置き換えたCredential
    /// </summary>
    public class Credential
    {
        internal const int CRED_MAX_VALUE_SIZE = 256;
        public CredFlags Flags { get; set; }
        public CredType Type { get; set; }
        public string TargetName { get; set; }
        public string Comment { get; set; }
        public DateTime LastWritten { get; set; }
        public byte[] CredentialBlob { get; set; }
        public CredPersist Persist { get; set; }
        public CredentialAttribute[] Attributes { get; set; }
        public string TargetAlias { get; set; }
        public string UserName { get; set; }
        
        public string Password
        {
            get => CredentialBlob?.Length > 0
                ? Encoding.Unicode.GetString(CredentialBlob)
                : null;
            set => CredentialBlob = string.IsNullOrEmpty(value)
                        ? null
                        : Encoding.Unicode.GetBytes(value);
        }

        /// <summary>
        /// 資格情報の列挙
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Credential> Enumerate(string Filter = null, CredFlags CredFlags = default(CredFlags))
            => Unmanaged.Credential.Enumerate(Filter, CredFlags).Using().SelectMany(h => h.Value).Select(uc => uc.ToManaged());
        /// <summary>
        /// 資格情報の読込
        /// </summary>
        /// <param name="TargetName"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        public static Credential Read(string TargetName, CredType Type)
            => Unmanaged.Credential.Read(TargetName, Type).Using(c => c.Value.ToManaged());
        /// <summary>
        /// 資格情報の登録
        /// </summary>
        public void Write() => Write(this, Flags);

        /// <summary>
        /// 資格情報の登録
        /// </summary>
        /// <param name="Credential"></param>
        /// <param name="flags"></param>
        public static void Write(Credential Credential, CredFlags Flags)
            => Unmanaged.Credential.Write(Credential, Flags);

        public void Delete() => Delete(TargetName, Type, Flags);
        /// <summary>
        /// 資格情報の削除
        /// </summary>
        /// <param name="TargetName"></param>
        /// <param name="Type"></param>
        /// <param name="Flags"></param>
        private static void Delete(string TargetName, CredType Type, CredFlags Flags)
            => Unmanaged.Credential.Delete(TargetName, Type, Flags);
        public static string MarshalCredentialAtCertCredential(byte[] RgbHashOfCert)
        {
            var certInfo = new CertCredentialInfo
            {
                cbSize = (uint)Marshal.SizeOf(nameof(CertCredentialInfo)),
                rgbHashOfCert = RgbHashOfCert,
            };
            int size = Marshal.SizeOf(certInfo);

            var Ptr = Marshal.AllocCoTaskMem(size);
            try
            {
                Marshal.StructureToPtr(certInfo, Ptr, false);
                return MarshalCredential(CredMarshalType.CertCredential, Ptr);
            }
            finally
            {
                Marshal.FreeCoTaskMem(Ptr);
            }
        }
        public static string MarshalCredentialAtUsername(string UsernameTarget)
        {
            var ut = new UsernameTargetCredentialInfo
            {
                UserName = UsernameTarget,
            };
            var Ptr = Marshal.AllocCoTaskMem(Marshal.SizeOf(ut));
            try
            {
                Marshal.StructureToPtr(ut, Ptr, false);
                return MarshalCredential(CredMarshalType.UsernameTargetCredential, Ptr);
            }
            finally
            {
                Marshal.FreeCoTaskMem(Ptr);
            }
        }
        /// <summary>
        /// 信任状を文字列に変換する
        /// </summary>
        /// <param name="CredType"></param>
        /// <param name="CredInforgbHashOfCert"></param>
        /// <param name="MarshaledCredential"></param>
        /// <returns></returns>
        public static void MarshalCredential(CredMarshalType CredType,IntPtr Credential, out string MarshaledCredential)
        {
            MarshaledCredential = null;
            var _marshaledCredential = IntPtr.Zero;
            try
            {
                bool result = false;
                if (result = Interop.CredMarshalCredential(CredType,
                                    Credential,
                                    out _marshaledCredential))
                {
                    MarshaledCredential = Marshal.PtrToStringUni(_marshaledCredential);
                    return;
                }
                else
                    throw new Exception(Interop.GetErrorMessage());
            }
            finally
            {
                if (_marshaledCredential != IntPtr.Zero)
                    Interop.CredFree(_marshaledCredential);
            }
        }
        public static string MarshalCredential(CredMarshalType CredType, IntPtr Credential)
        {
            MarshalCredential(CredType, Credential, out var _MarshalCredential);
            return _MarshalCredential;
        }
        public static void UnmarshalCredential(string MarshaledCredential, CredMarshalType CredType, IntPtr Credential)
        {

        }
        public static bool IsMarshalCredential(string marshaledCredential) => Interop.CredIsMarshaledCredential(marshaledCredential);
        public override string ToString()
            => $"{nameof(Credential)}{{"
            + $"{nameof(Flags)}: {Flags}"
            + $", {nameof(Type)}: {Type}"
            + $", {nameof(TargetName)}: {TargetName}"
            + $", {nameof(Comment)}: {Comment}"
            + $", {nameof(LastWritten)}: {LastWritten}"
            + $", {nameof(CredentialBlob)}: [{string.Join(" ", CredentialBlob?.Select(b => $"{b:X2}") ?? Enumerable.Empty<string>())}]"
            + $", {nameof(Password)}: {Password}"
            + $", {nameof(Persist)}: {Persist}"
            + $", {nameof(Attributes)}: [{string.Join(", ", Attributes?.Select(a => $"{a}") ?? Enumerable.Empty<string>())}]"
            + $", {nameof(TargetAlias)}: {TargetAlias}"
            + $", {nameof(UserName)}: {UserName}"
            + $"}}";
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct UsernameTargetCredentialInfo
    {
        public string UserName; 
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct BinaryBlobCredntialInfo
    {
        public uint cbBlob;
        public IntPtr pbBlob;
    }
}

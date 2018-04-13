using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;

namespace Advapi32.WinCred
{
    public static class CredentialsManager
    {
        public static Dictionary<CredType,CredPersist> GetSessionTypes(CredType Type = CredType.Maximum)
        {
            if(Interop.CredGetSessionTypes(Type,out var Persists))
                return (Persists == IntPtr.Zero ? Enumerable.Empty<int>() : Enumerable.Range(0,(int)(uint)Type))
                    .Select(i => Marshal.ReadIntPtr(Persists,i * sizeof(CredPersist)))
                    .Select(i => (CredPersist)Marshal.PtrToStructure(i,typeof(CredPersist)))
                    .Select((p, i) => (Value: p, Key: (CredType)i))
                    .ToDictionary(v => v.Key, v => v.Value) ;
            throw Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
        }
        public static string MarshalCredentialAtCertCredential(byte[] RgbHashOfCert)
        {
            if (RgbHashOfCert == null)
                throw new ArgumentNullException(nameof(RgbHashOfCert));
            if (RgbHashOfCert?.Length != 20)
                throw new ArgumentException("Length は 20 の必要があります。", nameof(RgbHashOfCert));
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
        public static string MarshalCredential(CredMarshalType CredType, IntPtr Credential)
        {
            if (Interop.CredMarshalCredential(CredType, Credential, out var MarshaledCredential))
                using (var getter = new CriticalCredGetterHandle<string>(MarshaledCredential, Marshal.PtrToStringUni))
                    return getter.Value;
            var hresult = Marshal.GetHRForLastWin32Error();
            var exception = Marshal.GetExceptionForHR(hresult);
            if (unchecked((uint)hresult) == 0x80070032)
                throw new NotSupportedException("not support.", exception);
            throw exception;
        }
        public static ICredGetterHandle<CredMarshalObject> UnmarshalCredential(string MarshaledCredential)
        {
            if (Interop.CredUnmarshalCredential(MarshaledCredential, out var CredType, out var Crednetial))
                return new CriticalCredGetterHandle<CredMarshalObject>(Crednetial, c => new CredMarshalObject(CredType, c));
            var hresult = Marshal.GetHRForLastWin32Error();
            var exception = Marshal.GetExceptionForHR(hresult);
            if (unchecked((uint)hresult) == 0x80070032)
                throw new NotSupportedException("not support.", exception);
            throw exception;
        }
        public class CredMarshalObject
        {
            public readonly CredMarshalType Type;
            public readonly byte[] RgbHashOfCert = null;
            public readonly string UserName = null;
            public CredMarshalObject(CredMarshalType Type, IntPtr Credential)
            {
                this.Type = Type;
                switch (Type)
                {
                    case CredMarshalType.CertCredential:
                        var CredentialInfo = Marshal.PtrToStructure<CertCredentialInfo>(Credential);
                        RgbHashOfCert = CredentialInfo.rgbHashOfCert;
                        break;
                    case CredMarshalType.UsernameTargetCredential:
                        var UsernameTargetCredentialInfo = Marshal.PtrToStructure<UsernameTargetCredentialInfo>(Credential);
                        UserName = UsernameTargetCredentialInfo.UserName;
                        break;
                    default:
                        System.Diagnostics.Debug.WriteLine($"{Type} is not support.");
                        break;
                }
            }
        }
        public static bool IsMarshalCredential(string marshaledCredential) => Interop.CredIsMarshaledCredential(marshaledCredential);
        public static void UIStoreSSOCred(string UserName, string Password, bool Persist) => UIStoreSSOCred(null, UserName, Password, Persist);
        public static void UIStoreSSOCred(string Realm, string UserName, string Password, bool Persist)
        {
            if (string.IsNullOrEmpty(UserName))
                throw new ArgumentNullException(nameof(UserName));
            if (string.IsNullOrEmpty(Password))
                throw new ArgumentNullException(nameof(Password));
            throw new NotImplementedException();
        }
    }
}

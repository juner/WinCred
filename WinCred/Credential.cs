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
        {
            var count = 0;
            var pCredentials = IntPtr.Zero;

            //CredEnumerate呼び出し
            if (!Interop.CredEnumerate(Filter, CredFlags, out count, out pCredentials))
            {
                throw new ApplicationException(Interop.GetErrorMessage());
            }
            using (var handle = new CriticalCredentialArrayHandle(count, pCredentials))
                foreach (var credential in handle.GetCredentials())
                    yield return credential;
        }
        /// <summary>
        /// 資格情報の読み込み
        /// </summary>
        /// <param name="targetName"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Credential Read(string targetName, CredType type)
        {
            if (Interop.CredRead(targetName, type, 0, out var credentialPtr))
                using (var credential = new CriticalCredentialHandle(credentialPtr))
                    return credential.GetCredential();
            throw new ApplicationException(Interop.GetErrorMessage());
        }
        public void Write() => Write(this, Flags);

        /// <summary>
        /// 資格情報の登録
        /// </summary>
        /// <param name="managedCred"></param>
        /// <param name="flags"></param>
        public static void Write(Credential managedCred, CredFlags flags)
        {
            new UnmanagedCredential(managedCred).Write(flags);
        }

        public void Delete() => Delete(TargetName, Type, Flags);
        /// <summary>
        /// 資格情報の削除
        /// </summary>
        /// <param name="TargetName"></param>
        /// <param name="Type"></param>
        /// <param name="Flags"></param>
        private static void Delete(string TargetName, CredType Type, CredFlags Flags)
        {
            if (!Interop.CredDelete(TargetName, Type, Flags))
                throw new ApplicationException(Interop.GetErrorMessage());
        }
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
}

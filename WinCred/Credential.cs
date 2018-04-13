using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public static IEnumerable<Credential> Enumerate(string Filter = null, CredEnumerateFlags CredFlags = default(CredEnumerateFlags))
            => Unmanaged.Credential.Enumerate(Filter, CredFlags).Using().SelectMany(h => h.Value).Select(uc => uc.ToManaged());
        /// <summary>
        /// 資格情報の読込
        /// </summary>
        /// <param name="TargetName"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        public static Credential Read(string TargetName, CredType Type = default(CredType), CredReadFlags Flags = default(CredReadFlags))
            => Unmanaged.Credential.Read(TargetName, Type, Flags).Using(c => c.Value.ToManaged());
        /// <summary>
        /// 資格情報の登録
        /// </summary>
        public void Write(CredWriteFlags Flags = default(CredWriteFlags)) => Write(this);

        /// <summary>
        /// 資格情報の登録
        /// </summary>
        /// <param name="Credential"></param>
        /// <param name="flags"></param>
        public static void Write(Credential Credential, CredWriteFlags Flags = default(CredWriteFlags))
            => Unmanaged.Credential.Write(Credential, Flags);

        public void Delete(CredDeleteFlags Flags = default(CredDeleteFlags)) => Delete(TargetName, Type, Flags);
        /// <summary>
        /// 資格情報の削除
        /// </summary>
        /// <param name="TargetName"></param>
        /// <param name="Type"></param>
        /// <param name="Flags"></param>
        public static void Delete(string TargetName, CredType Type, CredDeleteFlags Flags = default(CredDeleteFlags))
            => Unmanaged.Credential.Delete(TargetName, Type, Flags);
        /// <summary>
        /// 資格情報名の変更
        /// </summary>
        /// <param name="NewTargetName"></param>
        /// <param name="Type"></param>
        /// <param name="Flags"></param>
        public void Rename(string NewTargetName, CredType Type,CredRenameFlags Flags = default(CredRenameFlags))
        {
            Rename(TargetName, NewTargetName, Type, Flags);
            TargetName = NewTargetName;
        }
        /// <summary>
        /// 資格情報名の変更
        /// </summary>
        /// <param name="OldTargetName"></param>
        /// <param name="NewTargetName"></param>
        /// <param name="Type"></param>
        /// <param name="Flags"></param>
        public static void Rename(string OldTargetName, string NewTargetName, CredType Type, CredRenameFlags Flags = default(CredRenameFlags))
            => Unmanaged.Credential.Rename(OldTargetName, NewTargetName, Type, Flags);
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

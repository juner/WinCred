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
            get
            {
                return CredentialBlob.Length > 0
                    ? Encoding.UTF8.GetString(CredentialBlob)
                    : string.Empty;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    CredentialBlob = Encoding.UTF8.GetBytes(value);
            }
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
                Console.WriteLine(Interop.GetErrorMessage());
                throw new ApplicationException("資格情報の列挙に失敗しました。");
            }

            foreach (var Credential in Enumerable.Range(0, count)
                             .Select(n => Marshal.ReadIntPtr(pCredentials, n * Marshal.SizeOf(typeof(IntPtr))))
                             .Select(ptr => UnmanagedCredential.FromPtr(ptr).ToCredential()))
                yield return Credential;
        }
        /// <summary>
        /// 資格情報の読み込み
        /// </summary>
        /// <param name="targetName"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Credential Read(string targetName, CredType type)
        {
            var credential = IntPtr.Zero;

            if (!Interop.CredRead(targetName, type, 0, out credential))
            {
                Console.WriteLine(Interop.GetErrorMessage());
                throw new ApplicationException("資格情報の取得に失敗しました。");
            }

            return UnmanagedCredential.FromPtr(credential).ToCredential();
        }
        public void Write() => Write(this, Flags);

        /// <summary>
        /// 資格情報の登録
        /// </summary>
        /// <param name="managedCred"></param>
        /// <param name="flags"></param>
        public static void Write(Credential managedCred, CredFlags flags)
        {
            Write(new UnmanagedCredential(managedCred), flags);
        }

        /// <summary>
        /// 資格情報の登録
        /// </summary>
        /// <param name="unmanagedCred"></param>
        /// <param name="flags"></param>
        private static void Write(UnmanagedCredential unmanagedCred, CredFlags flags)
        {
            if (!Interop.CredWrite(ref unmanagedCred, flags))
            {
                Console.WriteLine(Interop.GetErrorMessage());
                throw new ApplicationException("資格情報の書き込みに失敗しました。");
            }

            Console.WriteLine("ok");
        }
        public void Delete() => Delete(TargetName, Type, Flags);
        /// <summary>
        /// 資格情報の削除
        /// </summary>
        /// <param name="targetName"></param>
        /// <param name="type"></param>
        /// <param name="flags"></param>
        private static void Delete(string targetName, CredType type, CredFlags flags)
        {
            if (!Interop.CredDelete(targetName, type, flags))
            {
                Console.WriteLine(Interop.GetErrorMessage());
                throw new ApplicationException("資格情報の削除に失敗しました。");
            }
        }
        public override string ToString()
            => $"{nameof(Credential)}{{"
            + $"{nameof(Flags)}: {Flags}"
            + $", {nameof(Type)}: {Type}"
            + $", {nameof(TargetName)}: {TargetName}"
            + $", {nameof(Comment)}: {Comment}"
            + $", {nameof(LastWritten)}: {LastWritten}"
            + $", {nameof(CredentialBlob)}: [{string.Join(" ", CredentialBlob?.Select(b => $"{b:X2}") ?? Enumerable.Empty<string>())}]"
            + $", {nameof(Persist)}: {Persist}"
            + $", {nameof(Attributes)}: [{string.Join(", ",Attributes?.Select(a => $"{a}") ?? Enumerable.Empty<string>())}]"
            + $", {nameof(TargetAlias)}: {TargetAlias}"
            + $", {nameof(UserName)}: {UserName}"
            + $"}}";
    }
}

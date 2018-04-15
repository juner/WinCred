using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;

namespace Advapi32.WinCred.Unmanaged
{
    /// <summary>
    /// アンマネージドなCredential
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct Credential : IUnmanaged<WinCred.Credential>
    {
        public CredFlags Flags;
        public CredType Type;
        public string TargetName;
        public string Comment;
        public System.Runtime.InteropServices.ComTypes.FILETIME LastWritten;
        private uint CredentialBlobSize;
        private IntPtr CredentialBlobPtr;
        public byte[] CredentialBlob
        {
            get
            {
                System.Diagnostics.Debug.Assert(CredentialBlobPtr == IntPtr.Zero ? CredentialBlobSize == 0 : CredentialBlobSize >= 0, $"{nameof(CredentialBlobSize)}({CredentialBlobSize})だけ初期化されて、{nameof(CredentialBlobPtr)}(0x{CredentialBlobPtr:X})が初期化されていない。");
                if (CredentialBlobPtr == IntPtr.Zero)
                    return null;
                var bytes = new byte[CredentialBlobSize];
                if (CredentialBlobSize > 0)
                    Marshal.Copy(CredentialBlobPtr, bytes, 0, (int)CredentialBlobSize);
                return bytes;
            }
        }
        public CredPersist Persist;
        private uint AttributeCount;
        private IntPtr AttributesPtr;
        public CredentialAttribute[] Attributes{
            get
            {
                System.Diagnostics.Debug.Assert(AttributesPtr == IntPtr.Zero ? AttributeCount == 0 : AttributeCount >= 0, $"{nameof(AttributeCount)}({AttributeCount})だけ設定され、{nameof(AttributesPtr)}(0x{AttributesPtr:X})が未設定。");
                if (AttributeCount <= 0)
                    return null;
                var self = this;
                var Size = Marshal.SizeOf(typeof(CredentialAttribute));
                var list = new CredentialAttribute[AttributeCount];
                return Enumerable
                    .Range(0, (int)AttributeCount - 1)
                    .Select(x => Marshal.ReadIntPtr(self.AttributesPtr, x * Size))
                    .Select(x => CredentialAttribute.From(x))
                    .ToArray();
            }
        }
        public string TargetAlias;
        public string UserName;
        public static ICredGetterHandle<IEnumerable<Credential>> Enumerate(string Filter = null, CredEnumerateFlags CredFlags = default(CredEnumerateFlags))
        {
            var Size = Marshal.SizeOf(typeof(IntPtr));
            if (Interop.CredEnumerate(Filter, CredFlags, out var count, out var pCredentials))
                return new CriticalCredGetterHandle<IEnumerable<Credential>>(pCredentials,
                    p => Enumerable.Range(0, count)
                             .Select(n => Marshal.ReadIntPtr(p, n * Size))
                             .Select(From));
            var hresult = Marshal.GetHRForLastWin32Error();
            var exception = Marshal.GetExceptionForHR(hresult);
            if (unchecked((uint)hresult) == 0x80070032)
                throw new NotSupportedException("not support.", exception);
            throw exception;
        }
        /// <summary>
        /// ポインタからの変換
        /// </summary>
        /// <param name="ptr"></param>
        public static Credential From(IntPtr ptr) => (Credential)Marshal.PtrToStructure(ptr, typeof(Credential));
        public static ICredGetterHandle<Credential> Read(string TagetName, CredType Type = default(CredType), CredReadFlags Flags = default(CredReadFlags))
        {
            if (Interop.CredRead(TagetName, Type, Flags, out var CredentialPtr))
                return new CriticalCredGetterHandle<Credential>(CredentialPtr,From);
            var hresult = Marshal.GetHRForLastWin32Error();
            var exception = Marshal.GetExceptionForHR(hresult);
            if (unchecked((uint)hresult) == 0x80070032)
                throw new NotSupportedException("not support.", exception);
            throw exception;
        }
        /// <summary>
        /// 資格情報の登録
        /// </summary>
        /// <param name="unmanagedCred"></param>
        /// <param name="Flags"></param>
        public void Write(CredWriteFlags Flags = default(CredWriteFlags))
        {
            if (Interop.CredWrite(ref this, Flags))
                return;
            var hresult = Marshal.GetHRForLastWin32Error();
            var exception = Marshal.GetExceptionForHR(hresult);
            if (unchecked((uint)hresult) == 0x80070032)
                throw new NotSupportedException("not support.", exception);
            throw exception;
        }
        /// <summary>
        /// 指定した認証情報を書き込みます。
        /// </summary>
        /// <param name="Credential">認証情報</param>
        /// <param name="Flags"></param>
        public static void Write(WinCred.Credential Credential,CredWriteFlags Flags)
        {
            var CredentialBlobPtr = IntPtr.Zero;
            var AttributePtr = IntPtr.Zero;
            try
            {
                var uc = new Credential(Credential)
                {
                    CredentialBlobSize = (uint)(Credential.CredentialBlob?.Length ?? 0),
                    AttributeCount = (uint)(Credential.Attributes?.Length ?? 0),
                };
                if (uc.CredentialBlobSize > 0)
                {
                    uc.CredentialBlobPtr = CredentialBlobPtr = Marshal.AllocCoTaskMem(sizeof(byte) * (int)uc.CredentialBlobSize);
                    Marshal.Copy(Credential.CredentialBlob, 0, uc.CredentialBlobPtr, Credential.CredentialBlob.Length);
                }
                if (uc.AttributeCount > 0)
                {
                    uc.AttributesPtr = AttributePtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(CredentialAttribute)) * (int)uc.AttributeCount);
                    Marshal.StructureToPtr(Credential.Attributes, uc.AttributesPtr, false);
                }
                uc.Write(Flags);
            }
            finally
            {
                if (CredentialBlobPtr != IntPtr.Zero)
                    Marshal.FreeCoTaskMem(CredentialBlobPtr);
                if (AttributePtr != IntPtr.Zero)
                    Marshal.FreeCoTaskMem(AttributePtr);
            }
        }
        public void Delete(CredDeleteFlags Flags = default(CredDeleteFlags)) => Delete(TargetName, Type, Flags);
        /// <summary>
        /// 削除します。
        /// </summary>
        /// <param name="TargetName"></param>
        /// <param name="Type"></param>
        /// <param name="Falgs"></param>
        public static void Delete(string TargetName, CredType Type, CredDeleteFlags Falgs = default(CredDeleteFlags))
        {
            if (Interop.CredDelete(TargetName, Type, Falgs))
                return;
            var hresult = Marshal.GetHRForLastWin32Error();
            var exception = Marshal.GetExceptionForHR(hresult);
            if (unchecked((uint)hresult) == 0x80070032)
                throw new NotSupportedException("not support.", exception);
            throw exception;
        }
        public void Rename(string NewTargetName, CredType Type, CredRenameFlags Flags = default(CredRenameFlags))
        {
            Rename(TargetName, NewTargetName, Type, Flags);
            TargetName = NewTargetName;
        }
        public static void Rename(string OldTargetName, string NewTargetName, CredType Type, CredRenameFlags Flags = default(CredRenameFlags))
        {
            if (Interop.CredRename(OldTargetName, NewTargetName, Type, Flags))
                return;
            var hresult = Marshal.GetHRForLastWin32Error();
            var exception = Marshal.GetExceptionForHR(hresult);
            if (unchecked((uint)hresult) == 0x80070032)
                throw new NotSupportedException("not support.", exception);
            throw exception;
        }
        /// <summary>
        /// マネージドな Credential からの生成
        /// </summary>
        /// <param name="Credential"></param>
        public Credential(WinCred.Credential Credential)
        {
            Flags = Credential.Flags;
            Type = Credential.Type;
            TargetName = Credential.TargetName;
            Comment = Credential.Comment;
            LastWritten = Credential.LastWritten.ToFILETIMEStructure();
            CredentialBlobSize = 0;
            CredentialBlobPtr = IntPtr.Zero;
            Persist = Credential.Persist;
            TargetAlias = Credential.TargetAlias;
            UserName = Credential.UserName;
            AttributeCount = 0;
            AttributesPtr = IntPtr.Zero;
        }
        public WinCred.Credential ToManaged()
        {
            return new WinCred.Credential
            {
                Flags = Flags,
                Type = Type,
                TargetName = TargetName,
                Comment = Comment,
                LastWritten = LastWritten.ToDateTime(),
                CredentialBlob = CredentialBlob,
                Persist = Persist,
                //Attributes = (Attributes ?? Enumerable.Empty<CredentialAttribute>())
                //    .Select(uca => uca.ToManaged())
                //    .ToArray(),
                TargetAlias = TargetAlias,
                UserName = UserName,
            };
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
    static class TimeExtension
    {
        public static DateTime ToDateTime(this System.Runtime.InteropServices.ComTypes.FILETIME time)
        {
            var hFT2 = (((long)time.dwHighDateTime) << 32) | (uint)time.dwLowDateTime;
            return DateTime.FromFileTime(hFT2);
        }
        public static System.Runtime.InteropServices.ComTypes.FILETIME ToFILETIMEStructure(this DateTime time)
        {
            var hFT1 = time.ToFileTime();
            return new System.Runtime.InteropServices.ComTypes.FILETIME
            {
                dwLowDateTime = (int)(hFT1 & 0xFFFFFFFF),
                dwHighDateTime = (int)(hFT1 >> 32),
            };
        }
    }
}

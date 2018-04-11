﻿using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace Advapi32.WinCred
{
    /// <summary>
    /// アンマネージドなCredential
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct UnmanagedCredential
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
            set
            {
                if (value?.Length > 0)
                {
                    IntPtr p = IntPtr.Zero;
                    CredentialBlobSize = (uint)value.Length;
                    try
                    {
                        p = Marshal.AllocHGlobal(value.Length);
                        Marshal.Copy(value, 0, p, value.Length);
                        CredentialBlobPtr = p;
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(p);
                    }
                }
                else
                {
                    CredentialBlobPtr = IntPtr.Zero;
                    CredentialBlobSize = 0;
                }
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
                var Ptrs = Enumerable
                    .Range(0, (int)AttributeCount - 1)
                    .Select(x => Marshal.ReadIntPtr(self.AttributesPtr, x * Size));
                var Strs = Ptrs
                    .Select(x => (CredentialAttribute)Marshal.PtrToStructure(x, typeof(CredentialAttribute)));

                //var Strs = Ptrs
                //    .Select(x =>
                //    {
                //        var bytes = new byte[Size];
                //        Marshal.Copy(x, bytes, 0, Size);
                //        return bytes;
                //    })
                //    .Select(bytes =>
                //    {
                //        GCHandle? handle = null;
                //        try
                //        {
                //            handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
                //            return (CredentialAttribute)Marshal.PtrToStructure(handle?.AddrOfPinnedObject() ?? IntPtr.Zero, typeof(CredentialAttribute));
                //        }
                //        finally
                //        {
                //            handle?.Free();
                //        }
                //    });
                 return Strs
                    .ToArray();
            }
            set
            {
                if (value?.Length > 0)
                {
                    AttributeCount = (uint)value.Length;
                    Marshal.Copy(value.Cast<int>().ToArray(), 0, AttributesPtr, value.Length);
                }
                else
                {
                    AttributeCount = 0;
                    AttributesPtr = IntPtr.Zero;
                }
            }
        }
        public string TargetAlias;
        public string UserName;
        /// <summary>
        /// ポインタからの変換
        /// </summary>
        /// <param name="ptr"></param>
        public static UnmanagedCredential FromPtr(IntPtr ptr) => (UnmanagedCredential)Marshal.PtrToStructure(ptr, typeof(UnmanagedCredential));
        /// <summary>
        /// マネージドな Credential からの生成
        /// </summary>
        /// <param name="Credential"></param>
        public UnmanagedCredential(Credential Credential)
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
            CredentialBlob = Credential.CredentialBlob;
            Attributes = Credential.Attributes;
        }
        public Credential ToCredential()
        {
            return new Credential
            {
                Flags = Flags,
                Type = Type,
                TargetName = TargetName,
                Comment = Comment,
                LastWritten = LastWritten.ToDateTime(),
                CredentialBlob = CredentialBlob,
                Persist = Persist,
                Attributes = Attributes,
                TargetAlias = TargetAlias,
                UserName = UserName,
            };
        }
        public override string ToString()
            => $"{nameof(UnmanagedCredential)}{{"
            + $"{nameof(Flags)}:{Flags}"
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

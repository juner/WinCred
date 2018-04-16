using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Advapi32.WinCred.Unmanaged
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
    public struct CredentialAttribute : IUnmanaged<WinCred.CredentialAttribute>
    {
        [MarshalAs(UnmanagedType.LPWStr)]
        public string Keyword;
        public uint Flags;
        internal uint ValueSize;
        internal IntPtr ValuePtr;
        public byte[] Value
        {
            get
            {
                System.Diagnostics.Debug.Assert(ValuePtr == IntPtr.Zero ? ValueSize == 0 : ValueSize >= 0, $"{nameof(ValueSize)}だけ初期化されて、{nameof(ValuePtr)}が初期化されていない。");
                if (ValuePtr == IntPtr.Zero)
                    return null;
                var bytes = new byte[ValueSize];
                if (ValueSize > 0)
                    Marshal.Copy(ValuePtr, bytes, 0, (int)ValueSize);
                return bytes;
            }
            set
            {
                if (value?.Length > 0)
                {
                    IntPtr p = IntPtr.Zero;
                    ValueSize = (uint)value.Length;
                    try
                    {
                        p = Marshal.AllocHGlobal(value.Length);
                        Marshal.Copy(value, 0, p, value.Length);
                        ValuePtr = p;
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(p);
                    }
                }
                else
                {
                    ValuePtr = IntPtr.Zero;
                    ValueSize = 0;
                }
            }
        }
        /// <summary>
        /// ポインターよりインスタンスを生成する
        /// </summary>
        /// <param name="ptr"></param>
        /// <returns></returns>
        public static CredentialAttribute From(IntPtr Ptr) => Marshal.PtrToStructure<CredentialAttribute>(Ptr);
        /// <summary>
        /// マネージドからインスタンスを作成する
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static CredentialAttribute From(WinCred.CredentialAttribute attribute)
            => new CredentialAttribute
            {
                Keyword = attribute.Keyword,
                Flags = attribute.Flags,
                //ValueSize = (uint)attribute.Value.Length,
                //Value = attribute.Value,
            };
        public IDisposable Copy(WinCred.CredentialAttribute attribute)
        {
            Keyword = attribute.Keyword;
            Flags = attribute.Flags;
            ValueSize = (uint)attribute.Value.Length;
            var Ptr = ValuePtr = Marshal.AllocCoTaskMem((int)ValueSize);
            Marshal.Copy(attribute.Value, 0, ValuePtr, (int)ValueSize);
            return Disposable.Create(() =>
            {
                if (Ptr != IntPtr.Zero)
                    Marshal.FreeCoTaskMem(Ptr);
                Ptr = IntPtr.Zero;
            });
        }

        /// <summary>
        /// マネージド版に変換する
        /// </summary>
        /// <returns></returns>
        public WinCred.CredentialAttribute ToManaged()
        {
            return new WinCred.CredentialAttribute
            {
                Keyword = Keyword,
                Flags = Flags,
                Value = Value,
            };
        }
        public override string ToString()
            => $"{nameof(CredentialAttribute)} {{"
                + $"{nameof(Keyword)}: {Keyword}"
                + $", {nameof(Flags)}: {Flags}"
                + $", {nameof(Value)}: [{(ValueSize > 0 ? string.Join(" ",Value) : "")}]"
                + $"}}";
    }
}

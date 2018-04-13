using System;
using System.Runtime.InteropServices;

namespace Advapi32.WinCred.Unmanaged
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
    public struct CredentialAttribute
    {
        public string Keyword;
        public uint Flags;
        uint ValueSize;
        IntPtr _Value;
        public byte[] Value
        {
            get
            {
                System.Diagnostics.Debug.Assert(_Value == IntPtr.Zero ? ValueSize == 0 : ValueSize >= 0, $"{nameof(ValueSize)}だけ初期化されて、{nameof(_Value)}が初期化されていない。");
                if (_Value == IntPtr.Zero)
                    return null;
                var bytes = new byte[ValueSize];
                if (ValueSize > 0)
                    Marshal.Copy(_Value, bytes, 0, (int)ValueSize);
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
                        _Value = p;
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(p);
                    }
                }
                else
                {
                    _Value = IntPtr.Zero;
                    ValueSize = 0;
                }
            }
        }
        /// <summary>
        /// ポインターよりインスタンスを生成する
        /// </summary>
        /// <param name="ptr"></param>
        /// <returns></returns>
        public static CredentialAttribute From(IntPtr Ptr)
        {
            return (CredentialAttribute)Marshal.PtrToStructure(Ptr, typeof(CredentialAttribute));
            //var Size = Marshal.SizeOf(typeof(UnmanagedCredentialAttribute));
            //var bytes = new byte[Size];
            //Marshal.Copy(Ptr, bytes, 0, Size);
            //var Keyword = Marshal.PtrToStringBSTR(new IntPtr(BitConverter.ToInt32(bytes, 0)));
            //var Flags = BitConverter.ToUInt32(bytes, 4);
            //var _Value = new IntPtr(BitConverter.ToInt32(bytes, 8));
            //return new UnmanagedCredentialAttribute
            //{
            //    Keyword = Keyword,
            //    Flags = Flags,
            //    _Value = _Value,
            //};
        }
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
                Value = attribute.Value,
            };

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

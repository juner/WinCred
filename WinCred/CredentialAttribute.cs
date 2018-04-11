using System;
using System.Runtime.InteropServices;

namespace Advapi32.WinCred
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
    public struct CredentialAttribute
    {
        [MarshalAs(UnmanagedType.LPWStr)]
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
        public override string ToString()
            => $"{nameof(CredentialAttribute)} {{"
                + $"{nameof(Keyword)}: {Keyword}"
                + $", {nameof(Flags)}: {Flags}"
                + $", {nameof(Value)}: [{(ValueSize > 0 ? string.Join(" ",Value) : "")}"
                + $"}}";
    }
}

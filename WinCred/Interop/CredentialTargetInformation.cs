using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Advapi32.WinCred.Unmanaged
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct CredentialTargetInformation :IUnmanaged<WinCred.CredentialTargetInformation>
    {
        public string TargetName;
        public string NetbiosServerName;
        public string DnsServerName;
        public string NetbiosDomainName;
        public string DnsDomainName;
        public string DnsTreeName;
        public string PackageName;
        public CredTiFlags Flags;
        private uint CredTypeCount;
        private IntPtr CredTypesPtr;
        public CredType[] CredTypes
        {
            get
            {
                if (CredTypesPtr == IntPtr.Zero || CredTypeCount <= 0)
                    return null;
                var Self = this;
                var Size = sizeof(CredType);
                var CredTypes = new int[CredTypeCount];
                Marshal.Copy(CredTypesPtr, CredTypes, 0, (int)CredTypeCount);
                return CredTypes.Cast<CredType>().ToArray();
            }
        }
        public IDisposable CopyCredType(CredType[] CredTypes)
        {
            var CredTypeCount = (int)this.CredTypeCount;
            var CredTypesPtr = Marshal.AllocCoTaskMem(CredTypeCount * sizeof(CredType));
            var DisposableResult = Disposable.Create(() => {
                if (CredTypesPtr != IntPtr.Zero)
                    Marshal.FreeCoTaskMem(CredTypesPtr);
                CredTypesPtr = IntPtr.Zero;
            });
            try
            {
                Marshal.Copy(CredTypes.Cast<int>().ToArray(), 0, CredTypesPtr, CredTypeCount);
                return DisposableResult;
            }
            catch
            {
                DisposableResult?.Dispose();
                throw;
            }
        }
        public static ICredGetterHandle<CredentialTargetInformation> GetTargetInfo(string TargetName, CredGetTargetInfoFlags Flags = default)
        {
            if (Interop.CredGetTargetInfo(TargetName, Flags, out var TargetInfo))
                return new CriticalCredGetterHandle<CredentialTargetInformation>(TargetInfo, From);
            var hresult = Marshal.GetHRForLastWin32Error();
            var exception = Marshal.GetExceptionForHR(hresult);
            if (unchecked((uint)hresult) == 0x80070032)
                throw new NotSupportedException("not support.", exception);
            throw exception;
        }
        public static CredentialTargetInformation From(IntPtr TargetInfoPtr) => Marshal.PtrToStructure<CredentialTargetInformation>(TargetInfoPtr);
        public void WriteDomainCredentials(Credential Credential, CredWriteDomainCredentialsFlag Flags = default)
        {
            if (Interop.CredWriteDomainCredentials(ref this, ref Credential, Flags))
                return;
            var hresult = Marshal.GetHRForLastWin32Error();
            var exception = Marshal.GetExceptionForHR(hresult);
            if (unchecked((uint)hresult) == 0x80070032)
                throw new NotSupportedException("not support.", exception);
            throw exception;
        }
        public ICredGetterHandle<IEnumerable<Credential>> ReadDomainCredentials(CredReadDomainCredentialsFlags Flags = default)
        {
            if (Interop.CredReadDomainCredentials(ref this, Flags, out var Count, out var Credentials))
                return new CriticalCredGetterHandle<IEnumerable<Credential>>(Credentials, p => Credential.Froms((int)Count, Credentials));
            var hresult = Marshal.GetHRForLastWin32Error();
            var exception = Marshal.GetExceptionForHR(hresult);
            if (unchecked((uint)hresult) == 0x80070032)
                throw new NotSupportedException("not support.", exception);
            throw exception;
        }

        public WinCred.CredentialTargetInformation ToManaged()
            => new WinCred.CredentialTargetInformation
        {
            TargetName = TargetName,
            NetbiosServerName = NetbiosServerName,
            DnsServerName = DnsServerName,
            NetbiosDomainName = NetbiosDomainName,
            DnsDomainName = DnsDomainName,
            DnsTreeName = DnsTreeName,
            PackageName = PackageName,
            Flags = Flags,
            CredTypes = CredTypes,
        };
    }
}
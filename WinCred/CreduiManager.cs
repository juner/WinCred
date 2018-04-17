using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Credui.WinCred
{
    public static class CreduiManager
    {
        /// <summary>
        /// シングルサインオン認証情報を保存します。
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="Password"></param>
        /// <param name="Persist"></param>
        public static void StoreSSOCred(string UserName, string Password, bool Persist) => StoreSSOCred(null, UserName, Password, Persist);
        /// <summary>
        /// シングルサインオン認証情報を保存します。
        /// </summary>
        /// <param name="Realm"></param>
        /// <param name="UserName"></param>
        /// <param name="Password"></param>
        /// <param name="Persist"></param>
        public static void StoreSSOCred(string Realm, string UserName, string Password, bool Persist)
        {
            if (string.IsNullOrEmpty(UserName))
                throw new ArgumentNullException(nameof(UserName));
            if (string.IsNullOrEmpty(Password))
                throw new ArgumentNullException(nameof(Password));
            var result = Interop.CredUIStoreSSOCred(Realm, UserName, Password, Persist);
            if (result != CredUIReturnCodes.NoError)
                throw new CredUIException(result);
        }
        /// <summary>
        /// シングルサインオン認証情報のユーザ名を取得します。
        /// </summary>
        /// <returns></returns>
        /// <exception cref="CredUIException">見つからなかった場合</exception>
        public static ICredUIGetter<string> ReadSSOCred() => ReadSSOCred(null);
        /// <summary>
        /// シングルサインオン認証情報のユーザ名を取得します。
        /// </summary>
        /// <param name="Realm"></param>
        /// <returns></returns>
        /// <exception cref="CredUIException">見つからなかった場合</exception>
        public static ICredUIGetter<string> ReadSSOCred(string Realm)
        {
            var result = Interop.CredUIReadSSOCred(Realm, out var UserName);
            if (result != CredUIReturnCodes.NoError)
                throw new CredUIException(result);
            return new CredUIGetterHandle<string>(UserName, (handle) => Marshal.PtrToStringUni(handle));
        }
        /// <summary>
        /// コマンドライン（コンソール）アプリケーションで作業しているユーザーからの資格情報を要求し、その資格情報を受け入れます。ユーザーが入力した名前とパスワードは、検証のために呼び出し元のアプリケーションに戻されます。
        /// </summary>
        /// <param name="TargetName">資格情報のターゲット名</param>
        /// <param name="Reserved">予約された引数。NULLの必要があります。</param>
        /// <param name="AuthError">資格情報の入力が必要な理由を指定します。呼び出し元は、別の認証呼び出しによって返されたこのWindowsエラーパラメータを渡して、ダイアログボックスが特定のエラーに対応できるようにすることができます。</param>
        /// <param name="UserName">信任状のユーザー名</param>
        /// <param name="UserNameMaxChars"></param>
        /// <param name="Password"></param>
        /// <param name="PasswordMaxChars"></param>
        /// <param name="Save">資格情報保存の初期状態</param>
        /// <param name="Flags"></param>
        public static (CredUIReturnCodes ResultCode, string UserName, string Password, bool IsSave) CmdLinePromptForCredentials(string TargetName, IntPtr Reserved, int AuthError, string UserName, uint UserNameMaxChars, string Password, uint PasswordMaxChars, bool Save, CreduiFlags Flags)
        {
            var UserNameBuffer = new StringBuilder((int)UserNameMaxChars);
            if (!string.IsNullOrEmpty(UserName))
                UserNameBuffer.Append(UserName);
            var PasswordBuffer = new StringBuilder((int)PasswordMaxChars);
            if (!string.IsNullOrEmpty(Password))
                PasswordBuffer.Append(Password);
            var result = Interop.CredUICmdLinePromptForCredentials(TargetName, Reserved, AuthError, UserNameBuffer, UserNameMaxChars, PasswordBuffer, PasswordMaxChars, ref Save, Flags);
            return (result, UserNameBuffer.ToString(), PasswordBuffer.ToString(), Save);
        }
        public static byte[] PromptForCredentials(in CreduiInfo CredInfo, int AuthError, ref uint AuthPackage, IntPtr InAuthBuffer, uint InAuthBufferSize, ref bool Save, CredUiWinFlags Flags)
        {
            var result = Interop.CredUIPromptForWindowsCredentials(CredInfo, AuthError, ref AuthPackage, InAuthBuffer, InAuthBufferSize, out var OutAuthBuffer, out var OutAuthBufferSize, ref Save, Flags);
            if (result != CredUIReturnCodes.NoError)
                throw new CredUIException(result);

            return null;
        }
        /// <summary>
        /// <see cref="Prom"/>
        /// </summary>
        /// <param name="Flags"></param>
        /// <param name="AuthBuffer"></param>
        /// <param name="AuthBufferSize"></param>
        /// <returns></returns>
        public static (string UserName, string DomainName, string Password) UnPackAuthenticationBuffer(CredPackFlags Flags, IntPtr AuthBuffer, uint AuthBufferSize)
        {
            var MaxUserName = 0u;
            var MaxDomainName = 0u;
            var MaxPassword = 0u;
            Interop.CredUnPackAuthenticationBuffer(Flags, AuthBuffer, AuthBufferSize, null, ref MaxUserName, null, ref MaxDomainName, null, ref MaxPassword);
            var UserNameBuffer = new StringBuilder((int)MaxUserName);
            var DomainNameBuffer = new StringBuilder((int)MaxDomainName);
            var PasswordBuffer = new StringBuilder((int)MaxPassword);
            if(Interop.CredUnPackAuthenticationBuffer(Flags, AuthBuffer, AuthBufferSize, UserNameBuffer, ref MaxUserName, DomainNameBuffer, ref MaxDomainName, PasswordBuffer, ref MaxPassword))
                return (UserNameBuffer.ToString(),DomainNameBuffer.ToString(), PasswordBuffer.ToString());
            var hresult = Marshal.GetHRForLastWin32Error();
            var exception = Marshal.GetExceptionForHR(hresult);
            if (unchecked((uint)hresult) == 0x80070032)
                throw new NotSupportedException("not support.", exception);
            throw exception;
        }
        public static byte[] PackAuthenticationBuffer(CredPackFlags Flags, string UserName, string Password)
        {
            var Size = 0u;
            Interop.CredPackAuthenticationBuffer(Flags, UserName, Password, IntPtr.Zero, ref Size);
            var PackedCredentialsPtr = Marshal.AllocCoTaskMem((int)Size);
            try
            {
                if(Interop.CredPackAuthenticationBuffer(Flags, UserName, Password, PackedCredentialsPtr, ref Size))
                {
                    var PackedCredentials = new byte[Size];
                    Marshal.Copy(PackedCredentialsPtr, PackedCredentials, 0, (int)Size);
                    return PackedCredentials;
                }
                var hresult = Marshal.GetHRForLastWin32Error();
                var exception = Marshal.GetExceptionForHR(hresult);
                if (unchecked((uint)hresult) == 0x80070032)
                    throw new NotSupportedException("not support.", exception);
                throw exception;
            }
            finally
            {
                if (PackedCredentialsPtr != IntPtr.Zero)
                    Marshal.FreeCoTaskMem(PackedCredentialsPtr);
            }
        }
    }
    public class CredUIException : Exception
    {
        public CredUIReturnCodes CredUIReturnCodes { get; }
        public CredUIException(CredUIReturnCodes CredUIReturnCodes) => this.CredUIReturnCodes = CredUIReturnCodes;
    }
}

using System;

namespace Credui.WinCred
{
    [Flags]
    public enum CredUiWinFlags : uint{
        /// <summary>
        /// 呼び出し元は、資格プロバイダがプレーンテキストでユーザー名とパスワードを返すよう要求しています。この値は<see cref="SecurePrompt"/>と組み合わせることはできません。
        /// </summary>
        Generic = 0x1,
        /// <summary>
        /// ダイアログボックスに[保存] チェックボックスが表示されます。
        /// </summary>
        Checkbox = 0x2,
        /// <summary>
        /// authPackageパラメーターで指定された認証パッケージをサポートする信任状プロバイダーのみを列挙する必要があります。この値は<see cref="IsCredOnly"/>と組み合わせることはできません。
        /// </summary>
        AuthPackageOnly = 0x10,
        /// <summary>
        /// authPackageパラメーターで指定された認証パッケージのInAuthBufferパラメーターで指定された資格情報のみを列挙する必要があります。このフラグが設定され、InAuthBufferパラメータがNULLの場合、関数は失敗します。この値は<see cref="AuthPackageOnly"/>と組み合わせることはできません。
        /// </summary>
        IsCredOnly = 0x20,
        /// <summary>
        /// 資格プロバイダは管理者のみを列挙する必要があります。この値は、ユーザーアカウント制御（UAC）のみを目的としています。外部の発信者はこのフラグを設定しないことを推奨します。
        /// </summary>
        EnumerateAdmins = 0x100,
        /// <summary>
        /// authPackageパラメーターで指定された認証パッケージの受信資格情報のみを列挙する必要があります。
        /// </summary>
        EnumerateCurrentUser = 0x200,
        /// <summary>
        /// セキュリティ保護されたデスクトップに資格情報ダイアログボックスが表示されます。この値はCREDUIWIN_GENERICと組み合わせることはできません。 <br/>Windows Vista：Windows Vista SP1以降では、この値はサポートされていません。
        /// </summary>
        SecurePrompt = 0x1000,
        /// <summary>
        /// 資格プロバイダは、プロバイダが64ビットシステムで実行されている場合でも、refOutAuthBufferパラメータによって指された資格情報BLOBを32ビット境界に揃える必要があります。
        /// </summary>
        Pack32Wow = 0x10000000,
    }
}

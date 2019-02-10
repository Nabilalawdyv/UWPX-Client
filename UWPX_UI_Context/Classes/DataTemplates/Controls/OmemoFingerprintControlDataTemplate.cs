﻿using libsignal;
using Shared.Classes;
using XMPP_API.Classes.Crypto;

namespace UWPX_UI_Context.Classes.DataTemplates.Controls
{
    public sealed class OmemoFingerprintControlDataTemplate : AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private string _Fingerprint;
        public string Fingerprint
        {
            get { return _Fingerprint; }
            set { SetProperty(ref _Fingerprint, value); }
        }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void UpdateView(AccountDataTemplate account)
        {
            if (!(account is null))
            {
                IdentityKey key = account.Account.omemoIdentityKeyPair?.getPublicKey();
                if (!(key is null))
                {
                    Fingerprint = CryptoUtils.genOmemoFingerprint(key);
                    return;
                }
                else
                {
                    Fingerprint = "Failed to load fingerprint!";
                }
            }
            Fingerprint = "";
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}

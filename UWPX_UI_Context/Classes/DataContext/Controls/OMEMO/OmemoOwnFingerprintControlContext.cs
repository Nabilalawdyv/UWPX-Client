﻿using UWPX_UI_Context.Classes.DataTemplates.Controls.OMEMO;
using Windows.UI.Xaml;
using XMPP_API.Classes.Network;

namespace UWPX_UI_Context.Classes.DataContext.Controls.OMEMO
{
    public class OmemoOwnFingerprintControlContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly OmemoOwnFingerprintControlDataTemplate MODEL = new OmemoOwnFingerprintControlDataTemplate();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void UpdateView(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is XMPPAccount account)
            {
                MODEL.UpdateView(account);
            }
            else
            {
                MODEL.UpdateView(null);
            }
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

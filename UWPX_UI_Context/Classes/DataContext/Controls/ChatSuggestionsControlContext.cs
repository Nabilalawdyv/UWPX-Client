﻿using System.Threading.Tasks;
using UWPX_UI_Context.Classes.DataTemplates.Controls;
using XMPP_API.Classes;

namespace UWPX_UI_Context.Classes.DataContext.Controls
{
    public sealed class ChatSuggestionsControlContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly ChatSuggestionsControlDataTemplate MODEL = new ChatSuggestionsControlDataTemplate();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public async Task UpdateViewAsync(XMPPClient client)
        {
            await MODEL.UpdateViewAsync(client);
        }

        public void UpdateView(string filterText)
        {
            MODEL.UpdateView(filterText);
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

﻿using System.Threading;
using System.Threading.Tasks;
using Logging;
using UWPX_UI_Context.Classes.DataTemplates;
using UWPX_UI_Context.Classes.DataTemplates.Dialogs;
using XMPP_API.Classes.Network.XML.Messages.XEP_0249;

namespace UWPX_UI_Context.Classes.DataContext.Dialogs
{
    public class MucInviteDialogContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly MucInviteDialogDataTemplate MODEL = new MucInviteDialogDataTemplate();
        private CancellationTokenSource inviteCTS;
        private Task inviteTask;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void UpdateView(ChatDataTemplate chat)
        {
            MODEL.RoomIsPasswordProtected = !string.IsNullOrEmpty(chat.MucInfo.password);
            MODEL.IncludePassword = MODEL.RoomIsPasswordProtected;
            MODEL.Title = "Invite to: " + (string.IsNullOrEmpty(chat.MucInfo.name) ? chat.Chat.chatJabberId : chat.MucInfo.name);
        }

        public async Task InviteAsync(ChatDataTemplate chat)
        {
            inviteCTS = new CancellationTokenSource();

            // Create a new task:
            inviteTask = Task.Run(async () =>
            {
                MODEL.IsInviting = true;
                Logger.Info("Sending an XEP-0249: Direct MUC Invitation to " + MODEL.TargetBareJid + " for room " + chat.Chat.chatJabberId + ".");
                string pw = MODEL.IncludePassword ? chat.MucInfo.password : null;
                await chat.Client.SendAsync(new DirectMUCInvitationMessage(chat.Client.getXMPPAccount().getBareJid(), MODEL.TargetBareJid, chat.Chat.chatJabberId, pw, MODEL.Reason));
                MODEL.IsInviting = false;
            }, inviteCTS.Token);

            // Await it:
            try
            {
                await inviteTask;
            }
            catch (TaskCanceledException) { }
            inviteCTS = null;
        }

        public void Cancel()
        {
            if (!(inviteCTS is null))
            {
                inviteCTS.Cancel();
                MODEL.IsInviting = false;
                Logger.Info("Canceled XEP-0249: Direct MUC Invitation for " + MODEL.TargetBareJid + ".");
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

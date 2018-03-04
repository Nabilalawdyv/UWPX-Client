﻿using System;
using Data_Manager2.Classes.DBTables;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes;
using UWP_XMPP_Client.Dialogs;

namespace UWP_XMPP_Client.Controls
{
    public sealed partial class SpeechBubbleMUCDirectInvitationControl : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public ChatTable Chat
        {
            get { return (ChatTable)GetValue(ChatProperty); }
            set { SetValue(ChatProperty, value); }
        }
        public static readonly DependencyProperty ChatProperty = DependencyProperty.Register("Chat", typeof(ChatTable), typeof(ChatDetailsControl), null);

        public MUCDirectInvitationTable Invitation
        {
            get { return (MUCDirectInvitationTable)GetValue(InvitationProperty); }
            set { SetValue(InvitationProperty, value); }
        }
        public static readonly DependencyProperty InvitationProperty = DependencyProperty.Register("Invitation", typeof(MUCDirectInvitationTable), typeof(SpeechBubbleMUCDirectInvitationControl), null);

        public ChatMessageTable ChatMessage
        {
            get { return (ChatMessageTable)GetValue(ChatMessageProperty); }
            set
            {
                SetValue(ChatMessageProperty, value);
                loadInvitation();
                showDate();
            }
        }
        public static readonly DependencyProperty ChatMessageProperty = DependencyProperty.Register("ChatMessage", typeof(ChatMessageTable), typeof(SpeechBubbleMUCDirectInvitationControl), null);

        private bool invitationLoaded;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 02/03/2018 Created [Fabian Sauter]
        /// </history>
        public SpeechBubbleMUCDirectInvitationControl()
        {
            this.Invitation = null;
            this.invitationLoaded = false;
            this.InitializeComponent();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void showDate()
        {
            if (ChatMessage.date.Date.CompareTo(DateTime.Now.Date) == 0)
            {
                date_tbx.Text = ChatMessage.date.ToString("HH:mm");
            }
            else
            {
                date_tbx.Text = ChatMessage.date.ToString("dd.MM.yyyy HH:mm");
            }
        }

        private void loadInvitation()
        {
            if (!invitationLoaded && ChatMessage != null)
            {
                invitationLoaded = true;
                main_grid.Visibility = Visibility.Collapsed;
                error_grid.Visibility = Visibility.Collapsed;
                loading_grid.Visibility = Visibility.Visible;

                string chatMessageId = ChatMessage.id;
                Task.Run(async () =>
                {
                    MUCDirectInvitationTable invitationTable = ChatDBManager.INSTANCE.getMUCDirectInvitation(chatMessageId);
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        Invitation = invitationTable;
                        showInvitation();
                    });
                });
            }
        }

        private void showInvitation()
        {

            if (Invitation != null)
            {
                text_tbx.Text = ChatMessage.fromUser + " has invited you to join:";
                if (Invitation.roomJid == null)
                {
                    error_tbx.Text = "No JID given!";
                    loading_grid.Visibility = Visibility.Collapsed;
                    error_grid.Visibility = Visibility.Visible;
                    return;
                }

                roomJid_tbx.Text = Invitation.roomJid;

                switch (Invitation.state)
                {
                    case MUCDirectInvitationState.REQUESTED:
                        result_tbx.Visibility = Visibility.Collapsed;
                        buttons_grid.Visibility = Visibility.Visible;
                        break;

                    case MUCDirectInvitationState.ACCEPTED:
                        result_tbx.Text = "You have accepted the invitation.";
                        buttons_grid.Visibility = Visibility.Collapsed;
                        result_tbx.Visibility = Visibility.Visible;
                        break;

                    case MUCDirectInvitationState.DECLINED:
                        result_tbx.Text = "You have declined the invitation.";
                        buttons_grid.Visibility = Visibility.Collapsed;
                        result_tbx.Visibility = Visibility.Visible;
                        break;
                }

                loading_grid.Visibility = Visibility.Collapsed;
                main_grid.Visibility = Visibility.Visible;
            }
            else
            {
                error_tbx.Text = "Unable to load invitation from DB!";
                loading_grid.Visibility = Visibility.Collapsed;
                error_grid.Visibility = Visibility.Visible;
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void decline_btn_Click(object sender, RoutedEventArgs e)
        {
            if (Invitation != null)
            {
                Invitation.state = MUCDirectInvitationState.DECLINED;
                string chatMessageId = Invitation.chatMessageId;
                Task.Run(() => ChatDBManager.INSTANCE.setMUCDirectInvitationState(chatMessageId, MUCDirectInvitationState.DECLINED));
                showInvitation();
            }
        }

        private async void accept_btn_Click(object sender, RoutedEventArgs e)
        {
            if (Invitation != null && Chat != null)
            {
                AddMUCDialog dialog;
                if (Invitation.roomPassword != null)
                {
                    dialog = new AddMUCDialog(Invitation.roomJid, Invitation.roomPassword, Chat.userAccountId);
                }
                else
                {
                    dialog = new AddMUCDialog(Invitation.roomJid);
                }
                await dialog.ShowAsync();
                if (dialog.cancled)
                {
                    return;
                }

                Invitation.state = MUCDirectInvitationState.ACCEPTED;
                string chatMessageId = Invitation.chatMessageId;
                await Task.Run(() => ChatDBManager.INSTANCE.setMUCDirectInvitationState(chatMessageId, MUCDirectInvitationState.ACCEPTED));
                showInvitation();
            }
        }

        #endregion
    }
}
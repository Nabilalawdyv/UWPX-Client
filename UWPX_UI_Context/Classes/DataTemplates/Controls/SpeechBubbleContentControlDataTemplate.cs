﻿using System;
using System.Threading.Tasks;
using Data_Manager2.Classes;
using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes.DBTables;
using Data_Manager2.Classes.Toast;
using Shared.Classes;

namespace UWPX_UI_Context.Classes.DataTemplates.Controls
{
    public sealed class SpeechBubbleContentControlDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private string _Text;
        public string Text
        {
            get => _Text;
            set => SetProperty(ref _Text, value);
        }
        private string _NicknameText;
        public string NicknameText
        {
            get => _NicknameText;
            set => SetProperty(ref _NicknameText, value);
        }
        private DateTime _Date;
        public DateTime Date
        {
            get => _Date;
            set => SetProperty(ref _Date, value);
        }
        private bool _IsEncrypted;
        public bool IsEncrypted
        {
            get => _IsEncrypted;
            set => SetProperty(ref _IsEncrypted, value);
        }
        private bool _IsCarbonCopy;
        public bool IsCarbonCopy
        {
            get => _IsCarbonCopy;
            set => SetProperty(ref _IsCarbonCopy, value);
        }
        private string _MessageType;
        public string MessageType
        {
            get => _MessageType;
            set => SetProperty(ref _MessageType, value);
        }
        private bool _IsImage;
        public bool IsImage
        {
            get => _IsImage;
            set => SetProperty(ref _IsImage, value);
        }
        private MessageState _State;
        public MessageState State
        {
            get => _State;
            set => SetProperty(ref _State, value);
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
        public void UpdateView(ChatTable chat, ChatMessageTable message)
        {
            if (chat is null || message is null)
            {
                return;
            }

            Text = message.message;
            IsCarbonCopy = message.isCC;
            IsEncrypted = message.isEncrypted;
            Date = message.date;
            MessageType = message.type;
            IsImage = message.isImage;
            State = message.state;

            if (State == MessageState.UNREAD)
            {
                // Mark message as read and update the badge notification count:
                Task.Run(() =>
                {
                    ChatDBManager.INSTANCE.markMessageAsRead(message);
                    ToastHelper.UpdateBadgeNumber();
                });
            }

            if (chat.chatType == ChatType.MUC)
            {
                NicknameText = message.fromUser;
            }
            else
            {
                NicknameText = string.Empty;
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

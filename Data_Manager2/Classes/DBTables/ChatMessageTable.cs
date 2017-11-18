﻿using Data_Manager.Classes;
using System;
using XMPP_API.Classes;
using XMPP_API.Classes.Network.XML.Messages;

namespace Data_Manager2.Classes.DBTables
{
    public class ChatMessageTable
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        // Random message id
        public string id { get; set; }
        // the chat id e.g. alice@jabber.orgbob@jaber.de
        public string chatId { get; set; }
        // error, chat, groupchat, ....
        public string type { get; set; }
        // The actual chat message
        public string message { get; set; }
        // Which user has send the message (useful for group chats e.g muc or mix)
        public string fromUser { get; set; }
        // The message date
        public DateTime date { get; set; }
        // send, read, sending, ...
        public MessageState state { get; set; }

        public event EventHandler ChatMessageChanged;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 18/11/2017 Created [Fabian Sauter]
        /// </history>
        public ChatMessageTable()
        {

        }

        public ChatMessageTable(MessageMessage msg, ChatTable chat)
        {
            if (msg.getType() != null && msg.getType().Equals("error"))
            {
                this.id = msg.getId() + '_' + chat.id + "_error";
            }
            else
            {
                this.id = msg.getId() + '_' + chat.id;
            }
            this.chatId = chat.id;
            this.type = msg.getType();
            this.message = msg.getMessage();
            this.fromUser = Utils.removeResourceFromJabberid(msg.getFrom());
            this.date = msg.getDelay();
            if (this.date == null || this.date.Equals(DateTime.MinValue))
            {
                this.date = DateTime.Now;
            }
            this.state = MessageState.UNREAD;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        /// <summary>
        /// Triggers the ChatMessageChanged event.
        /// </summary>
        public void onChanged()
        {
            ChatMessageChanged?.Invoke(this, new EventArgs());
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
﻿using System.Xml;
using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0363
{
    public class HTTPUploadRequestSlotMessage : IQMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly uint SIZE_BYTE;
        public readonly string FILE_NAME;
        public readonly string CONTENT_TYPE;

        public const string CONTENT_TYPE_JPEG = "image/jpeg";

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 02/06/2018 Created [Fabian Sauter]
        /// </history>
        public HTTPUploadRequestSlotMessage(string from, string to, string fileName, string contentType, uint sizeByte) : base(from, to, GET, getRandomId(), getQueryNode(fileName, contentType, sizeByte))
        {
            this.FILE_NAME = fileName;
            this.CONTENT_TYPE = contentType;
            this.SIZE_BYTE = sizeByte;
        }

        public HTTPUploadRequestSlotMessage(XmlNode node) : base(node)
        {
            XmlNode requestNode = XMLUtils.getChildNode(node, "request", Consts.XML_XMLNS, Consts.XML_XEP_0363_NAMESPACE);
            if (requestNode != null)
            {
                this.FILE_NAME = requestNode.Attributes["filename"]?.Value;
                this.CONTENT_TYPE = requestNode.Attributes["content-type"]?.Value;
                uint.TryParse(requestNode.Attributes["size"]?.Value, out this.SIZE_BYTE);
            }
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private static XElement getQueryNode(string fileName, string contentType, uint sizeByte)
        {
            XNamespace ns = Consts.XML_XEP_0363_NAMESPACE;
            XElement queryNode = new XElement(ns + "request");
            queryNode.Add(new XAttribute("filename", fileName));
            queryNode.Add(new XAttribute("content-type", contentType));
            queryNode.Add(new XAttribute("size", sizeByte));

            return queryNode;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


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
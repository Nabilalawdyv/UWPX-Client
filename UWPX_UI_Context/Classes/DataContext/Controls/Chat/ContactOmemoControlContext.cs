﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data_Manager2.Classes.DBManager.Omemo;
using Data_Manager2.Classes.DBTables;
using Logging;
using UWPX_UI_Context.Classes.DataTemplates.Controls.Chat;
using UWPX_UI_Context.Classes.DataTemplates.Dialogs;
using Windows.UI.Xaml;
using XMPP_API.Classes;
using XMPP_API.Classes.Network.XML.Messages.XEP_0384;
using XMPP_API.Classes.XmppUri;

namespace UWPX_UI_Context.Classes.DataContext.Controls.Chat
{
    public class ContactOmemoControlContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly ContactOmemoControlDataTemplate MODEL = new ContactOmemoControlDataTemplate();

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
            if (e.NewValue is ChatTable chat)
            {
                MODEL.Chat = chat;

                if (!(MODEL.Client is null))
                {
                    LoadFingerprints();
                }
            }
            else if (e.NewValue is XMPPClient client)
            {
                MODEL.Client = client;

                if (!(MODEL.Chat is null))
                {
                    LoadFingerprints();
                }
            }
        }

        public void OnFingerprintTrustedChanged(OmemoFingerprint fingerprint)
        {
            if (fingerprint.trusted)
            {
                MODEL.TrustedOnly = true;
            }

            if (!(MODEL.Client is null))
            {
                OmemoSignalKeyDBManager.INSTANCE.setFingerprint(fingerprint, MODEL.Client.getXMPPAccount().getBareJid());
            }
        }

        public void OnQrCodeScannerShown(QrCodeScannerDialogDataTemplate model)
        {
            if (model.Success)
            {
                Uri uri = null;
                try
                {
                    uri = new Uri(model.QrCode);
                }
                catch (Exception e)
                {
                    Logger.Error("Failed to parse OMEMO fingerprint XMPP URI. Malformed URI: " + model.QrCode, e);
                    return;
                }

                if (string.Equals(uri.LocalPath.ToLowerInvariant(), MODEL.Chat.chatJabberId.ToLowerInvariant()))
                {
                    IUriAction action = UriUtils.parse(uri);

                    if (action is OmemoFingerprintUriAction fingerprintUriAction)
                    {
                        OmemoFingerprint fingerprint = OmemoSignalKeyDBManager.INSTANCE.getFingerprint(fingerprintUriAction.FINGERPRINT.ADDRESS, MODEL.Client.getXMPPAccount().getBareJid());
                        if (fingerprint is null)
                        {
                            fingerprint = fingerprintUriAction.FINGERPRINT;
                        }
                        fingerprint.trusted = true;
                        OmemoSignalKeyDBManager.INSTANCE.setFingerprint(fingerprint, MODEL.Client.getXMPPAccount().getBareJid());
                        Logger.Info("Scanned OMEMO fingerprint successful.");
                        Logger.Debug("Fingerprint: " + fingerprint.ADDRESS.ToString());
                        LoadFingerprints();
                    }
                    else
                    {
                        Logger.Warn("Failed to parse OMEMO fingerprint XMPP URI. Not an " + nameof(OmemoFingerprintUriAction) + ".");
                    }
                }
                else
                {
                    Logger.Warn("Failed to parse OMEMO fingerprint XMPP URI. Wrong chat: " + uri.LocalPath);
                }
            }
        }

        #endregion

        #region --Misc Methods (Private)--
        private void LoadFingerprints()
        {
            if (MODEL.Loading)
            {
                return;
            }
            Task.Run(() =>
            {
                MODEL.Loading = true;
                List<OmemoFingerprint> fingerprints = OmemoSignalKeyDBManager.INSTANCE.getFingerprints(MODEL.Chat.id).ToList();
                // Sort based on the last seen date. If it's the same prefer trusted ones:
                fingerprints.Sort((x, y) =>
                {
                    int dateComp = y.lastSeen.CompareTo(x.lastSeen);
                    if (dateComp == 0)
                    {
                        if (x.trusted == y.trusted)
                        {
                            return 0;
                        }
                        else if (y.trusted)
                        {
                            return 1;
                        }
                        return -1;
                    }
                    return dateComp;
                });
                MODEL.FINGERPRINTS.Clear();
                MODEL.FINGERPRINTS.AddRange(fingerprints);
                MODEL.NoFingerprintsFound = fingerprints.Count <= 0;
                MODEL.Loading = false;
            });
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}

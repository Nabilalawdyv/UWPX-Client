﻿using Data_Manager2.Classes.DBManager;
using System.Collections.ObjectModel;
using UWP_XMPP_Client.DataTemplates;
using Windows.Security.Cryptography.Certificates;
using Windows.UI.Xaml.Controls;
using XMPP_API.Classes.Network;

namespace UWP_XMPP_Client.Dialogs
{
    public sealed partial class ChangeCertificateRequirementsDialog : ContentDialog
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private ObservableCollection<CertificateRequirementTemplate> certificateRequirements;
        private XMPPAccount account;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 12/04/2018 Created [Fabian Sauter]
        /// </history>
		public ChangeCertificateRequirementsDialog(XMPPAccount account)
        {
            this.account = account;
            this.certificateRequirements = new ObservableCollection<CertificateRequirementTemplate>();
            loadCertificateRequirements();
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
        private void loadCertificateRequirements()
        {
            certificateRequirements.Add(new CertificateRequirementTemplate()
            {
                certificateError = ChainValidationResult.Expired,
                description = "The certificate has not expired.",
                required = true,
                name = "Not expired"
            });
            certificateRequirements.Add(new CertificateRequirementTemplate()
            {
                certificateError = ChainValidationResult.InvalidName,
                description = "The certificate has a valid name.",
                required = true,
                name = "Valid name"
            });
            certificateRequirements.Add(new CertificateRequirementTemplate()
            {
                certificateError = ChainValidationResult.InvalidSignature,
                description = "The certificate has a valid signature.",
                required = true,
                name = "Valid signature"
            });
            certificateRequirements.Add(new CertificateRequirementTemplate()
            {
                certificateError = ChainValidationResult.Revoked,
                description = "The certificate has been revoked.",
                required = true,
                name = "Not revoked"
            });
            certificateRequirements.Add(new CertificateRequirementTemplate()
            {
                certificateError = ChainValidationResult.Untrusted,
                description = "The certificate is untrusted/self signed.",
                required = true,
                name = "Trusted"
            });
            certificateRequirements.Add(new CertificateRequirementTemplate()
            {
                certificateError = ChainValidationResult.WrongUsage,
                description = "The certificate is not intended to get used for this.",
                required = true,
                name = "Right usage"
            });

            // Load ignored errors:
            if(account != null)
            {
                foreach (ChainValidationResult item in account.connectionConfiguration.IGNORED_CERTIFICATE_ERRORS)
                {
                    for (int i = 0; i < certificateRequirements.Count; i++)
                    {
                        if(certificateRequirements[i].certificateError == item)
                        {
                            certificateRequirements[i].required = false;
                            break;
                        }
                    }
                }
            }
        }

        private void save()
        {
            if (account == null)
            {
                return;
            }

            account.connectionConfiguration.IGNORED_CERTIFICATE_ERRORS.Clear();

            foreach (CertificateRequirementTemplate c in certificateRequirements)
            {
                if (!c.required)
                {
                    account.connectionConfiguration.IGNORED_CERTIFICATE_ERRORS.Add(c.certificateError);
                }
            }

            AccountDBManager.INSTANCE.saveAccountConnectionConfiguration(account);
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            save();
        }

        #endregion
    }
}

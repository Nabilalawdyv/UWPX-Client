﻿using Data_Manager2.Classes;
using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes.DBTables;
using Data_Manager2.Classes.Events;
using Data_Manager2.Classes.ToastActivation;
using Logging;
using Microsoft.Toolkit.Uwp.UI;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UWP_XMPP_Client.Classes;
using UWP_XMPP_Client.DataTemplates;
using UWP_XMPP_Client.Dialogs;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using XMPP_API.Classes;

namespace UWP_XMPP_Client.Pages
{
    public sealed partial class ChatPage : Page
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly AdvancedCollectionView CHATS_ACV;
        private readonly ObservableChatDictionaryList CHATS;
        private string currentChatQuery;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 26/11/2017 Created [Fabian Sauter]
        /// </history>
        public ChatPage()
        {
            this.CHATS = new ObservableChatDictionaryList();
            this.CHATS_ACV = new AdvancedCollectionView(CHATS, true)
            {
                Filter = aCVFilter
            };
            this.CHATS_ACV.SortDescriptions.Add(new SortDescription(nameof(ChatTemplate.chat), SortDirection.Descending));
            this.currentChatQuery = string.Empty;
            this.InitializeComponent();
            SystemNavigationManager.GetForCurrentView().BackRequested += ChatPage2_BackRequested;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        /// <summary>
        /// Returns the current MasterDetailsView control.
        /// </summary>
        public MasterDetailsView getMasterDetailsView()
        {
            return masterDetail_pnl;
        }

        /// <summary>
        /// Returns true if the chat type of the given chat is CHAT and chat state messages aren't disabled.
        /// </summary>
        /// <param name="chat">The chat which </param>
        /// <returns></returns>
        private bool shouldSendChatState(ChatTable chat)
        {
            return !Settings.getSettingBoolean(SettingsConsts.DONT_SEND_CHAT_STATE) && chat != null && chat.chatType == ChatType.CHAT;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private bool aCVFilter(object o)
        {
            // For searching we could also use something like this: https://www.codeproject.com/Articles/11157/An-improvement-on-capturing-similarity-between-str
            return string.IsNullOrEmpty(currentChatQuery)
                || o is ChatTemplate chat
                    && (chat.chat.chatJabberId.ToLower().Contains(currentChatQuery)
                        || (chat.mucInfo != null
                            && chat.mucInfo.name != null
                            && chat.mucInfo.name.ToLower().Contains(currentChatQuery)));
        }

        /// <summary>
        /// Returns a list of ChatTemplates loaded from the DB.
        /// </summary>
        private List<ChatTemplate> getFilterdChats()
        {
            List<ChatTemplate> list = new List<ChatTemplate>();
            foreach (XMPPClient c in ConnectionHandler.INSTANCE.getClients())
            {
                foreach (ChatTable chat in ChatDBManager.INSTANCE.getAllChatsForClient(c.getXMPPAccount().getIdAndDomain()))
                {
                    ChatTemplate chatElement = new ChatTemplate { chat = chat, client = c };
                    if (chat.chatType == ChatType.MUC)
                    {
                        chatElement.mucInfo = MUCDBManager.INSTANCE.getMUCInfo(chat.id);
                    }
                    list.Add(chatElement);
                }
            }
            return list;
        }

        /// <summary>
        /// Loads all chats and inserts them into the chatsList.
        /// </summary>
        /// <param name="selectedChatId">The id of the chat which should get selected.</param>
        private void loadChats(string selectedChatId)
        {
            // Load all chats:
            Task.Run(() =>
            {
                ChatTemplate selectedChat = null;
                List<ChatTemplate> chats = getFilterdChats();
                for (int i = 0; i < chats.Count; i++)
                {
                    if (string.Equals(selectedChatId, chats[i].chat.id))
                    {
                        selectedChat = chats[i];
                    }
                }

                // Show selected chat:
                Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    // Clear list:
                    CHATS.Clear();

                    // Add chats
                    CHATS.AddRange(chats);
                    if (masterDetail_pnl.SelectedItem == null && selectedChat != null)
                    {
                        masterDetail_pnl.SelectedItem = selectedChat;
                    }
                }).AsTask();
            });
        }

        /// <summary>
        /// Adds a new chat to the chatsList and the DB.
        /// </summary>
        /// <param name="client">Which account/client owns this chat?</param>
        /// <param name="jID">The JID if the new chat.</param>
        /// <param name="addToRoster">Should the chat get added to the users roster?</param>
        /// <param name="requestSubscription">Request a presence subscription?</param>
        private async Task addChatAsync(XMPPClient client, string jID, bool addToRoster, bool requestSubscription)
        {
            if (client == null || jID == null)
            {
                string errorMessage = "Unable to add chat! client ?= " + (client == null) + " jabberId ?=" + (jID == null);
                Logger.Error(errorMessage);
                MessageDialog messageDialog = new MessageDialog("Error")
                {
                    Content = errorMessage
                };
                await messageDialog.ShowAsync();
            }
            else
            {
                if (addToRoster)
                {
                    await client.addToRosterAsync(jID);
                }
                if (requestSubscription)
                {
                    await client.requestPresenceSubscriptionAsync(jID);
                }
                ChatDBManager.INSTANCE.setChat(new ChatTable
                {
                    id = ChatTable.generateId(jID, client.getXMPPAccount().getIdAndDomain()),
                    chatJabberId = jID,
                    userAccountId = client.getXMPPAccount().getIdAndDomain(),
                    ask = null,
                    inRoster = addToRoster,
                    lastActive = DateTime.Now,
                    muted = false,
                    presence = Presence.Unavailable,
                    status = null,
                    subscription = requestSubscription ? "pending" : null
                }, false, true);
            }
        }

        /// <summary>
        /// Filters all chats and only shows those that contain the given string.
        /// </summary>
        /// <param name="s">The string for filtering chats.</param>
        /// <param name="force">Force filtering.</param>
        private void filterChats(string s, bool force)
        {
            if (!force && Equals(s, currentChatQuery))
            {
                return;
            }

            currentChatQuery = s;
            CHATS_ACV.RefreshFilter();
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            loading_grid.Visibility = Visibility.Visible;
            main_grid.Visibility = Visibility.Collapsed;

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            string toastActivationString = null;
            if (e.NavigationMode == NavigationMode.New && e.Parameter is string && ((e.Parameter as string).Equals("App.xaml.cs") || (e.Parameter as string).Equals("AddAccountPage.xaml.cs")))
            {
                await UiUtils.showInitialStartDialogAsync();
                await UiUtils.showWhatsNewDialog();
            }
            else if (e.Parameter is ChatToastActivation chatToastActivation)
            {
                toastActivationString = chatToastActivation.CHAT_ID;
            }
            loadChats(toastActivationString);

            loading_grid.Visibility = Visibility.Collapsed;
            main_grid.Visibility = Visibility.Visible;

            if (e.Parameter is ShowAddMUCNavigationParameter)
            {
                ShowAddMUCNavigationParameter parameter = e.Parameter as ShowAddMUCNavigationParameter;
                AddMUCDialog dialog = new AddMUCDialog(parameter.ROOM_JID);
                await dialog.ShowAsync();
            }
        }

        private void ChatPage2_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (!(Window.Current.Content is Frame rootFrame))
            {
                return;
            }
            if (rootFrame.CanGoBack && e.Handled == false)
            {
                e.Handled = true;
                rootFrame.GoBack();
            }
        }

        private void INSTANCE_ChatChanged(ChatDBManager handler, ChatChangedEventArgs args)
        {
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                // Backup selected chat:
                ChatTemplate selectedChat = null;
                if (masterDetail_pnl.SelectedItem != null && masterDetail_pnl.SelectedItem is ChatTemplate)
                {
                    selectedChat = masterDetail_pnl.SelectedItem as ChatTemplate;
                }

                if (args.REMOVED)
                {
                    CHATS.RemoveId(args.CHAT.id);
                    args.Cancel = true;

                    // Restore selected chat:
                    if (selectedChat != null && !string.Equals(args.CHAT.id, selectedChat.chat.id))
                    {
                        masterDetail_pnl.SelectedItem = selectedChat;
                    }
                    return;
                }
                else
                {
                    if (CHATS.UpdateChat(args.CHAT))
                    {
                        CHATS_ACV.RefreshSorting();
                        args.Cancel = true;
                        // Restore selected chat:
                        if (selectedChat != null)
                        {
                            masterDetail_pnl.SelectedItem = selectedChat;
                        }
                        return;
                    }
                }

                Task.Run(() =>
                {
                    // Add the new chat to the list of chats:
                    foreach (XMPPClient c in ConnectionHandler.INSTANCE.getClients())
                    {
                        if (Equals(args.CHAT.userAccountId, c.getXMPPAccount().getIdAndDomain()))
                        {
                            ChatTemplate chatElement = new ChatTemplate { chat = args.CHAT, client = c };
                            if (args.CHAT.chatType == ChatType.MUC)
                            {
                                chatElement.mucInfo = MUCDBManager.INSTANCE.getMUCInfo(args.CHAT.id);
                            }

                            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                            {
                                CHATS.Add(chatElement);
                                // Restore selected chat:
                                if (selectedChat != null)
                                {
                                    masterDetail_pnl.SelectedItem = selectedChat;
                                }
                            }).AsTask();
                        }
                    }
                });
            }).AsTask();
        }

        private void INSTANCE_MUCInfoChanged(MUCDBManager handler, MUCInfoChangedEventArgs args)
        {
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => CHATS.UpdateMUCInfo(args.MUC_INFO)).AsTask();
        }

        private async void addChat_mfoi_Click(object sender, RoutedEventArgs e)
        {
            AddChatDialog dialog = new AddChatDialog();
            await UiUtils.showDialogAsyncQueue(dialog);
            if (!dialog.cancled)
            {
                await addChatAsync(dialog.client, dialog.jabberId, dialog.addToRoster, dialog.requestSubscription);
            }
        }

        private async void addMUC_mfoi_Click(object sender, RoutedEventArgs e)
        {
            AddMUCDialog dialog = new AddMUCDialog();
            await UiUtils.showDialogAsyncQueue(dialog);
        }

        private void addMIX_mfoi_Click(object sender, RoutedEventArgs e)
        {
            // ToDo Add MIX support.
        }

        private void settings_abb_Click(object sender, RoutedEventArgs e)
        {
            (Window.Current.Content as Frame).Navigate(typeof(SettingsPage));
        }

        private async void masterDetail_pnl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Send active chat state:
            foreach (var added in e.AddedItems)
            {
                if (added is ChatTemplate)
                {
                    ChatTemplate c = added as ChatTemplate;
                    if (shouldSendChatState(c.chat))
                    {
                        await c.client.sendChatStateAsync(c.chat.chatJabberId, XMPP_API.Classes.Network.XML.Messages.XEP_0085.ChatState.ACTIVE);
                    }
                }
            }
            // Send inactive chat state:
            foreach (var added in e.RemovedItems)
            {
                if (added is ChatTemplate)
                {
                    ChatTemplate c = added as ChatTemplate;
                    if (shouldSendChatState(c.chat))
                    {
                        await c.client.sendChatStateAsync(c.chat.chatJabberId, XMPP_API.Classes.Network.XML.Messages.XEP_0085.ChatState.INACTIVE);
                    }
                }
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            UiUtils.setBackgroundImage(backgroundImage_img);

            // Subscribe to chat and MUC info changed events:
            ChatDBManager.INSTANCE.ChatChanged -= INSTANCE_ChatChanged;
            ChatDBManager.INSTANCE.ChatChanged += INSTANCE_ChatChanged;
            MUCDBManager.INSTANCE.MUCInfoChanged -= INSTANCE_MUCInfoChanged;
            MUCDBManager.INSTANCE.MUCInfoChanged += INSTANCE_MUCInfoChanged;
        }

        private void searchChats_asb_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            filterChats(searchChats_asb.Text.ToLower(), false);
        }

        private void searchChats_asb_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            filterChats((args.QueryText ?? searchChats_asb.Text).ToLower(), true);
        }

        private void master_cmdb_Opening(object sender, object e)
        {
            changePresence_apbb.IsEnabled = ConnectionHandler.INSTANCE.getClients().Count > 0;
        }

        private async void changePresence_apbb_Click(object sender, RoutedEventArgs e)
        {
            ChangeAccountPresenceDialog dialog = new ChangeAccountPresenceDialog();
            await UiUtils.showDialogAsyncQueue(dialog);
        }

        private void manageBookmarks_apbb_Click(object sender, RoutedEventArgs e)
        {
            (Window.Current.Content as Frame).Navigate(typeof(ManageBookmarksPage));
        }

        private void filter_abb_Checked(object sender, RoutedEventArgs e)
        {
            filter_stckp.Visibility = Visibility.Visible;
            filterChats(searchChats_asb.Text.ToLower(), false);
        }

        private void filter_abb_Unchecked(object sender, RoutedEventArgs e)
        {
            filter_stckp.Visibility = Visibility.Collapsed;
            filterChats(string.Empty, false);
        }

        private async void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            await UiUtils.onPageSizeChangedAsync(e);
        }

        protected async override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            await UiUtils.onPageNavigatedFromAsync();
        }
        #endregion
    }
}

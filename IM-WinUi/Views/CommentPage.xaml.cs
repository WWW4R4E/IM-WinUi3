using IMWinUi.Models;
using IMWinUi.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Web.WebView2.Core;
using System;
using System.Diagnostics;
using System.Linq;
using Windows.Storage.Pickers;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using IMWinUi.Services;

namespace IMWinUi.Views
{
    public sealed partial class CommentPage
    {
        internal ChatClientService ChatClient;
        internal CommentPageViewModel CommentPageViewModel;

        public CommentPage()
        {
            InitializeComponent();
            InitializeWebView();
            CommentPageViewModel = new CommentPageViewModel();
            ChatClient = Ioc.Default.GetRequiredService<ChatClientService>();
            ChatClient.MessageSent += ChatClient_MessageSent;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var data = e.Parameter;
            if (data != null && data is IMUser user)
            {
                if (CommentPageViewModel.Users.All(x => x.UserName != user.UserName))
                {
                    CommentPageViewModel.Users.Add(user);
                }
                else
                {
                    Console.WriteLine("用户已存在：" + user.UserName);
                }

                UserListBox.SelectedIndex =
                    CommentPageViewModel.Users.IndexOf(
                        CommentPageViewModel.Users.FirstOrDefault(x => x.UserName == user.UserName));
            }
        }


        private async void InitializeWebView()
        {
            await EmojiWebView.EnsureCoreWebView2Async();
            string htmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, "Assets/emoji-picker.html");
            EmojiWebView.Source = new Uri(htmlPath);
            EmojiWebView.CoreWebView2.WebMessageReceived += WebView_WebMessageReceived;
        }

        private void WebView_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            string selectedEmoji = e.TryGetWebMessageAsString();
            ChatInput.Text += selectedEmoji;
        }

        private async void sendTextButton_Click(object sender, RoutedEventArgs e)
        {
            var iMMessage = new IMMessage(MessageType.Text, Properties.Settings.Default.LastUserName,
                CommentPageViewModel.SelectUser.UserName, ChatInput.Text);
            try
            {
                Debug.WriteLine("正在尝试发送消息");
                await ChatClient.SendMessageAsync(iMMessage); // 使用await等待异步操作完成
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"发送消息失败: {ex.Message}");
            }
        }


        // 将新消息添加进聊天记录
        private void ChatClient_MessageSent(object sender, MessageReceiveEventArgs e)
        {
            if (e.Success)
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    GetNewChatMessages(CommentPageViewModel.SelectUser.UserName);
                    ChatInput.Text = string.Empty;
                });
            }
        }


        private async void ImageButton_Click(object sender, RoutedEventArgs e)
        {
            var senderButton = sender as Button;
            if (senderButton != null)
            {
                senderButton.IsEnabled = false;
                var openPicker = new FileOpenPicker();
                var window = (Application.Current as App)?.m_window;
                var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
                WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);

                openPicker.ViewMode = PickerViewMode.Thumbnail;
                openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
                openPicker.FileTypeFilter.Add(".jpg");
                openPicker.FileTypeFilter.Add(".jpeg");
                openPicker.FileTypeFilter.Add(".png");

                var file = await openPicker.PickSingleFileAsync();
                senderButton.IsEnabled = true;

                // TODO 处理选中的文件（这里仅作为示例，未实现具体逻辑）
                if (file != null)
                {
                    Debug.WriteLine($"选择了文件: {file.Name}");
                }
            }
        }

        private void RefreshChatMessages(string user)
        {
            var content = Ioc.Default.GetRequiredService<LocalDbContext>();
            var newMessages = content.GetIMMessages(Properties.Settings.Default.LastUserName, user);

            // 标记消息为已读
            content.MarkMessagesAsRead(newMessages);

            // 更新 ViewModel 中的消息列表
            CommentPageViewModel.Messages = newMessages;
        }


        private void GetNewChatMessages(string user)
        {
            var content = Ioc.Default.GetRequiredService<LocalDbContext>();
            var newMessage = content.GetLatestMessageBetweenUsers(Properties.Settings.Default.LastUserName, user);
            DispatcherQueue.TryEnqueue(() => { CommentPageViewModel.Messages.Add(newMessage); });
        }

        private void ListView_SelectChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListView listView && listView.SelectedItem is CommentListItem item)
            {
                CommentPageViewModel.SelectUser = item.user;
                RefreshChatMessages(CommentPageViewModel.SelectUser.UserName);
            }
        }
    }
}
using IMWinUi.Models;
using IMWinUi.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Web.WebView2.Core;
using System;
using System.Diagnostics;
using System.Linq;
using Windows.Storage.Pickers;
using Microsoft.Extensions.DependencyInjection;

namespace IMWinUi.Views
{
    public sealed partial class CommentPage
    {
        private readonly ChatClientViewModel _chatClient = new();
        internal CommentPageViewModel CommentPageViewModel = new();

        public CommentPage()
        {
            this.InitializeComponent();
            InitializeWebView();
            _chatClient.MessageSent += ChatClient_MessageSent!;
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var data = e.Parameter;
            if (data != null && data is IMUser user)
            {
                if (CommentPageViewModel.Users.All(x => x != user))
                {
                    CommentPageViewModel.Users.Add(user);
                }
                CommentPageViewModel.SelectUser = user;
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
            if (string.IsNullOrEmpty(CommentPageViewModel.SelectUser.Username))
            {
                Debug.WriteLine("请选择一个用户发送消息。");
                return;
            }

            var iMMessage = new IMMessage(MessageType.Text, Properties.Settings.Default.LastUserName, CommentPageViewModel.SelectUser.Username, ChatInput.Text);
            try
            {
                Debug.WriteLine("正在尝试发送消息");
                await _chatClient.SendMessageAsync(iMMessage); // 使用await等待异步操作完成
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"发送消息失败: {ex.Message}");
            }
        }


        // 刷新当前用户的聊天记录
        private void ChatClient_MessageSent(object sender, MessageSentEventArgs e)
        {
            if (e.Success)
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    GetNewChatMessages(CommentPageViewModel.SelectUser.Username);
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

                // 处理选中的文件（这里仅作为示例，未实现具体逻辑）
                if (file != null)
                {
                    Debug.WriteLine($"选择了文件: {file.Name}");
                }
            }
        }

        private void RefreshChatMessages(string user)
        {
            var content = App.ServiceProvider.GetRequiredService<LocalDbcontext>();
                var newMessages = content.GetIMMessages(Properties.Settings.Default.LastUserName, user);
                CommentPageViewModel.Messages = newMessages;
                Debug.WriteLine(newMessages.Count());
 
        }

        private void GetNewChatMessages(string user)
        {
            var content = App.ServiceProvider.GetRequiredService<LocalDbcontext>();
                var newMessage = content.GetLatestMessageBetweenUsers(Properties.Settings.Default.LastUserName, user);
                CommentPageViewModel.Messages.Add(newMessage);
        }

        private void ListView_SelectChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListView listView && listView.SelectedItem is CommentListItem item)
            {
                Debug.WriteLine(2);
                CommentPageViewModel.SelectUser = item.user;
                Debug.WriteLine(item.user.Username);
                RefreshChatMessages(CommentPageViewModel.SelectUser.Username);
            }
        }
    }
}
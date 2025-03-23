using IMWinUi.Models;
using IMWinUi.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Windows.Storage.Pickers;

namespace IMWinUi.Views
{
    public sealed partial class CommentPage : Page
    {
        private ChatClientViewModel chatClient = new ChatClientViewModel();
        internal CommentPageViewModel CommentPageViewModel = new CommentPageViewModel();

        public CommentPage()
        {
            this.InitializeComponent();
            InitializeWebView();
            chatClient.MessageSent += ChatClient_MessageSent;
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var data = e.Parameter;
            if (data != null && data is IMUser user)
            {
                if (!CommentPageViewModel.Users.Any(x => x == user))
                {
                    CommentPageViewModel.Users.Add(user);
                }
                CommentPageViewModel.SelectUser = CommentPageViewModel.Users.FirstOrDefault(x => x == user);
            }
        }
        private async void InitializeWebView()
        {
            await emojiWebView.EnsureCoreWebView2Async();
            string htmlPath = System.IO.Path.Combine(System.AppContext.BaseDirectory, "Assets/emoji-picker.html");
            emojiWebView.Source = new System.Uri(htmlPath);
            emojiWebView.CoreWebView2.WebMessageReceived += WebView_WebMessageReceived;
        }

        private void WebView_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            string selectedEmoji = e.TryGetWebMessageAsString();
            chatInput.Text += selectedEmoji;
        }

        private async void sendTextButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(CommentPageViewModel.SelectUser.Username))
            {
                Debug.WriteLine("请选择一个用户发送消息。");
                return;
            }

            var iMMessage = new IMMessage(MessageType.Text, Properties.Settings.Default.LastUserName, CommentPageViewModel.SelectUser.Username, chatInput.Text);
            try
            {
                Debug.WriteLine("正在尝试发送消息");
                await chatClient.SendMessageAsync(iMMessage); // 使用await等待异步操作完成
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"发送消息失败: {ex.Message}");
            }
        }
        private void ChatClient_MessageSent(object sender, MessageSentEventArgs e)
        {
            if (e.Success)
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    RefreshChatMessages(CommentPageViewModel.SelectUser.Username);
                    chatInput.Text = string.Empty;
                });

            }
        }

        private async void ImageButton_Click(object sender, RoutedEventArgs e)
        {
            var senderButton = sender as Button;
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

        private void ListBox_SelectChanged(object sender, RoutedEventArgs e)
        {
            if (userListBox.SelectedItem == null) return;

            var newUser = ((IMUser)userListBox.SelectedItem).Username;
            if (CommentPageViewModel.SelectUser.Username != newUser)
            {
                CommentPageViewModel.SelectUser.Username = newUser;
                RefreshChatMessages(CommentPageViewModel.SelectUser.Username);
            }
        }

        private void RefreshChatMessages(string user)
        {
            if (CommentPageViewModel.Messages != null)
            {
                CommentPageViewModel.Messages.Clear();
            }

            var db = new LocalDbcontext();
            var newMessages = db.GetIMMessages(Properties.Settings.Default.LastUserName, user);
            if (newMessages != null)
            {
                foreach (var message in newMessages)
                {
                    CommentPageViewModel.Messages.Add(message);
                }
            }
        }
    }
}
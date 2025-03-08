using IMWinUi.Models;
using IMWinUi.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
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
        private ObservableCollection<IMMessage> Messages;
        private List<IMUser> Users;
        private string selectUser;
        private ChatClientViewModel chatClient = new ChatClientViewModel();

        public CommentPage()
        {
            Users = new List<IMUser>
            {
                new IMUser(1, "张三"),
                new IMUser(2, "王五")
            };

            this.InitializeComponent();
            InitializeWebView();
            Messages = new ObservableCollection<IMMessage>(); // 初始化Messages集合
            chatClient.MessageSent += ChatClient_MessageSent; // 订阅发送成功事件

 
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
            if (string.IsNullOrEmpty(selectUser))
            {
                Debug.WriteLine("请选择一个用户发送消息。");
                return;
            }

            var iMMessage = new IMMessage(MessageType.Text, Properties.Settings.Default.LastUserName, selectUser, chatInput.Text);
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


        // 刷新当前用户的聊天记录
        private void ChatClient_MessageSent(object sender, MessageSentEventArgs e)
        {
            if (e.Success)
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    RefreshChatMessages(selectUser);
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
            if (selectUser != newUser)
            {
                selectUser = newUser;
                RefreshChatMessages(selectUser);
            }
        }

        private void RefreshChatMessages(string user)
        {
            if (Messages != null)
            {
                Messages.Clear();
            }

            var db = new LocalDbcontext();
            var newMessages = db.GetIMMessages(Properties.Settings.Default.LastUserName, user);
            if (newMessages != null)
            {
                foreach (var message in newMessages)
                {
                    Messages.Add(message);
                }
            }
        }
    }
}
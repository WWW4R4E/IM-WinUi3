using IMWinUi.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace IMWinUi.ViewModels
{
    internal class CommentPageViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<IMMessage> _messages;
        private List<IMUser> _users;
        private string _selectUser;
        private string _chatInput;

        public ObservableCollection<IMMessage> Messages
        {
            get => _messages;
            set
            {
                _messages = value;
                OnPropertyChanged();
            }
        }

        public List<IMUser> Users
        {
            get => _users;
            set
            {
                _users = value;
                OnPropertyChanged();
            }
        }

        public string SelectUser
        {
            get => _selectUser;
            set
            {
                _selectUser = value;
                OnPropertyChanged();
            }
        }

        public string ChatInput
        {
            get => _chatInput;
            set
            {
                _chatInput = value;
                OnPropertyChanged();
            }
        }

        public ChatClientViewModel ChatClient { get; }

        public CommentPageViewModel()
        {
            ChatClient = new ChatClientViewModel();
            Users = new List<IMUser>
            {
                new IMUser(1, "张三"),
                new IMUser(2, "王五")
            };
            Messages = new ObservableCollection<IMMessage>();
        }

        public async Task SendMessageAsync()
        {
            var iMMessage = new IMMessage(MessageType.Text, Properties.Settings.Default.LastUserName, SelectUser, ChatInput);
            try
            {
                Debug.WriteLine("正在尝试发送消息");
                await ChatClient.SendMessageAsync(iMMessage);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"发送消息失败: {ex.Message}");
            }
        }

        public void RefreshChatMessages(string user)
        {
            Messages.Clear();

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

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
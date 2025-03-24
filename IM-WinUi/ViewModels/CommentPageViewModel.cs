using System.Collections.Generic;
using IMWinUi.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;

namespace IMWinUi.ViewModels
{
    internal class CommentPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private ObservableCollection<IMMessage> _messages;
        private ObservableCollection<IMUser> _users;
        private ObservableCollection<CommentListItem> commentLists;
        private IMUser _selectUser;
        private string _chatInput;

        public CommentPageViewModel()
        {
            var context = App.ServiceProvider.GetRequiredService<LocalDbcontext>();
            {
                Users = context.GetHistoryImUsers();
            }
            commentLists = new ObservableCollection<CommentListItem>();
            InitializeCommentLists();
        }

        public ObservableCollection<IMUser> Users
        {
            get => _users;
            set
            {
                if (_users != value)
                {
                    _users = value;
                    OnPropertyChanged();
                    InitializeCommentLists();
                }
            }
        }

        public ObservableCollection<IMMessage> Messages
        {
            get => _messages;
            set
            {
                if (_messages != value)
                {
                    _messages = value;
                    OnPropertyChanged();
                    InitializeCommentLists();
                }
            }
        }

        public string ChatInput
        {
            get => _chatInput;
            set
            {
                if (_chatInput != value)
                {
                    _chatInput = value;
                    OnPropertyChanged();
                }
            }
        }

        public IMUser SelectUser
        {
            get => _selectUser;
            set
            {
                if (_selectUser != value)
                {
                    _selectUser = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<CommentListItem> CommentLists
        {
            get => commentLists;
            set
            {
                if (commentLists != value)
                {
                    commentLists = value;
                    OnPropertyChanged();
                }
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void InitializeCommentLists()
        {
            // 确保 commentLists 已初始化
            commentLists ??= new ObservableCollection<CommentListItem>();
            commentLists.Clear();

            // 获取当前用户（避免空值）
            var LoginUser = Properties.Settings.Default.LastUserName ?? string.Empty;
            // 新增双重空值检查
            if (string.IsNullOrEmpty(LoginUser) )
            {
                Debug.Write("用户或用户名为空");
                return;
            }

            var dbContext = App.ServiceProvider.GetRequiredService<LocalDbcontext>().IMMessages;
            if (dbContext == null)
            {
                Debug.Write("数据库消息为空");
                return;
            }

  
           foreach (var user in Users)  
           {  
               // 新增：为每个用户单独查询  
               var latestMessage = dbContext  
                   .Where(m =>  
                       (m.SenderName == LoginUser && m.ReceiverName == user.Username) ||  
                       (m.SenderName == user.Username && m.ReceiverName == LoginUser)  
                   )  
                   .OrderByDescending(msg => msg.SentAt)  
                   .FirstOrDefault();  

               if (latestMessage != null)  
                   commentLists.Add(new CommentListItem { user = user, Message = latestMessage });  
           }  
   
            Debug.WriteLine("里面有"+commentLists.Count());
        }
    }
}
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using IMWinUi.Models;

namespace IMWinUi.ViewModels
{
    internal partial class CommentPageViewModel : ObservableObject
    {
        [ObservableProperty] private ObservableCollection<IMUser> _users;

        [ObservableProperty] private ObservableCollection<IMMessage> _messages;

        [ObservableProperty] private string _chatInput;

        [ObservableProperty] private IMUser _selectUser;

        [ObservableProperty] private ObservableCollection<CommentListItem> _commentLists = new();

        public CommentPageViewModel()
        {
            var content = Ioc.Default.GetRequiredService<LocalDbContext>();
            Users = content.GetHistoryImUsers();
            InitializeCommentLists();
        }

        partial void OnUsersChanged(ObservableCollection<IMUser> value)
        {
            InitializeCommentLists();
        }


        private void InitializeCommentLists()
        {
            CommentLists.Clear();

            var loginUser = Properties.Settings.Default.LastUserName ?? string.Empty;
            if (string.IsNullOrEmpty(loginUser))
            {
                Debug.Write("用户或用户名为空");
                return;
            }

            var content = Ioc.Default.GetRequiredService<LocalDbContext>();
            var message = content.IMMessages;
            foreach (var user in Users)
            {
                var latestMessage = message
                    .Find(m =>
                        (m.SenderName == loginUser && m.ReceiverName == user.UserName) ||
                        (m.SenderName == user.UserName && m.ReceiverName == loginUser)
                    )
                    .OrderByDescending(msg => msg.SentAt)
                    .FirstOrDefault();

                if (latestMessage != null)
                {
                    CommentLists.Add(new CommentListItem { user = user, Message = latestMessage });
                }
            }
        }
    }
}
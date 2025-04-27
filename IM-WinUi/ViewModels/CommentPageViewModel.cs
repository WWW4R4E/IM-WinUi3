using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using IMWinUi.Models;

namespace IMWinUi.ViewModels
{
    internal partial class CommentPageViewModel : ObservableObject
    {
        [ObservableProperty] private ObservableCollection<LocalUser> _users;

        [ObservableProperty] private ObservableCollection<LocalMessage> _messages;

        [ObservableProperty] private string _chatInput;

        [ObservableProperty] private LocalUser _selectUser;
        [ObservableProperty] private ObservableCollection<CommentListItem> _commentLists;

        public CommentPageViewModel()
        {
            var content = Ioc.Default.GetRequiredService<LocalDbContext>();
            Users = content.GetHistoryUsers();
            InitializeCommentLists();
        }

        partial void OnUsersChanged(ObservableCollection<LocalUser> value)
        {
            InitializeCommentLists();
        }


        private void InitializeCommentLists()
        {
            if (CommentLists == null)
            {
                CommentLists = new ObservableCollection<CommentListItem>();
            }
            else
            {
                CommentLists.Clear();
            }
            var loginUserId = Properties.Settings.Default.LastUserId;

            var content = Ioc.Default.GetRequiredService<LocalDbContext>();
            var message = content.Messages;
            foreach (var user in Users)
            {
                var latestMessage = message
                    .Find(m =>
                        (m.SenderId == loginUserId && m.ReceiverId == user.UserId) ||
                        (m.SenderId == user.UserId && m.ReceiverId == loginUserId)
                    )
                    .OrderByDescending(msg => msg.SendTime)
                    .FirstOrDefault();

                if (latestMessage != null)
                {
                    CommentLists.Add(new CommentListItem { user = user, Message = latestMessage });
                }
            }
        }
    }
}
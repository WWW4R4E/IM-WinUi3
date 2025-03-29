using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using IMWinUi.Models;
using IMWinUi.Services;

namespace IMWinUi.ViewModels
{
    internal partial class MainWindowVIewModel : ObservableObject
    {
        [ObservableProperty]
        private int _commentBadgeCount = 0;
        [ObservableProperty]
        private int _contactBadgeCount = 0;
        [ObservableProperty]
        private int _favoriteBadgeCount = 0;

        internal MainWindowVIewModel()
        {
            _commentBadgeCount = GetNewMessageCount();
            _contactBadgeCount = 0;
            _favoriteBadgeCount = 0;


            var chatClientViewModel = Ioc.Default.GetService<ChatClientService>();
            chatClientViewModel.MessageSent += (sender, e) =>
            {
                CommentBadgeCount = GetNewMessageCount();
            };
        }

        private int GetNewMessageCount()
        {
            var content = Ioc.Default.GetRequiredService<LocalDbcontext>();
            var newMessageCount = content.GetNewMessageCount(Properties.Settings.Default.LastUserName);
            return newMessageCount;
        }
    }
}

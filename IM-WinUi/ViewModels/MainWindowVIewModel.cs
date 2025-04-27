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
            
            Ioc.Default.GetService<ChatClientService>()!.MessageSent += (sender, e) =>
            {
                CommentBadgeCount = GetNewMessageCount();
            };
        }

        private int GetNewMessageCount()
        {
            return  Ioc.Default.GetRequiredService<LocalDbContext>().GetNewMessageCount();
        }
    }
}

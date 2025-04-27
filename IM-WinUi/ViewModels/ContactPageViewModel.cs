using CommunityToolkit.Mvvm.ComponentModel; 
using IMWinUi.Models;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.DependencyInjection;

namespace IMWinUi.ViewModels
{
    internal partial class ContactPageViewModel : ObservableObject 
    {
        [ObservableProperty] 
        private LocalUser? _user; 
        [ObservableProperty] 
        private ObservableCollection<LocalUser> _friends; 
        
        internal static ObservableCollection<GroupInfoList> GetContactsGroupedAsync()
        {
            var content = Ioc.Default.GetRequiredService<LocalDbContext>();

            var friendUsers = content.GetUsers();
            
            var query = from item in friendUsers
                        group item by item.Username.Substring(0, 1).ToUpper() 
                        into g
                        orderby g.Key 
                        select new GroupInfoList(g) { Key = g.Key };

            return new ObservableCollection<GroupInfoList>(query); 
        }

    }
}

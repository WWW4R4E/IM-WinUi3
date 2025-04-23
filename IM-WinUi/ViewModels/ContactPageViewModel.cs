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
        private IMUser? _user; 
        [ObservableProperty] 
        private ObservableCollection<IMUser> _friends; 
        
        internal static ObservableCollection<GroupInfoList> GetContactsGroupedAsync()
        {
            var content = Ioc.Default.GetRequiredService<LocalDbContext>(); 
            
            var contacts = content.GetImUsers();
            
            var query = from item in contacts
                        group item by item.UserName.Substring(0, 1).ToUpper() 
                        into g
                        orderby g.Key 
                        select new GroupInfoList(g) { Key = g.Key };

            return new ObservableCollection<GroupInfoList>(query); 
        }

    }
}

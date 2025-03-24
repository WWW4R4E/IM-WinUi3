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

        #region 联系人分组方法
        internal static ObservableCollection<GroupInfoList> GetContactsGroupedAsync()
        {
            var content = Ioc.Default.GetRequiredService<LocalDbcontext>(); 
            
            var contacts = content.GetIMUsers();
            
            var query = from item in contacts
                        group item by item.Username.Substring(0, 1).ToUpper() 
                        into g
                        orderby g.Key 
                        select new GroupInfoList(g) { Key = g.Key };

            return new ObservableCollection<GroupInfoList>(query); 
        }
        #endregion

    }
}

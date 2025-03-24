using IMWinUi.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;

namespace IMWinUi.ViewModels;

internal class ContactPageViewModel:INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    internal List<IMUser> Friends;
    private IMUser? _user;
    internal IMUser? User {
        get => _user;
        set
        {
            if (_user != value)
            {
                _user = value;
                OnPropertyChanged();
            }
        }
    }

    internal static ObservableCollection<GroupInfoList> GetContactsGroupedAsync()
    {
        var content = App.ServiceProvider.GetRequiredService<LocalDbcontext>();
        // 从数据库获取联系人列表
        var contacts = content.GetIMUsers();

        // 按用户名首字母分组
        var query = from item in contacts
            group item by item.Username.Substring(0, 1).ToUpper()
            into g
            orderby g.Key
            select new GroupInfoList(g) { Key = g.Key };

        return new ObservableCollection<GroupInfoList>(query);
}


    public void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
using IMWinUi.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.System;

namespace IMWinUi.ViewModels;

internal class ContactPageViewModel:INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    internal List<IMUser> Friends;
    private IMUser _user;
    internal IMUser User {
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
        var db = new LocalDbcontext();

        // 从数据库获取联系人列表
        var contacts = db.GetIMUsers();

        // 如果数据库中没有数据，创建一些默认数据
        if (contacts == null || !contacts.Any())
        {
            contacts = CreateDefaultContacts();
        }

        // 按用户名首字母分组
        var query = from item in contacts
                    group item by item.Username.Substring(0, 1).ToUpper() into g
                    orderby g.Key
                    select new GroupInfoList(g) { Key = g.Key };

        return new ObservableCollection<GroupInfoList>(query);
    }

    // 创建默认联系人列表
    private static List<IMUser> CreateDefaultContacts()
    {
        return new List<IMUser>
    {
        new IMUser { Username = "Alice" },
        new IMUser { Username = "Bob" },
        new IMUser { Username = "Charlie" },
        new IMUser { Username = "David" },
        new IMUser { Username = "Eve" }
    };
    }
    public void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.DependencyInjection;
using IMWinUi.Models;
using Microsoft.UI.Xaml.Data;

namespace IMWinUi.Converter
{
    public class UserIdToUsernameConverter : IValueConverter
    {
        // 缓存用户ID到用户名的映射（减少重复查询）
        private readonly Dictionary<long, string> _userCache = new();
        LocalDbContext context = Ioc.Default.GetService<LocalDbContext>();

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is not long userId)
                return "未知用户";

            // 优先从缓存获取
            if (_userCache.TryGetValue(userId, out string cachedName))
                return cachedName;
            
            var user = context.Users.FindOne(x => x.UserId == userId);
            if (user != null)
            {
                _userCache[userId] = user.Username;
                return user.RemarkName ?? user.Username;
            }
            return "用户不存在";
        }

        // 反向转换接口
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace IMWinUi.Models
{
    internal class LocalDbcontext : DbContext
    {
        public DbSet<IMUser> IMUsers { get; set; } // 本地用户表
        public DbSet<IMMessage> IMMessages { get; set; } // 本地消息表
        
        // 添加接受DbContextOptions的构造函数
        public LocalDbcontext(DbContextOptions<LocalDbcontext> options) : base(options) { }

        // 保留无参构造函数（可选）
        //public LocalDbcontext() {}

        // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        // {
        //     var projectDirectory = AppDomain.CurrentDomain.BaseDirectory;
        //     var dbPath = System.IO.Path.Combine(projectDirectory, "localMIContext.db");
        //     optionsBuilder.UseSqlite($"Data Source={dbPath}"); 
        // }

        internal void CreateMessage(IMMessage iMMessage)
        {
            iMMessage.MessageId = 0;
            IMMessages.Add(iMMessage);
            SaveChanges();
        }
        internal void CreateUser(IMUser iMUser)
        {
            IMUsers.Add(iMUser);
            SaveChanges();
        }

        internal ObservableCollection<IMMessage> GetIMMessages(string lastUserName, string selectUser)
        {
            // 查询两个用户之间的消息
            var messages = IMMessages
                .Where(m => (m.SenderName == lastUserName && m.ReceiverName == selectUser) ||
                            (m.SenderName == selectUser && m.ReceiverName == lastUserName))
                .OrderBy(m => m.SentAt)
                .ToList();
            return new ObservableCollection<IMMessage>(messages);
        }
        internal IMMessage GetLatestMessageBetweenUsers(string lastUserName, string selectUser)
        {
            // 查询两个用户之间的最新消息
            var latestMessage = IMMessages
                .Where(m => (m.SenderName == lastUserName && m.ReceiverName == selectUser) ||
                            (m.SenderName == selectUser && m.ReceiverName == lastUserName))
                .OrderByDescending(m => m.SentAt) // 按时间降序排列
                .FirstOrDefault(); // 获取最新的一条消息

            return latestMessage;
        }

        
        internal ObservableCollection<IMUser> GetHistoryImUsers()
        {
            // 1. 分别获取发送者和接收者的用户名，合并后去重
            var senderNames = IMMessages.Select(m => m.SenderName);
            var receiverNames = IMMessages.Select(m => m.ReceiverName);
            var userNames = senderNames.Union(receiverNames).Distinct().ToList();

            // 2. 查询用户表
            var users = IMUsers
                .Where(user => userNames.Contains(user.Username))
                .ToList();

            return new ObservableCollection<IMUser>(users);
        }

        internal List<IMUser> GetIMUsers()
        {
            var users = IMUsers
                .ToList();
            return users;
        }
    }
}

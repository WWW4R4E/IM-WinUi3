using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using LiteDB;

namespace IMWinUi.Models
{
    internal class LocalDbContext
    {
        private readonly LiteDatabase _db;
        public LiteCollection<IMUser> IMUsers { get; }
        public LiteCollection<IMMessage> IMMessages { get; }

        // 构造函数：初始化 LiteDB 数据库
        public LocalDbContext()
        {
            // var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "localMIContext.db");
            var dbPath = Path.Combine("C:\\Users\\123\\Desktop\\IM-WinUi3\\IM-WinUi", "localMIContext.db");
            Console.WriteLine("dbPath:" + dbPath);
            _db = new LiteDatabase(dbPath);

            // 初始化集合
            IMUsers = (LiteCollection<IMUser>?)_db.GetCollection<IMUser>("IMUsers");
            IMMessages = (LiteCollection<IMMessage>?)_db.GetCollection<IMMessage>("IMMessages");

            // 确保主键索引
            IMUsers.EnsureIndex(x => x.UserName);
            IMMessages.EnsureIndex(x => x.MessageId, true); // 主键唯一索引
        }

        internal void CreateMessage(IMMessage iMMessage)
        {
            iMMessage.MessageId = 0; // LiteDB 自动分配主键
            IMMessages.Insert(iMMessage);
        }


        internal void MarkMessagesAsRead(IEnumerable<IMMessage> messages)
        {
            // 将所有消息标记为已读
            var updatedMessages = messages.Select(message =>
            {
                message.IsRead = true;
                return message;
            }).ToList();

            // 批量更新
            IMMessages.Update(updatedMessages);
        }


        internal void AddUser(IMUser iMUser)
        {
            IMUsers.Insert(iMUser);
        }

        internal ObservableCollection<IMMessage> GetIMMessages(string lastUserName, string selectUser)
        {
            var messages = IMMessages
                .Find(m => (m.SenderName == lastUserName && m.ReceiverName == selectUser) ||
                           (m.SenderName == selectUser && m.ReceiverName == lastUserName))
                .OrderBy(m => m.SentAt)
                .ToList();
            return new ObservableCollection<IMMessage>(messages);
        }

        internal IMMessage GetLatestMessageBetweenUsers(string lastUserName, string selectUser)
        {
            var latestMessage = IMMessages
                .Find(m => (m.SenderName == lastUserName && m.ReceiverName == selectUser) ||
                           (m.SenderName == selectUser && m.ReceiverName == lastUserName))
                .OrderByDescending(m => m.SentAt)
                .FirstOrDefault();
            return latestMessage;
        }

        internal ObservableCollection<IMUser> GetHistoryImUsers()
        {
            var senderNames = IMMessages.FindAll().Select(m => m.SenderName);
            var receiverNames = IMMessages.FindAll().Select(m => m.ReceiverName);
            var userNames = senderNames.Union(receiverNames).Distinct().ToList();

            var users = IMUsers
                .Find(user => userNames.Contains(user.UserName))
                .ToList();

            return new ObservableCollection<IMUser>(users);
        }

        internal List<IMUser> GetImUsers()
        {
            return IMUsers.FindAll().ToList();
        }

        internal int GetNewMessageCount(string lastUserName)
        {
            return IMMessages.Count(m => (m.SenderName == lastUserName || m.ReceiverName == lastUserName) && !m.IsRead);
        }

        public void UpdateImMessages(List<IMMessage> update1)
        {
            foreach (var message in update1)
            {
                IMMessages.Update(message);
            }
        }

        public void UpdateImUsers(List<IMUser> update2)
        {
            foreach (var user in update2)
            {
                IMUsers.Update(user);
            }
        }
    }
}

// namespace IMWinUi.Models
// {
//     internal class LocalDbcontext : DbContext
//     {
//         public DbSet<IMUser> IMUsers { get; set; } // 本地用户表
//         public DbSet<IMMessage> IMMessages { get; set; } // 本地消息表
//         public DbSet<FavoriteItem> FavoriteItems { get; set; } //本地信息表
//
//         // 添加接受DbContextOptions的构造函数
//         public LocalDbcontext(DbContextOptions<LocalDbcontext> options) : base(options)
//         {
//         }
//
//         internal void CreateMessage(IMMessage iMMessage)
//         {
//             iMMessage.MessageId = 0;
//             IMMessages.Add(iMMessage);
//             SaveChanges();
//         }
//
//         internal void CreateUser(IMUser iMUser)
//         {
//             IMUsers.Add(iMUser);
//             SaveChanges();
//         }
//
//         internal ObservableCollection<IMMessage> GetIMMessages(string lastUserName, string selectUser)
//         {
//             // 查询两个用户之间的消息
//             var messages = IMMessages
//                 .Where(m => (m.SenderName == lastUserName && m.ReceiverName == selectUser) ||
//                             (m.SenderName == selectUser && m.ReceiverName == lastUserName))
//                 .OrderBy(m => m.SentAt)
//                 .ToList();
//             return new ObservableCollection<IMMessage>(messages);
//         }
//
//         internal IMMessage GetLatestMessageBetweenUsers(string lastUserName, string selectUser)
//         {
//             // 查询两个用户之间的最新消息
//             var latestMessage = IMMessages
//                 .Where(m => (m.SenderName == lastUserName && m.ReceiverName == selectUser) ||
//                             (m.SenderName == selectUser && m.ReceiverName == lastUserName))
//                 .OrderByDescending(m => m.SentAt) // 按时间降序排列
//                 .FirstOrDefault(); // 获取最新的一条消息
//
//             return latestMessage;
//         }
//
//
//         internal ObservableCollection<IMUser> GetHistoryImUsers()
//         {
//             // 1. 分别获取发送者和接收者的用户名，合并后去重
//             var senderNames = IMMessages.Select(m => m.SenderName);
//             var receiverNames = IMMessages.Select(m => m.ReceiverName);
//             var userNames = senderNames.Union(receiverNames).Distinct().ToList();
//
//             // 2. 查询用户表
//             var users = IMUsers
//                 .Where(user => userNames.Contains(user.Username))
//                 .ToList();
//
//             return new ObservableCollection<IMUser>(users);
//         }
//
//         internal List<IMUser> GetIMUsers()
//         {
//             var users = IMUsers
//                 .ToList();
//             return users;
//         }
//
//         internal int GetNewMessageCount(string lastUserName)
//         {
//             var result = IMMessages
//                 .Count(m => (m.SenderName == lastUserName | m.ReceiverName == lastUserName) && m.IsRead == false);
//             return result;
//         }
//
//         internal ObservableCollection<FavoriteItem> GetFavoriteItems()
//         {
//             var favoriteItems = FavoriteItems.ToList();
//             return new ObservableCollection<FavoriteItem>(favoriteItems);
//         }
//         
//         internal bool AddFavoriteItems(string Text, List<string> ImageUrls)
//         {
//             try
//             {
//                 FavoriteItems.Add(new FavoriteItem
//                     (Text, ImageUrls)
//                     {
//                         Text = Text,
//                         ImageUrls = ImageUrls
//                     });
//             }
//             catch (Exception ex)
//             {
//                 Debug.WriteLine(ex.Message);
//             }
//         
//             return false;
//         }
//     }
// }
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using LiteDB;

namespace IMWinUi.Models
{
    public class LocalDbContext
    {
        private string _loginId;
        private LiteDatabase _database;

        #region 集合

        public ILiteCollection<LocalUser> Users => _database.GetCollection<LocalUser>("users");
        public ILiteCollection<LocalGroup> Groups => _database.GetCollection<LocalGroup>("groups");

        public ILiteCollection<LocalGroupMember> GroupMembers =>
            _database.GetCollection<LocalGroupMember>("groupmembers");

        public ILiteCollection<LocalMessage> Messages => _database.GetCollection<LocalMessage>("messages");
        public ILiteCollection<Notification> Notifications => _database.GetCollection<Notification>("notifications");

        #endregion


        public LocalDbContext()
        {
        }

        public void InitializeDatabase(string userId)
        {
            _loginId = userId;
            // 根据用户 ID 创建对应的文件夹路径
            string userFolderPath = Path.Combine("UserData", _loginId);
            Directory.CreateDirectory(userFolderPath);

            // 数据库文件路径
            string dbFilePath = Path.Combine(userFolderPath, "user_data.db");

            // 初始化 LiteDB 数据库
            _database = new LiteDatabase(dbFilePath);
        }

        // 全量同步模板
        public void Sync<T>(ILiteCollection<T> collection, List<T>? items) where T : class
        {
            try
            {
                // 开始事务
                _database.BeginTrans();

                // 执行操作
                collection.DeleteAll();
                if (items?.Any() ?? false)
                    collection.InsertBulk(items);

                // 提交事务
                _database.Commit();
            }
            catch (Exception)
            {
                // 发生异常时回滚事务
                _database.Rollback();
                throw; // 重新抛出异常供调用方处理
            }
        }


        // 同步用户信息
        public void SyncUsers(List<LocalUser>? users)
        {
            Sync(Users, users);
        }
        
        // 同步群组信息
        public void SyncMessages(List<LocalMessage> messages)
        {
            Sync(Messages, messages);
        }

        // 同步群组列表
        public void SyncGroups(List<LocalGroup>? groups)
        {
            Sync(Groups, groups);
        }

        // 同步群组成员列表
        public void SyncGroupMembers(long groupId, List<LocalGroupMember>? members)
        {
            Sync(GroupMembers, members);
        }

        // 添加消息到本地数据库
        public void AddMessages(List<LocalMessage> messages)
        {
            foreach (var message in messages)
            {
                Messages.Insert(message);
            }
        }

        // 添加通知到本地数据库
        public void AddNotifications(List<Notification> notifications)
        {
            foreach (var notification in notifications)
            {
                Notifications.Insert(notification);
            }
        }

        // 获取未读消息
        public List<LocalMessage> GetUnreadMessages()
        {
            return Messages.Find(m => m.IsRead == false).ToList();
        }

        // 获取未读通知
        public List<Notification> GetUnreadNotifications()
        {
            return Notifications.Find(n => n.IsRead == false).ToList();
        }

        // 标记消息为已读
        public void MarkMessageAsRead(long messageId)
        {
            var message = Messages.FindById(messageId);
            if (message != null)
            {
                message.IsRead = true;
                Messages.Update(message);
            }
        }

        // 标记通知为已读
        public void MarkNotificationAsRead(int notificationId)
        {
            var notification = Notifications.FindById(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                Notifications.Update(notification);
            }
        }

        // 获取联系人列表
        public List<LocalUser> GetUsers()
        {
            return Users.FindAll().ToList();
        }

        // 获取群组列表
        public List<LocalGroup> GetGroups()
        {
            return Groups.FindAll().ToList();
        }

        // 获取群组成员列表
        public List<LocalGroupMember> GetGroupMembers(long groupId)
        {
            return GroupMembers.Find(m => m.GroupId == groupId).ToList();
        }

        // 获取消息列表
        public List<LocalMessage> GetMessages(long? receiverId = null, long? groupId = null)
        {
            if (receiverId.HasValue)
            {
                return Messages.Find(m => m.ReceiverId == receiverId).ToList();
            }
            else if (groupId.HasValue)
            {
                return Messages.Find(m => m.GroupId == groupId).ToList();
            }
            else
            {
                return Messages.FindAll().ToList();
            }
        }

        // 获取通知列表
        public List<Notification> GetNotifications()
        {
            return Notifications.FindAll().ToList();
        }

        // 获取用户信息
        public LocalUser GetUser(long userId)
        {
            return Users.FindOne(x => x.UserId == userId);
        }

        // 更新用户状态
        public void UpdateUserStatus(long userId, int status)
        {
            var user = Users.FindOne(x => x.UserId == userId);
            if (user != null)
            {
                user.Status = status;
                Users.Update(user);
            }
        }
        

        public ObservableCollection<LocalUser> GetHistoryUsers()
        {
            var senderIds = Messages.FindAll().Select(m => m.SenderId);
            var receiverIds = Messages.FindAll().Select(m => m.ReceiverId);
            var userIds = senderIds.Union<long>(receiverIds).Distinct().ToList();

            var users = Users.Find(u => userIds.Contains(u.UserId)).ToList();
            
            return new ObservableCollection<LocalUser>(users);
        }
        
        
        // 实现 IDisposable 接口，确保正确释放资源
        public void Dispose()
        {
            _database.Dispose();
        }

        public int GetNewMessageCount()
        {
            long defaultLastUserId = Properties.Settings.Default.LastUserId;
            return Messages.Count(m => (m.SenderId == defaultLastUserId || m.ReceiverId == defaultLastUserId) && !m.IsRead);
        }

        public LocalMessage GetLatestMessageBetweenUsers(long defaultLastUserId, long userId)
        {
            var latestMessage = Messages
                .Find(m => (m.SenderId == defaultLastUserId && m.ReceiverId == userId) ||
                           (m.SenderId == userId && m.ReceiverId == defaultLastUserId))
                .OrderByDescending(m => m.SendTime)
                .FirstOrDefault();
            return latestMessage;
        }
    }
}
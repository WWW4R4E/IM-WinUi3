using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace IMWinUi.Models
{
    internal class LocalDbcontext : DbContext
    {
        public DbSet<IMUser> IMUsers { get; set; } // 本地用户表
        public DbSet<IMMessage> IMMessages { get; set; } // 本地消息表

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var projectDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var dbPath = System.IO.Path.Combine(projectDirectory, "localMIContext.db");
            optionsBuilder.UseSqlite($"Data Source={dbPath}"); 
        }

        internal void CreateMessage(IMMessage iMMessage)
        {
            IMMessages.Add(iMMessage);
            SaveChanges();
        }
        internal void CreateUser(IMUser iMUser)
        {
            IMUsers.Add(iMUser);
            SaveChanges();
        }

        internal List<IMMessage> GetIMMessages(string lastUserName, string selectUser)
        {
            // 查询两个用户之间的消息
            var messages = IMMessages
                .Where(m => (m.SenderName == lastUserName && m.ReceiverName == selectUser) ||
                            (m.SenderName == selectUser && m.ReceiverName == lastUserName))
                .OrderBy(m => m.SentAt)
                .ToList();
            return messages;
        }

        internal List<IMUser> GetIMUsers()
        {
            var users = IMUsers
                .ToList();
            return users;
        }
    }
}

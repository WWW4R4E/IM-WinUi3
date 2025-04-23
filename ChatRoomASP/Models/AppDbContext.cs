using System.Collections.Concurrent;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ChatRoomASP.Models;

public class AppDbContext : DbContext
{
  public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
  {
  }

  public ConcurrentDictionary<int, string> UserConnections { get; } = new();
  public DbSet<IMUser> Users { get; set; }
  public DbSet<IMMessage> IMMessages { get; set; }
  public DbSet<RelationType> RelationTypes { get; set; }
  public DbSet<UserRelation> UserRelations { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);


    // 配置 UserRelations 表的唯一约束
    modelBuilder.Entity<UserRelation>()
        .HasIndex(ur => new { ur.UserId1, ur.UserId2, ur.RelationTypeId })
        .IsUnique();

    // 配置 UserRelations 表的外键约束
    modelBuilder.Entity<UserRelation>()
        .HasOne(ur => ur.User1)
        .WithMany(u => u.UserRelations1)
        .HasForeignKey(ur => ur.UserId1)
        .OnDelete(DeleteBehavior.Restrict);

    modelBuilder.Entity<UserRelation>()
        .HasOne(ur => ur.User2)
        .WithMany(u => u.UserRelations2)
        .HasForeignKey(ur => ur.UserId2)
        .OnDelete(DeleteBehavior.Restrict);

    modelBuilder.Entity<UserRelation>()
        .HasOne(ur => ur.RelationType)
        .WithMany(rt => rt.UserRelations)
        .HasForeignKey(ur => ur.RelationTypeId);
  }

  public static IMMessage CreateMessage(MessageType type, string senderName, string receiverName, string content)
  {
    return new IMMessage
    {
      Type = type,
      SenderName = senderName,
      ReceiverName = receiverName,
      Content = content,
      SentAt = DateTime.Now,
      IsRead = false
    };
  }
}






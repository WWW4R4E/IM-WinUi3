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
    public DbSet<User> Users { get; set; }
    public DbSet<IMMessage> IMMessages { get; set; }
    public DbSet<RelationType> RelationTypes { get; set; }
    public DbSet<UserRelation> UserRelations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // 配置 Identity 相关表
        modelBuilder.Entity<User>().ToTable("Users");
        
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

public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int UserId { get; set; }

    [Required] [MaxLength(255)] public string UserName { get; set; }

    [MaxLength(255)] public string Email { get; set; }

    [Required] [MaxLength(255)] public string PasswordHash { get; set; }

    public ICollection<UserRelation> UserRelations1 { get; set; }
    public ICollection<UserRelation> UserRelations2 { get; set; }
}

public class IMMessage
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int MessageId { get; set; }

    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public MessageType Type { get; set; }

    [Required] public string SenderName { get; set; }

    [Required] public string ReceiverName { get; set; }

    [Required] public string Content { get; set; }

    [Required] [DefaultValue("GETDATE()")] public DateTime SentAt { get; set; }

    [Required] [DefaultValue(false)] public bool IsRead { get; set; }
}

public enum MessageType
{
    Text,
    Image,
    File
}

public class RelationType
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int RelationTypeId { get; set; }

    [Required] [MaxLength(50)] public string Name { get; set; }

    public ICollection<UserRelation> UserRelations { get; set; }
}

public class UserRelation
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int UserRelationId { get; set; } // 修改为主键

    [Required] public int UserId1 { get; set; }
    [Required] public int UserId2 { get; set; }
    [Required] public int RelationTypeId { get; set; }
    
    [Required] public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    [Required] public DateTime UpdatedAt { get; set; } = DateTime.Now;
    [ForeignKey("UserId1")] public User User1 { get; set; }
    [ForeignKey("UserId2")] public User User2 { get; set; }
    [ForeignKey("RelationTypeId")] public RelationType RelationType { get; set; }
}
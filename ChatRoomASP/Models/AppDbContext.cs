using Microsoft.EntityFrameworkCore;

namespace ChatRoomASP.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // 用户相关
    public DbSet<IMUser> IMUsers { get; set; }
    public DbSet<RelationType> RelationTypes { get; set; }
    public DbSet<UserRelation> UserRelations { get; set; } // 添加 UserRelations 的 DbSet

    // 消息相关
    public DbSet<IMMessage> IMMessages { get; set; }

    // 群组相关
    public DbSet<IMGroup> IMGroups { get; set; }
    public DbSet<IMGroupMember> IMGroupMembers { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 配置 IMUser 表的ID从 10000000 开始
        modelBuilder.Entity<IMUser>().HasData(
            new IMUser
            {
                UserId = 10000000, 
                UserName = "InitialUser",
                Email = "initial@example.com",
                PasswordHash = "hashedpassword",
                Status = 0,
                LastActiveTime = new DateTime(2025, 1, 1), // 静态值
                CreatedAt = new DateTime(2025, 1, 1),     // 静态值
                UpdatedAt = new DateTime(2025, 1, 1)      // 静态值
            }
        );
        
        
        // 配置消息表的索引
        modelBuilder.Entity<IMMessage>()
            .HasIndex(m => new { m.ReceiverId, m.SentAt });

        modelBuilder.Entity<IMMessage>()
            .HasIndex(m => new { m.GroupId, m.SentAt });

        // 配置群组成员表的复合主键(可选方案)
        modelBuilder.Entity<IMGroupMember>()
            .HasIndex(gm => new { gm.GroupId, gm.UserId })
            .IsUnique();

        // 配置 UserRelation 与 RelationType 的关系
        modelBuilder.Entity<UserRelation>()
            .HasOne(ur => ur.RelationType) // 关联导航属性 RelationType（引用类型）
            .WithMany(rt => rt.UserRelations) // 关联 RelationType 的集合属性
            .HasForeignKey(ur => ur.RelationTypeId); // 使用外键字段

        // 配置 IMUser 和 UserRelation 的关系
        modelBuilder.Entity<IMUser>()
            .HasMany(u => u.InitiatedRelations) // 配置 IMUser 的导航属性 InitiatedRelations
            .WithOne(ur => ur.InitiatorUser) // 关联 UserRelation 的 InitiatorUser
            .HasForeignKey(ur => ur.InitiatorUserId); // 配置外键字段

        modelBuilder.Entity<IMUser>()
            .HasMany(u => u.ReceivedRelations) // 配置 IMUser 的导航属性 ReceivedRelations
            .WithOne(ur => ur.TargetUser) // 关联 UserRelation 的 TargetUser
            .HasForeignKey(ur => ur.TargetUserId); // 配置外键字段

        // 配置RelationType表的种子数据
        modelBuilder.Entity<RelationType>().HasData(
            new RelationType { RelationTypeId = 1, RelationTypeName = "Friend" }, // 好友
            new RelationType { RelationTypeId = 2, RelationTypeName = "SpecialCare" }, // 特别关心
            new RelationType { RelationTypeId = 3, RelationTypeName = "Blacklist" } // 黑名单
        );
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatRoomASP.Models;

public class IMGroupMember
{
    [Key] public long Id { get; set; }

    [Required] public long GroupId { get; set; }

    [Required] public long UserId { get; set; }

    [Required] public GroupMemberRole Role { get; set; } = GroupMemberRole.Member;
    public bool IsAdmin { get; set; } // 或扩展权限字段
    public DateTimeOffset? MuteUntil { get; set; } // 禁言结束时间

    [Required] public DateTimeOffset JoinedAt { get; set; } = DateTimeOffset.Now;

    // 导航属性
    [ForeignKey("GroupId")] public IMGroup Group { get; set; }

    [ForeignKey("UserId")] public IMUser User { get; set; }
}

public enum GroupMemberRole
{
    Owner, // 群主
    Admin, // 管理员
    Member, // 普通成员
    Muted // 被禁言成员
}
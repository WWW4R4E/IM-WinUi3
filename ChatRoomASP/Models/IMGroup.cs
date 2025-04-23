using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatRoomASP.Models;

public class IMGroup
{
    [Key]
    public long GroupId { get; set; }

    [Required]
    [MaxLength(100)]
    public string GroupName { get; set; }

    [MaxLength(500)]
    public string Description { get; set; }

    [Required]
    public long CreatorId { get; set; }

    [Required]
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;

    [MaxLength(255)]
    public string AvatarUrl { get; set; }

    [Required]
    public GroupType Type { get; set; } = GroupType.Normal;

    // 导航属性
    [ForeignKey("CreatorId")]
    public IMUser Creator { get; set; }

    public ICollection<IMGroupMember> Members { get; set; }
    public ICollection<IMMessage> Messages { get; set; }
}

public enum GroupType
{
    Normal,      // 普通群组
    Official,    // 官方群
    Temporary    // 临时群组(如会议群)
}

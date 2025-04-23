using System.ComponentModel.DataAnnotations;

namespace ChatRoomASP.Models;
public class IMUser {
  [Key]
  public long UserId { get; set; } // 用户ID
    
  [Required]
  [MaxLength(50)]
  public string UserName { get; set; } // 用户名
    
  [MaxLength(10240)]
  public byte[] ProfilePicture { get; set; } = new byte[0];  // 用户头像
    
  [Required]
  [MaxLength(100)]
  public string Email { get; set; } // 用户邮箱
    
  [Required]
  [MaxLength(255)]
  public string PasswordHash { get; set; }  // 密码哈希
    
  public int Status { get; set; } = 0; // 用户状态
    
  public DateTimeOffset LastActiveTime { get; set; } // 最后在线时间
    
  public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now; // 用户创建的时间戳
    
  public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.Now; // 用户信息最后更新的时间戳
    
  public ICollection<UserRelation> InitiatedRelations { get; set; } // 用户发起的关系
  public ICollection<UserRelation> ReceivedRelations { get; set; }  // 用户接收的关系
}


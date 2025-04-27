using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatRoomASP.Models;
public class IMMessage
{
  [Key]
  public long MessageId { get; set; }

  [Required]
  public MessageType Type { get; set; }

  [Required]
  public long SenderId { get; set; }

  public long? ReceiverId { get; set; }  // 个人消息接收者
  public long? GroupId { get; set; }    // 群组消息

  [Required]
  [MaxLength(500)]
  public string Content { get; set; }  // 根据Type不同含义不同

  [MaxLength(255)]
  public string? FileName { get; set; } // 可选，用于文件消息

  [Required]
  public DateTimeOffset SentAt { get; set; } = DateTimeOffset.Now;

  [Required]
  public MessageStatus Status { get; set; } = MessageStatus.Sent;

  [Required] public bool IsDeleted { get; set; } = false; // 消息是否软删除,及登录时默认不同步


  // 导航属性
  [ForeignKey("SenderId")]
  public IMUser Sender { get; set; }

  [ForeignKey("ReceiverId")]
  public IMUser Receiver { get; set; }

  [ForeignKey("GroupId")]
  public IMGroup Group { get; set; }
}

public enum MessageStatus
{
  Sent,      // 已发送
  Delivered, // 已送达
  Read       // 已读
}
public enum MessageType
{
  Text,        // 普通文本
  Markdown,    // Markdown格式文本
  Image,       // 静态图片(支持常见格式)
  AnimatedImage, // 动图(GIF/WebP等)
  Video,       // 视频(支持常见格式)
  File         // 通用文件(不特殊处理)
}

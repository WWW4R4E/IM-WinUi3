
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ChatRoomASP.Models;
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

  [Required][DefaultValue("GETDATE()")] public DateTime SentAt { get; set; }

  [Required][DefaultValue(false)] public bool IsRead { get; set; }
}
public enum MessageType
{
  Text,
  Image,
  File
}

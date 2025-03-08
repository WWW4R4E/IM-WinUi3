using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace IMWinUi.Models
{
    internal record class IMMessage
    {
        public IMMessage(MessageType type, string senderName, string receiverName, string content)
        {
            this.Type = type;
            this.SenderName = senderName;
            this.ReceiverName = receiverName;
            this.Content = content;
            this.SentAt = DateTime.Now;
            this.IsRead = false;
        }

        [Key] // 主键
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // 数据库自动生成
        public int MessageId { get; set; }
        [Required]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public MessageType Type { get; set; }
        [Required]
        public string SenderName { get; set; }
        [Required]
        public string ReceiverName { get; set; }

        [Required] // 不可为空
        public string Content { get; set; }

        [Required]
        [DefaultValue("GETDATE()")] // 默认值
        public DateTime SentAt { get; set; }

        [Required]
        [DefaultValue(false)] // 默认值
        public bool IsRead { get; set; }
    }
    public enum MessageType
    {
        Text,
        Image,
        emjio
    }
}
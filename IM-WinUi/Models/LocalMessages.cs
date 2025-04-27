using System;
using IMWinUi.Properties;
using IMWinUi.ViewModels;
using LiteDB;

namespace IMWinUi.Models;

public class LocalMessage
{

    [BsonId]
    public ObjectId Id { get; set; }
    public long MessageId { get; set; } // 与后端一致的消息ID
    public long SenderId { get; set; }
    public long ReceiverId { get; set; } // 个人消息接收者
    public long? GroupId { get; set; } // 群消息群组ID
    public MessageType  ContentType { get; set; }
    public string Content { get; set; }
    public DateTime SendTime { get; set; }
    public int Status { get; set; }
    public bool IsOutgoing { get; set; } // 是否是自己发送的消息
    public bool IsRead { get; set; } // 是否已读

    public static LocalMessage CreateMessage(long receiverId  ,string chatInputText)
    {
        return new LocalMessage
        {
            SenderId = Settings.Default.LastUserId,
            ReceiverId = receiverId,
            ContentType = MessageType.Text,
            Content = chatInputText,
            SendTime = DateTime.Now,
            Status = 0, // 默认状态
            IsOutgoing = true, // 假设是发送的消息
            IsRead = false // 默认未读
        };
    }
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

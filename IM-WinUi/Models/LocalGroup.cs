using System;
using LiteDB;

namespace IMWinUi.Models;

public class LocalGroup
{
    [BsonId]
    public long GroupId { get; set; } // 与后端一致的群组ID
    public string GroupName { get; set; }
    public string AvatarUrl { get; set; }
    public string Description { get; set; }
    public long OwnerId { get; set; }
    public DateTime LastMessageTime { get; set; } // 最后消息时间
}

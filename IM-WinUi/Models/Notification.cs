using System;

namespace IMWinUi.Models;

public class Notification
{
    public int NotificationId { get; set; }
    public string Type { get; set; } // "FriendRequest", "GroupInvite"
    public int SenderId { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }
} 

using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ChatRoomASP.Models;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public ConcurrentDictionary<string, string> UserConnections { get; } = new ConcurrentDictionary<string, string>();
    public DbSet<IMMessage> IMMessages { get; set; }

    public static IMMessage CreateMessage(MessageType type, string senderName, string receiverName, string content)
    {
        return new IMMessage
        {
            Type = type,
            SenderName = senderName,
            ReceiverName = receiverName,
            Content = content,
            SentAt = DateTime.Now,
            IsRead = false
        };
    }
}


public class ApplicationUser : IdentityUser
{
}

public class IMMessage
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int MessageId { get; set; }

    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public MessageType Type { get; set; }

    [Required]
    public string SenderName { get; set; }

    [Required]
    public string ReceiverName { get; set; }

    [Required]
    public string Content { get; set; }

    [Required]
    [DefaultValue("GETDATE()")]
    public DateTime SentAt { get; set; }

    [Required]
    [DefaultValue(false)]
    public bool IsRead { get; set; }
}
public enum MessageType
{
    Text,
    Image,
    File
}

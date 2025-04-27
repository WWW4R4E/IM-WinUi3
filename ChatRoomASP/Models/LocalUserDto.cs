namespace ChatRoomASP.Models;

public class LocalUserDto
{
    public long UserId { get; set; }
    public string Username { get; set; }
    public byte[] ProfilePicture { get; set; }
    public int Status { get; set; }
    public DateTime LastActiveTime { get; set; } // 上次活跃时间
    public DateTime LastInteractionTime { get; set; } // 上次交互时间
    public string RemarkName { get; set; } // 本地备注名（仅当前用户视角）
    public int RelationshipType { get; set; } // 与当前用户的关系类型
}
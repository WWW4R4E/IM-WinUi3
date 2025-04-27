using LiteDB;

namespace IMWinUi.Models;

public class LocalGroupMember
{
    [BsonId]
    public ObjectId Id { get; set; }
    public long GroupId { get; set; }
    public long UserId { get; set; }
    public string Nickname { get; set; } // 群昵称
    public int MemberRole { get; set; } // 成员角色
}

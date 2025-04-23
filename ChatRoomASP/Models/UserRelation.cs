using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ChatRoomASP.Models;

public class UserRelation
{
  [Key]
  [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
  public int UserRelationId { get; set; } // 修改为主键

  [Required] public int UserId1 { get; set; }
  [Required] public int UserId2 { get; set; }
  [Required] public int RelationTypeId { get; set; }

  [Required] public DateTime CreatedAt { get; set; } = DateTime.Now;

  [Required] public DateTime UpdatedAt { get; set; } = DateTime.Now;
  [ForeignKey("UserId1")] public IMUser User1 { get; set; }
  [ForeignKey("UserId2")] public IMUser User2 { get; set; }
  [ForeignKey("RelationTypeId")] public RelationType RelationType { get; set; }
}

public class RelationType
{
  [Key]
  [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
  public int RelationTypeId { get; set; }

  [Required][MaxLength(50)] public string Name { get; set; }

  public ICollection<UserRelation> UserRelations { get; set; }
}
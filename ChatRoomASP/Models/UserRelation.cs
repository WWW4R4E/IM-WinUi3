using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatRoomASP.Models;

public class UserRelation {
  [Key]
  public long UserRelationId { get; set; } // 改为long
    
  [Required]
  public long InitiatorUserId { get; set; } // 明确发起方
    
  [Required]
  public long TargetUserId { get; set; } // 明确目标方
    
  [Required]
  public int RelationTypeId { get; set; } // 新增外键字段
  
  public RelationType RelationType { get; set; }  // 添加导航属性 RelationType（引用 RelationType 实体）

    
  [MaxLength(50)]
  public string RemarkName { get; set; } // 新增
    
  [Required]
  public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
    
  [Required]
  public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.Now;
    
  [ForeignKey("InitiatorUserId")]
  public IMUser InitiatorUser { get; set; } // 重命名
    
  [ForeignKey("TargetUserId")]
  public IMUser TargetUser { get; set; }
  
}




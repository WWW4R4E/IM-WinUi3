using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatRoomASP.Models;

public class RelationType
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int RelationTypeId { get; set; }

    [Required][MaxLength(50)] public string RelationTypeName { get; set; }

    public ICollection<UserRelation> UserRelations { get; set; }
}


using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatRoomASP.Models;
public class IMUser
{
  [Key]
  [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
  public int UserId { get; set; }

  [Required][MaxLength(255)] public string UserName { get; set; }
  public byte[] ProfilePicture { get; set; } = new byte[0];

  [MaxLength(255)] public string Email { get; set; }

  [Required][MaxLength(255)] public string PasswordHash { get; set; }

  public ICollection<UserRelation> UserRelations1 { get; set; }
  public ICollection<UserRelation> UserRelations2 { get; set; }

}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMWinUi.Models
{
    public class IMUser
    {
        public IMUser(int userId, string userName)
        {
            UserId = userId;
            UserName = userName;
        }

        private IMUser()
        {
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string UserName { get; set; }
        public byte[] ProfilePicture { get; set; }

    } 
}

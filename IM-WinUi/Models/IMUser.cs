using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMWinUi.Models
{
    internal class IMUser
    {
        public IMUser(int userId, string userName)
        {
            this.UserId = userId;
            this.Username = userName;
        }

        private IMUser()
        {
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string Username { get; set; }
        public byte[] ProfilePicture { get; set; }

    }
}

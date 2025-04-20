using System;
using System.ComponentModel.DataAnnotations;

namespace ChatRoomASP.Models;

public class LoginModel
{
    [Required]
    public string Name { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Display(Name = "记住我?")]
    public bool RememberMe { get; set; }
}
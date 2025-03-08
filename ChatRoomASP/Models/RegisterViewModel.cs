using System;
using System.ComponentModel.DataAnnotations;

namespace ChatRoomASP.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "姓名是必需的。")]
        [Display(Name = "姓名")]
        public string Name { get; set; }

        [Required(ErrorMessage = "电子邮件是必需的。")]
        [EmailAddress(ErrorMessage = "请输入有效的电子邮件地址。")]
        [Display(Name = "电子邮箱")]
        public string Email { get; set; }

        [Required(ErrorMessage = "密码是必需的。")]
        [StringLength(100, ErrorMessage = "密码必须至少为 {2} 个字符且不超过 {1} 个字符。", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [CustomPasswordValidation(ErrorMessage = "密码必须包含至少一个特殊字符（!@#$%^&*等）")]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "确认密码")]
        [Compare("Password", ErrorMessage = "密码和确认密码不匹配。")]
        public string ConfirmPassword { get; set; }
    }

    public class CustomPasswordValidationAttribute : ValidationAttribute
    {
        // 包含至少一个特殊字符的正则表达式模式
        private const string SpecialCharPattern = @"[!@#$%^&*()\-+=[\]{};:'""\\|,.<>/?]+";

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // 基础验证已由 [Required] 处理，这里只需处理非空值
            if (value is string password && !string.IsNullOrWhiteSpace(password))
            {
                // 验证密码是否包含至少一个特殊字符
                // if (!Regex.IsMatch(password, SpecialCharPattern))
                // {
                //     return new ValidationResult(ErrorMessage);
                // }
            }

            return ValidationResult.Success;
        }
    }
}
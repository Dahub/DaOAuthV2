using System;
using System.ComponentModel.DataAnnotations;

namespace DaOAuthV2.Gui.Front.Models
{
    public class UpdateUserModel : AbstractModel
    {
        public string FullName { get; set; }

        public string EMail { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? BirthDate { get; set; }
    }

    public class LoginModel : AbstractModel
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }
    }

    public class RegisterModel : AbstractModel
    {
        public string UserName { get; set; }

        public string FullName { get; set; }

        public string EMail { get; set; }

        public string Password { get; set; }

        public string RepeatPassword { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? BirthDate { get; set; }
    }

    public class ChangePasswordModel : AbstractModel
    {
        public string OldPassword { get; set; }

        public string NewPassword { get; set; }

        public string NewPasswordRepeat { get; set; }
    }

    public class NewPasswordModel : AbstractModel
    {
        public string Token { get; set; }

        public string NewPassword { get; set; }

        public string NewPasswordRepeat { get; set; }
    }

    public class AskNewPasswordModel : AbstractModel
    {
        public string Email { get; set; }
    }
}

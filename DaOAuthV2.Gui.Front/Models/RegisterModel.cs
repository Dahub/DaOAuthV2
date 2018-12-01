using System;

namespace DaOAuthV2.Gui.Front.Models
{
    public class RegisterModel : AbstractModel
    {
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string EMail { get; set; }
        public string Password { get; set; }
        public string RepeatPassword { get; set; }
        public DateTime? BirthDate { get; set; }
    }
}

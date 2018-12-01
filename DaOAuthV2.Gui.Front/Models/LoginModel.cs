namespace DaOAuthV2.Gui.Front.Models
{
    public class LoginModel : AbstractModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
        public string ReturnUrl { get; set; }
    }
}

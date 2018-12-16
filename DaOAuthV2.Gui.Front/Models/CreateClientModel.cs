namespace DaOAuthV2.Gui.Front.Models
{
    public class CreateClientModel : AbstractModel
    {
        public string ClientType { get; set; }
        public string Name { get; set; }
        public string DefaultReturnUrl { get; set; }
        public string Description { get; set; }
    }
}

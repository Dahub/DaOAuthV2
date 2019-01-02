using System.Collections.Generic;

namespace DaOAuthV2.Gui.Front.Models
{
    public class EditClientModel : AbstractModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ClientPublicId { get; set; }
        public string ClientSecret { get; set; }
        public string Description { get; set; }
        public IDictionary<int, string> ReturnsUrls { get; set; }
        public IList<EditClientScopeModel> Scopes { get; set; }
    }

    public class EditClientScopeModel
    {
        public int Id { get; set; }
        public string Wording { get; set; }
        public bool Selected { get; set; }
    }
}

using System.Collections.Generic;

namespace DaOAuthV2.Gui.Front.Models
{
    public class CreateClientModel : AbstractModel
    {
        public string ClientType { get; set; }

        public string Name { get; set; }

        public IList<string> ReturnUrls { get; set; }

        public string Description { get; set; }

        public IDictionary<string, IList<ScopeClientModel>> Scopes { get; set; } 
    }

    public class UpdateClientModel : AbstractModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string PublicId { get; set; }

        public string ClientSecret { get; set; }

        public string Description { get; set; }

        public string ClientType { get; set; }

        public IList<string> ReturnUrls { get; set; }

        public IDictionary<string, IList<ScopeClientModel>> Scopes { get; set; }
    } 

    public class ScopeClientModel
    {
        public int Id { get; set; }

        public string Wording { get; set; }

        public string NiceWording { get; set; }

        public bool Selected { get; set; }
    }

    public class RevokeOrAcceptClientModel : AbstractModel
    {
        public string ClientPublicId { get; set; }
    }
}

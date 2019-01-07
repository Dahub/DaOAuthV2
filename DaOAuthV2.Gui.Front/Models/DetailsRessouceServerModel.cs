using System.Collections.Generic;

namespace DaOAuthV2.Gui.Front.Models
{
    public class DetailsRessouceServerModel : AbstractModel
    {
        public string Name { get; set; }
        public string Login { get; set; }
        public string Description { get; set; }
        public IList<DetailsRessourceServerScopeModel> Scopes { get; set; }
    }

    public class DetailsRessourceServerScopeModel
    {
        public string Wording { get; set; }
        public string NiceWording { get; set; }
    }
}

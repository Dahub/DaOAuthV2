using System.Collections.Generic;

namespace DaOAuthV2.Gui.Front.Models
{
    public class UpdateRessouceServerModel : AbstractModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Login { get; set; }
        public string Description { get; set; }
        public IList<UpdateRessourceServerScopeModel> Scopes { get; set; }
    }

    public class UpdateRessourceServerScopeModel
    {
        public int? IdScope { get; set; }
        public string NiceWording { get; set; }
        public bool IsReadWrite { get; set; }
    }
}

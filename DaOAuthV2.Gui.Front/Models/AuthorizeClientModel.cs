using System.Collections.Generic;

namespace DaOAuthV2.Gui.Front.Models
{
    public class AuthorizeClientModel
    {
        public string ClientRef { get; set; }
        public string ClientName { get; set; }
        public IList<string> NiceWordingScopes { get; set; }
    }
}

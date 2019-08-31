using System.Collections.Generic;

namespace DaOAuthV2.Gui.Front.Models
{
    public class CreateRessouceServerModel : AbstractModel
    {
        public string Name { get; set; }

        public string Login { get; set; }

        public string Description { get; set; }

        public string Password { get; set; }

        public string RepeatPassword { get; set; }

        public IDictionary<string, bool> Scopes { get; set; }
    }
}

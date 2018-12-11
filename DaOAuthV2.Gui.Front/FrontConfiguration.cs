using System;

namespace DaOAuthV2.Gui.Front
{
    public class FrontConfiguration
    {
        public string AppsDomain { get; set; }
        public string DataProtectionProviderDirectory { get; set; }
        public string DefaultScheme { get; set; }
        public Uri GuiApiUrl { get; set; }
        public Uri OAuthApiUrl { get; set; }
    }
}

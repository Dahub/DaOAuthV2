using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using System;
using System.Threading.Tasks;

namespace DaOAuthV2.Gui.Front.Tools
{
    public class DaOAuthRouteDataRequestCultureProvider : RequestCultureProvider
    {
        public int IndexOfCulture { get; set; }
        public int IndexofUICulture { get; set; }

        public override Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));

            string culture = null;
            string uiCulture = null;

            var twoLetterCultureName = httpContext.Request.Path.Value.Split('/')[IndexOfCulture]?.ToString();
            var twoLetterUICultureName = httpContext.Request.Path.Value.Split('/')[IndexofUICulture]?.ToString();

            if (twoLetterCultureName == "fr")
                culture = "fr-FR";
            else if (twoLetterCultureName == "en")
                culture = "en-US";

            if (twoLetterUICultureName == "fr")
                uiCulture = "fr-FR";
            else if (twoLetterUICultureName == "en")
                culture =  "en-US";

            if (culture == null && uiCulture == null)
                return NullProviderCultureResult;

            if (culture != null && uiCulture == null)
                uiCulture = culture;

            if (culture == null && uiCulture != null)
                culture = uiCulture;

            var providerResultCulture = new ProviderCultureResult(culture, uiCulture);

            return Task.FromResult(providerResultCulture);
        }
    }
}

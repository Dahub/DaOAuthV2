using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DaOAuthV2.Gui.Front.Tools
{
    public class DaOauthFrontController : Controller
    {
        protected readonly FrontConfiguration _conf;
        private HttpClient _client = new HttpClient();

        public DaOauthFrontController(IConfiguration Configuration)
        {
            _conf = Configuration.GetSection("FrontConfiguration").Get<FrontConfiguration>();
        }

        public override ViewResult View()
        {
            return this.View(this.ControllerContext.ActionDescriptor.ActionName);
        }

        public override ViewResult View(object model)
        {
            return this.View(this.ControllerContext.ActionDescriptor.ActionName, model);
        }

        public override ViewResult View(string viewName)
        {
            return this.View(viewName, model: ViewData.Model);
        }

        public override ViewResult View(string viewName, object model)
        {
            return base.View(BuildViewName(viewName), model);
        }

        private string BuildViewName(string viewName)
        {
            var culture = "en";

            if (ControllerContext.RouteData.Values.ContainsKey("culture"))
                culture = this.ControllerContext.RouteData.Values["culture"].ToString();

            return $"{culture}.{viewName}";
        }

        protected async Task<HttpResponseMessage> GetToApi(string route)
        {
            return await GetToApi(route, null);
        }

        protected async Task<HttpResponseMessage> GetToApi(string route, NameValueCollection queryParams)
        {
            Uri myUri = BuildRouteWithParams(ref route, queryParams);

            return await _client.GetAsync(
                $"{_conf.GuiApiUrl}/{route}");
        }

        protected async Task<HttpResponseMessage> PutToApi(string route, object data)
        {
            route = ApplyCultureToRoute(route);

            AddAuthorizationCookieIfAuthentificated();

            return await _client.PutAsJsonAsync(
                $"{_conf.GuiApiUrl}/{route}", data);
        }

        protected async Task<HttpResponseMessage> PostToApi(string route, object data)
        {
            route = ApplyCultureToRoute(route);

            AddAuthorizationCookieIfAuthentificated();

            return await _client.PostAsJsonAsync(
                $"{_conf.GuiApiUrl}/{route}", data);
        }

        protected async Task<HttpResponseMessage> HeadToApi(string route)
        {
            return await HeadToApi(route, null);
        }

        protected async Task<HttpResponseMessage> HeadToApi(string route, NameValueCollection queryParams)
        {
            Uri myUri = BuildRouteWithParams(ref route, queryParams);

            return await _client.SendAsync(new HttpRequestMessage()
            {
                Method = HttpMethod.Head,
                RequestUri = myUri
            });
        }

        private void AddAuthorizationCookieIfAuthentificated()
        {
            if (HttpContext.Request.Cookies[".AspNetCore.DaOAuth"] != null)
            {
                string value = HttpContext.Request.Cookies[".AspNetCore.DaOAuth"];
                _client.DefaultRequestHeaders.Add("Cookie", $".AspNetCore.DaOAuth={value}");
            }
        }

        private string ApplyCultureToRoute(string route)
        {
            string culture = String.Empty;

            if (ControllerContext.RouteData.Values.ContainsKey("culture"))
                culture = this.ControllerContext.RouteData.Values["culture"].ToString();
            else
                culture = "en";

            if (!String.IsNullOrWhiteSpace(culture))
            {
                switch (culture)
                {
                    case "en":
                        route = $"{route}?culture=en-US";
                        break;
                    case "fr":
                        route = $"{route}?culture=fr-FR";
                        break;
                }
            }

            return route;
        }

        private Uri BuildRouteWithParams(ref string route, NameValueCollection queryParams)
        {
            route = ApplyCultureToRoute(route);

            if (queryParams != null)
            {
                foreach (var k in queryParams.AllKeys)
                {
                    route = String.Concat(route, $"&{k}={queryParams.GetValues(k).FirstOrDefault()}");
                }
            }

            AddAuthorizationCookieIfAuthentificated();

            Uri.TryCreate($"{_conf.GuiApiUrl}/{route}", UriKind.Absolute, out Uri myUri);
            return myUri;
        }
    }
}

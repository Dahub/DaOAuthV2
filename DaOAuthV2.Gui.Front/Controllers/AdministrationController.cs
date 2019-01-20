using DaOAuthV2.ApiTools;
using DaOAuthV2.Constants;
using DaOAuthV2.Gui.Front.Models;
using DaOAuthV2.Gui.Front.Tools;
using DaOAuthV2.Service.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

namespace DaOAuthV2.Gui.Front.Controllers
{
    [Authorize(Roles = RoleName.Administrator)]
    public class AdministrationController : DaOauthFrontController
    {
        public AdministrationController(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<IActionResult> List()
        {
            AdministrationDashboardModel model = new AdministrationDashboardModel();

            NameValueCollection nv = new NameValueCollection();
            nv.Add("skip", "0");
            nv.Add("limit", "50");

            var response = await GetToApi($"administration", nv);

            if (!await model.ValidateAsync(response))
                return View(model);

            var users = JsonConvert.DeserializeObject<SearchResult<AdminUsrDto>>(
                await response.Content.ReadAsStringAsync());

            model.Users = users.Datas.Select(u => new AdministrationUserModel()
            {
                ClientCount = u.ClientCount,
                IsValid = u.IsValid,
                UserId = u.Id,
                UserMail = u.Email,
                UserName = u.UserName
            }).ToList();

            return View(model);
        }
    }
}
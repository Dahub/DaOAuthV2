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

        public async Task<IActionResult> UserDetails(int id)
        {
            var model = new AdministrationUserDetailsModel();

            var response = await GetToApi(string.Concat("administration/", id));

            if (!await model.ValidateAsync(response))
            {
                return View(model);
            }

            var userDetail = JsonConvert.DeserializeObject<AdminUserDetailDto>(
             await response.Content.ReadAsStringAsync());

            model.BirthDate = userDetail.BirthDate;
            model.Email = userDetail.Email;
            model.FullName = userDetail.FullName;
            model.Id = userDetail.Id;
            model.UserName = userDetail.UserName;
            model.Clients = userDetail.Clients.Select(c => new AdministrationUserDetailsClientsModel()
            {
                Id = c.Id,
                ClientName = c.ClientName,
                IsActif = c.IsActif,
                IsCreator = c.IsCreator,
                RefreshToken = c.RefreshToken
            }).ToList();

            return View(model);
        }

        [Route("{culture}/Administration/Activate/{userName}")]
        public async Task<IActionResult> Activate(string userName)
        {
            var nv = new NameValueCollection
            {
                { "userName", userName }
            };

            await PutToApi("users/activate", new ActivateOrDesactivateUserDto()
            {
                UserName = userName
            });

            return RedirectToAction("List");
        }

        [Route("{culture}/Administration/Desactivate/{userName}")]
        public async Task<IActionResult> Desactivate(string userName)
        {          
            await PutToApi("users/desactivate", new ActivateOrDesactivateUserDto()
            {
                UserName = userName
            });

            return RedirectToAction("List");
        }

        [Route("{culture}/Administration/UserDelete/{userName}")]
        public async Task<IActionResult> UserDelete(string userName)
        {
            await DeleteToApi($"users/{userName}");

            return RedirectToAction("List");
        }

        public async Task<IActionResult> List()
        {
            var model = new AdministrationDashboardModel();

            var nv = new NameValueCollection
            {
                { "skip", "0" },
                { "limit", "50" }
            };

            var response = await GetToApi($"administration", nv);

            if (!await model.ValidateAsync(response))
            {
                return View(model);
            }

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
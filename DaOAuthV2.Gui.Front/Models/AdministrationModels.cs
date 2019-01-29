using System;
using System.Collections.Generic;

namespace DaOAuthV2.Gui.Front.Models
{
    public class AdministrationDashboardModel : AbstractModel
    {
        public AdministrationDashboardModel()
        {
            Users = new List<AdministrationUserModel>();
        }

        public IList<AdministrationUserModel> Users { get; set; }
    }

    public class AdministrationUserModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserMail { get; set; }
        public bool IsValid { get; set; }
        public int ClientCount { get; set; }
    }

    public class AdministrationUserDetailsModel : AbstractModel
    {
        public AdministrationUserDetailsModel()
        {
            Clients = new List<AdministrationUserDetailsClientsModel>();
        }

        public int Id { get; set; }
        public string UserName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public IList<AdministrationUserDetailsClientsModel> Clients { get; set; }
    }

    public class AdministrationUserDetailsClientsModel
    {
        public int Id { get; set; }
        public string RefreshToken { get; set; }
        public bool IsActif { get; set; }
        public bool IsCreator { get; set; }
        public string ClientName { get; set; }
    }
}

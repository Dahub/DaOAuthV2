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
}

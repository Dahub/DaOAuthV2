using DaOAuthV2.Domain;
using DaOAuthV2.Service.DTO;
using DaOAuthV2.Service.ExtensionsMethods;
using DaOAuthV2.Service.Interface;
using System.Collections.Generic;
using System.Linq;

namespace DaOAuthV2.Service
{
    public class ScopeService : ServiceBase, IScopeService
    {
        public IEnumerable<ScopeDto> GetAll()
        {
            using (var context = RepositoriesFactory.CreateContext())
            {
                var scopes = new List<Scope>();
                var rsRepo = RepositoriesFactory.GetRessourceServerRepository(context);
                var scopeRepo = RepositoriesFactory.GetScopeRepository(context);

                var validRs = rsRepo.GetAll().Where(rs => rs.IsValid.Equals(true)).Select(rs => rs.Id).ToList();

                scopes = scopeRepo.GetAll().Where(s => validRs.Contains(s.RessourceServerId)).ToList();

                return scopes.ToDto();
            }
        }
    }
}

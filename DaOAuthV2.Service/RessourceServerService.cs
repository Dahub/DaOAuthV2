using DaOAuthV2.Constants;
using DaOAuthV2.Domain;
using DaOAuthV2.Service.DTO;
using DaOAuthV2.Service.ExtensionsMethods;
using DaOAuthV2.Service.Interface;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DaOAuthV2.Service
{
    public class RessourceServerService : ServiceBase, IRessourceServerService
    {
        public int CreateRessourceServer(CreateRessourceServerDto toCreate)
        {
            int rsId = 0;

            IList<ValidationResult> ExtendValidation(CreateRessourceServerDto toValidate)
            {
                var resource = this.GetErrorStringLocalizer();
                IList<ValidationResult> result = new List<ValidationResult>();

                if (!String.IsNullOrEmpty(toCreate.Password)
                    && !String.IsNullOrEmpty(toCreate.RepeatPassword)
                    && !toCreate.Password.Equals(toCreate.RepeatPassword, StringComparison.Ordinal))
                    result.Add(new ValidationResult(resource["CreateRessourceServerPasswordDontMatch"]));

                if (!toValidate.Password.IsMatchPasswordPolicy())
                    result.Add(new ValidationResult(resource["CreateRessourceServerPasswordPolicyFailed"]));

                // check empties or multiple scopes names
                if (toValidate.Scopes != null)
                {
                    if (toValidate.Scopes.Where(s => String.IsNullOrWhiteSpace(s.NiceWording)).Any())
                        result.Add(new ValidationResult(resource["CreateRessourceServerEmptyScopeWording"]));

                    if (toValidate.Scopes.Where(s => !String.IsNullOrWhiteSpace(s.NiceWording)).GroupBy(s => s.NiceWording.ToUpper()).Where(x => x.Count() > 1).Any())
                        result.Add(new ValidationResult(resource["CreateRessourceServerMultipleScopeWording"]));
                }

                return result;
            }

            Validate(toCreate, ExtendValidation);

            using (var context = RepositoriesFactory.CreateContext(ConnexionString))
            {
                var userRepo = RepositoriesFactory.GetUserRepository(context);
                var rsRepo = RepositoriesFactory.GetRessourceServerRepository(context);
                var scopeRepo = RepositoriesFactory.GetScopeRepository(context);

                var myUser = userRepo.GetByUserName(toCreate.UserName);
                if (myUser == null || !myUser.IsValid)
                {
                    throw new DaOAuthServiceException("CreateRessourceServerInvalidUserName");
                }
                if(myUser.UsersRoles.FirstOrDefault(r => r.RoleId.Equals((int)ERole.ADMIN)) == null)
                {
                    throw new DaOAuthServiceException("CreateRessourceServerNonAdminUserName");
                }

                var existingRs = rsRepo.GetByLogin(toCreate.Login);
                if(existingRs != null)
                {
                    throw new DaOAuthServiceException("CreateRessourceServerExistingLogin");
                }

                // create ressource server
                rsId = rsRepo.Add(new RessourceServer()
                {
                    CreationDate = DateTime.Now,
                    Description = toCreate.Description,
                    IsValid = true,
                    Login = toCreate.Login,
                    Name = toCreate.Name,
                    ServerSecret = Sha256Hash(string.Concat(Configuration.PasswordSalt, toCreate.Password))
                });

                // check for existing scope, if ok, create
                if (toCreate.Scopes != null)
                {
                    foreach (var s in toCreate.Scopes)
                    {
                        string s1 = s.NiceWording.ToScopeWording(true);
                        string s2 = s.NiceWording.ToScopeWording(false);

                        var scope = scopeRepo.GetByWording(s1);
                        if(scope != null)
                            throw new DaOAuthServiceException("CreateRessourceServerExistingScope");

                        scope = scopeRepo.GetByWording(s2);
                        if (scope != null)
                            throw new DaOAuthServiceException("CreateRessourceServerExistingScope");

                        scope = new Scope()
                        {
                            NiceWording = s.NiceWording,
                            Wording = s.NiceWording.ToScopeWording(s.IsReadWrite),
                            RessourceServerId = rsId
                        };

                        scopeRepo.Add(scope);
                    }
                }

                context.Commit();
            }

            return rsId;
        }

        public void Delete(DeleteRessourceServerDto toDelete)
        {
            throw new NotImplementedException();
        }

        public RessourceServerDto GetById(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<RessourceServerDto> Search(RessourceServerSearchDto criterias)
        {
            throw new NotImplementedException();
        }

        public int SearchCount(RessourceServerSearchDto criterias)
        {
            throw new NotImplementedException();
        }

        public RessourceServerDto Update(UpdateRessourceServerDto toUpdate)
        {
            throw new NotImplementedException();
        }
    }
}

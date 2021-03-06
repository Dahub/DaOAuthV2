﻿using DaOAuthV2.Domain;
using DaOAuthV2.Service.DTO;
using DaOAuthV2.Service.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using DaOAuthV2.Service.ExtensionsMethods;

namespace DaOAuthV2.Service
{
    /// <summary>
    /// User managment
    /// Activate or desactivate an user, revoke refreshs token
    /// </summary>
    public class AdministrationService : ServiceBase, IAdministrationService
    {
        public int SearchCount(AdminUserSearchDto criterias)
        {
            Validate(criterias, ExtendValidationSearchCriterias);

            using (var c = RepositoriesFactory.CreateContext())
            {
                var userRepo = RepositoriesFactory.GetUserRepository(c);
                return userRepo.GetAllByCriteriasCount(criterias.UserName, criterias.Email, criterias.IsValid);
            }
        }

        public IEnumerable<AdminUsrDto> Search(AdminUserSearchDto criterias)
        {
            Validate(criterias, ExtendValidationSearchCriterias);

            IList<User> users = null;

            using (var context = RepositoriesFactory.CreateContext())
            {
                var userRepo = RepositoriesFactory.GetUserRepository(context);

                users = userRepo.GetAllByCriterias(criterias.UserName, criterias.Email, criterias.IsValid,
                    criterias.Skip, criterias.Limit).ToList();
            }

            if (users != null)
            {
                return users.ToAdminDto();
            }

            return new List<AdminUsrDto>();
        }

        private IList<ValidationResult> ExtendValidationSearchCriterias(AdminUserSearchDto c)
        {
            var resource = this.GetErrorStringLocalizer();
            IList<ValidationResult> result = new List<ValidationResult>();

            if (c.Limit - c.Skip > 50)
            {
                result.Add(new ValidationResult(String.Format(resource["SearchAdministrationAskTooMuch"], c)));
            }

            return result;
        }

        public AdminUserDetailDto GetByIdUser(int idUser)
        {
            using (var context = RepositoriesFactory.CreateContext())
            {
                var userClientRepo = RepositoriesFactory.GetUserRepository(context);

                var user = userClientRepo.GetById(idUser);

                if (user == null)
                {
                    throw new DaOAuthNotFoundException("GetByIdUserUserNotFound");
                }

                return user.ToAdminDetailDto();
            }
        }
    }
}

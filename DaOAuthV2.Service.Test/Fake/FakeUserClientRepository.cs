using DaOAuthV2.Dal.Interface;
using DaOAuthV2.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DaOAuthV2.Service.Test.Fake
{
    public class FakeUserClientRepository : IUserClientRepository
    {
        public IContext Context { get; set; }

        public int Add(UserClient toAdd)
        {
            if(FakeDataBase.Instance.UsersClient.Count() == 0)
            {
                toAdd.Id = 1;
            }
            else
            {
                toAdd.Id = FakeDataBase.Instance.UsersClient.Max(u => u.Id) + 1;
            }
            FakeDataBase.Instance.UsersClient.Add(toAdd);
            return toAdd.Id;
        }

        public void Delete(UserClient toDelete)
        {
            var uc = FakeDataBase.Instance.UsersClient.FirstOrDefault(r => r.Id.Equals(toDelete.Id));
            if (uc != null)
            {
                FakeDataBase.Instance.UsersClient.Remove(uc);
            }
        }

        public IEnumerable<UserClient> GetAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<UserClient> GetAllByClientId(int clientId)
        {
            return FakeDataBase.Instance.UsersClient.Where(uc => uc.ClientId.Equals(clientId));
        }

        public IEnumerable<UserClient> GetAllByCriterias(string userName, string name, bool? isValid, int? clientTypeId, uint skip, uint take)
        {
            var user = FakeDataBase.Instance.Users.Where(u => u.UserName.Equals(userName, StringComparison.Ordinal)).FirstOrDefault();

            if (user == null)
            {
                return null;
            }

            var userClients = FakeDataBase.Instance.UsersClient;

            if (userClients == null)
            {
                return null;
            }

            foreach (var uc in userClients)
            {
                uc.User = FakeDataBase.Instance.Users.FirstOrDefault(u => u.Id.Equals(uc.UserId));
                uc.Client = FakeDataBase.Instance.Clients.FirstOrDefault(c => c.Id.Equals(uc.ClientId));
                if (uc.Client != null)
                {
                    uc.Client.ClientType = FakeDataBase.Instance.ClientTypes.FirstOrDefault(ct => ct.Id.Equals(uc.Client.ClientTypeId));
                    uc.Client.ClientReturnUrls = FakeDataBase.Instance.ClientReturnUrls.Where(cru => cru.ClientId.Equals(uc.Client.Id)).ToList();
                    uc.Client.ClientsScopes = FakeDataBase.Instance.ClientsScopes.Where(cs => cs.ClientId.Equals(uc.Client.Id)).ToList();
                    uc.Client.UserCreator = FakeDataBase.Instance.Users.FirstOrDefault(u => u.Id.Equals(uc.Client.UserCreatorId));
                    if (uc.Client.ClientsScopes != null)
                    {
                        foreach (var cs in uc.Client.ClientsScopes)
                        {
                            cs.Scope = FakeDataBase.Instance.Scopes.FirstOrDefault(s => s.Id.Equals(cs.ScopeId));
                        }
                    }
                }
            }
            return userClients.Where(uc => uc.Client != null).
                Where(uc => uc.UserId.Equals(user.Id)
                && (String.IsNullOrEmpty(name) || uc.Client.Name.Equals(name))
                && (!isValid.HasValue || uc.Client.IsValid.Equals(isValid.Value))
                && (!clientTypeId.HasValue || uc.Client.ClientTypeId.Equals(clientTypeId.Value)))
                .Skip((int)skip).Take((int)take);
        }

        public int GetAllByCriteriasCount(string userName, string name, bool? isValid, int? clientTypeId)
        {
             var user = FakeDataBase.Instance.Users.Where(u => u.UserName.Equals(userName, StringComparison.Ordinal)).FirstOrDefault();

            if (user == null)
            {
                return 0;
            }

            var userClients = FakeDataBase.Instance.UsersClient;

            if (userClients == null)
            {
                return 0;
            }

            foreach (var uc in userClients)
            {
                uc.User = FakeDataBase.Instance.Users.FirstOrDefault(u => u.Id.Equals(uc.UserId));
                uc.Client = FakeDataBase.Instance.Clients.FirstOrDefault(c => c.Id.Equals(uc.ClientId));
                if (uc.Client != null)
                {
                    uc.Client.ClientType = FakeDataBase.Instance.ClientTypes.FirstOrDefault(ct => ct.Id.Equals(uc.Client.ClientTypeId));
                    uc.Client.ClientReturnUrls = FakeDataBase.Instance.ClientReturnUrls.Where(cru => cru.ClientId.Equals(uc.Client.Id)).ToList();
                    uc.Client.ClientsScopes = FakeDataBase.Instance.ClientsScopes.Where(cs => cs.ClientId.Equals(uc.Client.Id)).ToList();
                    if (uc.Client.ClientsScopes != null)
                    {
                        foreach (var cs in uc.Client.ClientsScopes)
                        {
                            cs.Scope = FakeDataBase.Instance.Scopes.FirstOrDefault(s => s.Id.Equals(cs.ScopeId));
                        }
                    }
                }
            }
            var list = userClients.Where(uc => uc.Client != null).
                Where(uc => uc.UserId.Equals(user.Id)
                && (String.IsNullOrEmpty(name) || uc.Client.Name.Equals(name))
                && (!isValid.HasValue || uc.Client.IsValid.Equals(isValid.Value))
                && (!clientTypeId.HasValue || uc.Client.ClientTypeId.Equals(clientTypeId.Value)));

            return list.Count();
        }

        public IEnumerable<UserClient> GetAllByUserId(int userId)
        {
            var ucs = FakeDataBase.Instance.UsersClient.Where(uc => uc.UserId.Equals(userId));
            if (ucs == null)
            {
                new List<UserClient>();
            }

            foreach (var uc in ucs)
            {
                uc.Client = FakeDataBase.Instance.Clients.Where(c => c.Id.Equals(uc.ClientId)).FirstOrDefault();
                uc.User = FakeDataBase.Instance.Users.Where(u => u.Id.Equals(uc.UserId)).FirstOrDefault();
            }

            return ucs;
        }

        public UserClient GetById(int id)
        {
            return FakeDataBase.Instance.UsersClient.Where(uc => uc.Id.Equals(id)).FirstOrDefault();
        }

        public UserClient GetUserClientByClientPublicIdAndUserName(string clientPublicId, string userName)
        {
            var cl = FakeDataBase.Instance.Clients.Where(c => c.PublicId.Equals(clientPublicId)).FirstOrDefault();
            if (cl == null)
            {
                return null;
            }

            var us = FakeDataBase.Instance.Users.Where(u => u.UserName.Equals(userName)).FirstOrDefault();
            if (us == null)
            {
                return null;
            }

            var toReturn = FakeDataBase.Instance.UsersClient.Where(x => x.ClientId.Equals(cl.Id) && x.UserId.Equals(us.Id)).FirstOrDefault();

            if (toReturn == null)
            {
                return null;
            }

            toReturn.Client = cl;
            toReturn.User = us;

            toReturn.Client.UserCreator = FakeDataBase.Instance.Users.FirstOrDefault(u => u.Id.Equals(cl.UserCreatorId));
            
            return toReturn;
        }

        public void Update(UserClient toUpdate)
        {
            var uc = FakeDataBase.Instance.UsersClient.Where(u => u.Id.Equals(toUpdate.Id)).First();
            uc.Id = toUpdate.Id;
            uc.IsActif = toUpdate.IsActif;
            uc.RefreshToken = toUpdate.RefreshToken;
            uc.UserId = toUpdate.UserId;      
        }
    }
}

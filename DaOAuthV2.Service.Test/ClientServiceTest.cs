﻿using DaOAuthV2.ApiTools;
using DaOAuthV2.Constants;
using DaOAuthV2.Dal.Interface;
using DaOAuthV2.Domain;
using DaOAuthV2.Service.DTO;
using DaOAuthV2.Service.Interface;
using DaOAuthV2.Service.Test.Fake;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DaOAuthV2.Service.Test
{
    [TestClass]
    public class ClientServiceTest
    {
        private IClientService _service;
        private IClientRepository _repo;
        private Client _validClient;
        private Client _invalidClient;
        private Client _anotherValidClient;
        private User _validUserCreator;
        private User _validUserNonCreator;
        private User _invalidUser;
        private ClientReturnUrl _clientReturnUrl;
        private Scope _scope;
        private readonly string _generatedClientSecret = "azerty";

        [TestInitialize]
        public void Init()
        {
            _validUserCreator = new User()
            {
                CreationDate = DateTime.Now,
                EMail = "sam@crab.org",
                FullName = "Sammy le Crabe",
                Id = 646,
                IsValid = true,
                Password = new byte[] { 0 },
                UserName = "Sam_Crab"
            };

            _validUserNonCreator = new User()
            {
                CreationDate = DateTime.Now,
                EMail = "jack@crab.org",
                FullName = "Jack le Crabe",
                Id = 7215,
                IsValid = true,
                Password = new byte[] { 0 },
                UserName = "Jack_Crab"
            };

            _invalidUser = new User()
            {
                CreationDate = DateTime.Now,
                EMail = "nop@crab.org",
                FullName = "Nop le Crabe",
                Id = 2765,
                IsValid = false,
                Password = new byte[] { 0 },
                UserName = "Nop_Crab"
            };

            _validClient = new Client()
            {
                ClientSecret = "abcdefghijklmnopqrstuv",
                ClientTypeId = 1,
                CreationDate = DateTime.Now,
                Description = "test",
                Id = 777,
                IsValid = true,
                Name = "test",
                PublicId = "abc",
                UserCreatorId = _validUserCreator.Id
            };

            _anotherValidClient = new Client()
            {
                ClientSecret = "fghjzertukh",
                ClientTypeId = 1,
                CreationDate = DateTime.Now,
                Description = "test again",
                Id = 778,
                IsValid = true,
                Name = "test again",
                PublicId = "def",
                UserCreatorId = _validUserCreator.Id
            };

            _invalidClient = new Client()
            {
                ClientSecret = "abcdefghijklmnopqrstuv",
                ClientTypeId = 1,
                CreationDate = DateTime.Now,
                Description = "test_invalid",
                Id = 776,
                IsValid = false,
                Name = "test_invalid",
                PublicId = "abc_invalid",
                UserCreatorId = _invalidUser.Id
            };

            FakeDataBase.Instance.Clients.Add(_validClient);
            FakeDataBase.Instance.Clients.Add(_invalidClient);
            FakeDataBase.Instance.Clients.Add(_anotherValidClient);
            FakeDataBase.Instance.Users.Add(_validUserCreator);
            FakeDataBase.Instance.Users.Add(_validUserNonCreator);
            FakeDataBase.Instance.Users.Add(_invalidUser);

            _clientReturnUrl = new ClientReturnUrl()
            {
                Id = 6131,
                ClientId = _validClient.Id,
                ReturnUrl = "http://www.perdu.com"
            };

            FakeDataBase.Instance.ClientReturnUrls.Add(_clientReturnUrl);

            _scope = new Scope()
            {
                Id = 2245,
                RessourceServerId = 1,
                Wording = "test",
                NiceWording = "test"
            };

            FakeDataBase.Instance.Scopes.Add(_scope);

            FakeDataBase.Instance.ClientsScopes.Add(new ClientScope()
            {
                Id = 154,
                ClientId = _validClient.Id,
                ScopeId = _scope.Id
            });

            FakeDataBase.Instance.UsersClient.Add(new UserClient()
            {
                ClientId = _invalidClient.Id,
                CreationDate = DateTime.Now,
                Id = 9595,
                IsActif = true,
                UserId = _validUserCreator.Id
            });
            FakeDataBase.Instance.UsersClient.Add(new UserClient()
            {
                ClientId = _validClient.Id,
                CreationDate = DateTime.Now,
                Id = 378,
                IsActif = true,
                UserId = _validUserCreator.Id
            });
            FakeDataBase.Instance.UsersClient.Add(new UserClient()
            {
                ClientId = _validClient.Id,
                CreationDate = DateTime.Now,
                Id = 3784,
                IsActif = true,
                UserId = _validUserNonCreator.Id
            });
            FakeDataBase.Instance.UsersClient.Add(new UserClient()
            {
                ClientId = _validClient.Id,
                CreationDate = DateTime.Now,
                Id = 3785,
                IsActif = true,
                UserId = _invalidUser.Id
            });

            _service = new ClientService()
            {
                Configuration = FakeConfigurationHelper.GetFakeConf(),
                RepositoriesFactory = new FakeRepositoriesFactory(),
                StringLocalizerFactory = new FakeStringLocalizerFactory(),
                Logger = new FakeLogger(),
                RandomService = new FakeRandomService(1, _generatedClientSecret)
            };

            _repo = new FakeClientRepository();
        }

        [TestCleanup]
        public void CleanUp()
        {
            FakeDataBase.Reset();
        }

        [TestMethod]
        public void Create_New_Client_Should_Create_A_Creator_User_Client()
        {
            var name = Guid.NewGuid().ToString();
            var description = Guid.NewGuid().ToString();
            var userName = _validUserCreator.UserName;

            var id = _service.CreateClient(new DTO.CreateClientDto()
            {
                ClientType = ClientTypeName.Confidential,
                ReturnUrls = new List<string>() { "http://www.perdu.com" },
                Name = name,
                UserName = userName,
                Description = description
            });

            Assert.IsTrue(id > 0);

            var client = _repo.GetById(id);

            Assert.IsNotNull(client);

            var ucrepo = new FakeUserClientRepository();
            var myUc = ucrepo.GetUserClientByClientPublicIdAndUserName(client.PublicId, userName);

            Assert.IsNotNull(myUc);
            Assert.IsTrue(myUc.Client.UserCreator.UserName.Equals(userName));
        }

        [TestMethod]
        public void Create_New_Client_Should_Return_Int()
        {
            var name = Guid.NewGuid().ToString();
            var description = Guid.NewGuid().ToString();
            var userName = _validUserCreator.UserName;

            var id = _service.CreateClient(new DTO.CreateClientDto()
            {
                ClientType = ClientTypeName.Confidential,
                ReturnUrls = new List<string>() { "http://www.perdu.com" },
                Name = name,
                UserName = userName,
                Description = description
            });

            Assert.IsTrue(id > 0);

            var client = _repo.GetById(id);

            Assert.IsNotNull(client);
            Assert.AreEqual(description, client.Description);
            Assert.AreEqual(name, client.Name);
            Assert.IsTrue((DateTime.Now - client.CreationDate).TotalSeconds < 10);
        }

        [TestMethod]
        public void Create_New_Client_Should_Create_Scopes()
        {
            var name = Guid.NewGuid().ToString();
            var description = Guid.NewGuid().ToString();
            var userName = _validUserCreator.UserName;
            var sc1 = new Scope()
            {
                Id = 456,
                NiceWording = Guid.NewGuid().ToString(),
                Wording = Guid.NewGuid().ToString()
            };
            var sc2 = new Scope()
            {
                Id = 457,
                NiceWording = Guid.NewGuid().ToString(),
                Wording = Guid.NewGuid().ToString()
            };

            FakeDataBase.Instance.Scopes.Add(sc1);
            FakeDataBase.Instance.Scopes.Add(sc2);

            var id = _service.CreateClient(new DTO.CreateClientDto()
            {
                ClientType = ClientTypeName.Confidential,
                ReturnUrls = new List<string>() { "http://www.perdu.com" },
                Name = name,
                UserName = userName,
                Description = description,
                ScopesIds = new List<int> { 456, 457 }
            });

            Assert.IsTrue(id > 0);

            var client = _repo.GetById(id);

            Assert.IsNotNull(client);

            var clientsScopes = FakeDataBase.Instance.ClientsScopes.Where(cs => cs.ClientId.Equals(id));

            Assert.IsNotNull(clientsScopes);
            Assert.AreEqual(2, clientsScopes.Count());
            Assert.IsTrue(clientsScopes.Select(cs => cs.ScopeId).Contains(sc1.Id));
        }

        [TestMethod]
        public void Create_New_Client_Should_Return_Client_With_Generated_Secret()
        {
            var name = Guid.NewGuid().ToString();
            var description = Guid.NewGuid().ToString();
            var userName = _validUserCreator.UserName;
            var id = _service.CreateClient(new DTO.CreateClientDto()
            {
                ClientType = ClientTypeName.Confidential,
                ReturnUrls = new List<string>() { "http://www.perdu.com" },
                Name = name,
                UserName = userName,
                Description = description
            });

            Assert.IsTrue(id > 0);

            var client = _repo.GetById(id);

            Assert.IsNotNull(client);
            Assert.AreEqual(_generatedClientSecret, client.ClientSecret);
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Create_New_Client_With_Empty_Client_Type_Should_Throw_DaOauthServiceException()
        {
            var id = _service.CreateClient(new DTO.CreateClientDto()
            {
                ReturnUrls = new List<string>() { "http://www.perdu.com" },
                Name = Guid.NewGuid().ToString(),
                UserName = _validUserCreator.UserName
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Create_New_Client_With_Empty_Name_Should_Throw_DaOauthServiceException()
        {
            var id = _service.CreateClient(new DTO.CreateClientDto()
            {
                ClientType = ClientTypeName.Confidential,
                ReturnUrls = new List<string>() { "http://www.perdu.com" },
                Name = String.Empty,
                UserName = _validUserCreator.UserName
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Create_New_Client_With_Empty_Return_Url_Should_Throw_DaOauthServiceException()
        {
            var id = _service.CreateClient(new DTO.CreateClientDto()
            {
                ClientType = ClientTypeName.Confidential,
                ReturnUrls = new List<string>(),
                Name = Guid.NewGuid().ToString(),
                UserName = _validUserCreator.UserName
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Create_New_Client_With_Incorrect_Client_Type_Should_Throw_DaOauthServiceException()
        {
            var id = _service.CreateClient(new DTO.CreateClientDto()
            {
                ClientType = Guid.NewGuid().ToString(),
                ReturnUrls = new List<string>() { "http://www.perdu.com" },
                Name = Guid.NewGuid().ToString(),
                UserName = _validUserCreator.UserName
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Create_New_Client_With_Existing_Name_Should_Throw_DaOauthServiceException()
        {
            var id = _service.CreateClient(new DTO.CreateClientDto()
            {
                ClientType = ClientTypeName.Confidential,
                ReturnUrls = new List<string>() { "http://www.perdu.com" },
                Name = _validClient.Name,
                UserName = _validUserCreator.UserName
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Create_New_Client_With_Incorrect_Return_Url_Should_Throw_DaOauthServiceException()
        {
            var id = _service.CreateClient(new DTO.CreateClientDto()
            {
                ClientType = ClientTypeName.Confidential,
                ReturnUrls = new List<string>() { "httpwww.perdcom" },
                Name = Guid.NewGuid().ToString(),
                UserName = _validUserCreator.UserName
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Create_New_Client_With_Inactive_User_Should_Throw_DaOauthServiceException()
        {
            var id = _service.CreateClient(new DTO.CreateClientDto()
            {
                ClientType = ClientTypeName.Confidential,
                ReturnUrls = new List<string>() { "http://www.perdu.com" },
                Name = Guid.NewGuid().ToString(),
                UserName = _invalidUser.UserName
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Create_New_Client_With_Non_Existing_User_Should_Throw_DaOauthServiceException()
        {
            var id = _service.CreateClient(new DTO.CreateClientDto()
            {
                ClientType = ClientTypeName.Confidential,
                ReturnUrls = new List<string>() { "http://www.perdu.com" },
                Name = Guid.NewGuid().ToString(),
                UserName = Guid.NewGuid().ToString()
            });
        }

        [TestMethod]
        public void Search_Count_Should_Return_All_Valid_Clients_Number()
        {
            var valid = FakeDataBase.Instance.Clients.Where(c => c.IsValid.Equals(true)).Count();

            Assert.AreEqual(valid, _service.SearchCount(new DTO.ClientSearchDto()));
        }

        [TestMethod]
        public void Search_Count_Should_Return_1_With_Client_Name()
        {
            var clientName = FakeDataBase.Instance.Clients.Where(c => c.IsValid.Equals(true)).Select(c => c.Name).FirstOrDefault();
            Assert.IsNotNull(clientName);
            var nbr = _service.SearchCount(new DTO.ClientSearchDto()
            {
                Name = clientName
            });
            Assert.AreEqual(1, nbr);
        }

        [TestMethod]
        public void Search_Count_Should_Return_0_With_Non_Existing_Client_Name()
        {
            var nbr = _service.SearchCount(new DTO.ClientSearchDto()
            {
                Name = Guid.NewGuid().ToString(),
                Skip = 0,
                Limit = 50
            });
            Assert.AreEqual(0, nbr);
        }

        [TestMethod]
        public void Search_Count_Should_Return_All_Confidential_And_Valid_Clients_Number()
        {
            var clientTypeId = FakeDataBase.Instance.ClientTypes.Where(ct => ct.Wording.Equals(ClientTypeName.Confidential)).First().Id;
            var valid = FakeDataBase.Instance.Clients.Where(c => c.IsValid.Equals(true) && c.ClientTypeId.Equals(clientTypeId)).Count();
            var nbr = _service.SearchCount(new DTO.ClientSearchDto()
            {
                ClientType = ClientTypeName.Confidential,
                Skip = 0,
                Limit = 50
            });
            Assert.AreEqual(valid, nbr);
        }

        [TestMethod]
        public void Search_Should_Return_All_Valid_Clients()
        {
            var valid = FakeDataBase.Instance.Clients.Where(c => c.IsValid.Equals(true)).Count();

            var clients = _service.Search(new DTO.ClientSearchDto()
            {
                Skip = 0,
                Limit = 50
            });

            Assert.IsNotNull(clients);
            Assert.AreEqual(valid, clients.Count());
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Search_Should_Throw_DaOAuthServiceException_When_Ask_Too_Much_Results()
        {
            _service.Search(new DTO.ClientSearchDto()
            {
                Skip = 0,
                Limit = 51
            });
        }

        [TestMethod]
        public void Search_Should_Return_One_Client_Where_Limit_Is_Set_To_1()
        {
            var clients = _service.Search(new DTO.ClientSearchDto()
            {
                Skip = 0,
                Limit = 1
            });

            Assert.IsNotNull(clients);
            Assert.AreEqual(1, clients.Count());
        }

        [TestMethod]
        public void Search_Should_Return_One_Client_Where_Limit_Is_Set_To_2()
        {
            var clients = _service.Search(new DTO.ClientSearchDto()
            {
                Skip = 0,
                Limit = 2
            });

            Assert.IsNotNull(clients);
            Assert.AreEqual(2, clients.Count());
        }

        [TestMethod]
        public void Search_Should_Return_Client_Where_Search_By_Name()
        {
            var clientName = FakeDataBase.Instance.Clients.Where(c => c.IsValid.Equals(true)).Select(c => c.Name).FirstOrDefault();
            Assert.IsNotNull(clientName);
            var clients = _service.Search(new DTO.ClientSearchDto()
            {
                Name = clientName,
                Skip = 0,
                Limit = 50
            });

            Assert.IsNotNull(clients);
            Assert.AreEqual(1, clients.Count());
            Assert.AreEqual(clientName, clients.First().Name);
        }

        [TestMethod]
        public void Search_Should_Return_Client_Where_Search_By_Public_Id()
        {
            var publicId = FakeDataBase.Instance.Clients.Where(c => c.IsValid.Equals(true)).Select(c => c.PublicId).FirstOrDefault();
            Assert.IsNotNull(publicId);
            var clients = _service.Search(new DTO.ClientSearchDto()
            {
                PublicId = publicId,
                Skip = 0,
                Limit = 50
            });

            Assert.IsNotNull(clients);
            Assert.AreEqual(1, clients.Count());
        }

        [TestMethod]
        public void Search_Should_Return_All_Confidential_And_Valid_Clients()
        {
            var clientTypeId = FakeDataBase.Instance.ClientTypes.Where(ct => ct.Wording.Equals(ClientTypeName.Confidential)).Single().Id;
            var valid = FakeDataBase.Instance.Clients.Where(c => c.IsValid.Equals(true) && c.ClientTypeId.Equals(clientTypeId)).Count();

            var clients = _service.Search(new DTO.ClientSearchDto()
            {
                ClientType = ClientTypeName.Confidential,
                Skip = 0,
                Limit = 50
            });

            Assert.IsNotNull(clients);
            Assert.AreEqual(valid, clients.Count());
            Assert.IsTrue(clients.Where(c => c.ClientType.Equals(ClientTypeName.Public)).Count() == 0);
        }

        [TestMethod]
        public void Get_By_Id_Should_Return_Client_For_Existing_Id()
        {
            var c = _service.GetById(_validClient.Id, _validUserCreator.UserName);
            Assert.IsNotNull(c);
        }

        [TestMethod]
        public void Get_By_Id_Should_Return_Client_With_Secret_For_Existing_Id_And_Creator()
        {
            var c = _service.GetById(_validClient.Id, _validUserCreator.UserName);
            Assert.IsNotNull(c);
            Assert.IsFalse(String.IsNullOrEmpty(c.ClientSecret));
        }

        [TestMethod]
        public void Get_By_Id_Should_Return_Client_Without_Secret_For_Existing_Id_And_Non_Creator()
        {
            var c = _service.GetById(_validClient.Id, _validUserNonCreator.UserName);
            Assert.IsNotNull(c);
            Assert.IsTrue(String.IsNullOrEmpty(c.ClientSecret));
        }

        [TestMethod]
        public void Get_By_Id_Should_Return_Client_Without_Public_Id_For_Existing_Id_And_Non_Creator()
        {
            var c = _service.GetById(_validClient.Id, _validUserNonCreator.UserName);
            Assert.IsNotNull(c);
            Assert.IsTrue(String.IsNullOrEmpty(c.PublicId));
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthNotFoundException))]
        public void Get_By_Id_Should_Throw_DaOAuthNotFoundException_For_Non_Existing_Id()
        {
            _service.GetById(85674, _validUserCreator.UserName);
        }

        [TestMethod]
        public void Get_By_Id_Should_Return_Client_With_Active_Ressource_Server_Scopes()
        {
            Client cl;
            Scope sc1, sc2, sc3;
            InitTestForInvalidRessourceServerScopes(out cl, out sc1, out sc2, out sc3);

            var client = _service.GetById(cl.Id, _validUserCreator.UserName);

            Assert.IsNotNull(client);
            Assert.IsNotNull(client.Scopes);
            Assert.AreEqual(2, client.Scopes.Count());
            Assert.IsNull(client.Scopes.Where(s => s.Wording.Equals(sc3.Wording)).FirstOrDefault());
            Assert.IsNotNull(client.Scopes.Where(s => s.Wording.Equals(sc1.Wording)).FirstOrDefault());
            Assert.IsNotNull(client.Scopes.Where(s => s.Wording.Equals(sc2.Wording)).FirstOrDefault());
        }

        [TestMethod]
        public void Search_Should_Return_Client_With_Active_Ressource_Server_Scopes()
        {
            Client cl;
            Scope sc1, sc2, sc3;
            InitTestForInvalidRessourceServerScopes(out cl, out sc1, out sc2, out sc3);

            var client = _service.Search(new DTO.ClientSearchDto()
            {
                Name = cl.Name,
                Skip = 0,
                Limit = 1
            }).FirstOrDefault();

            Assert.IsNotNull(client);
            Assert.IsNotNull(client.Scopes);
            Assert.AreEqual(2, client.Scopes.Count());
            Assert.IsNull(client.Scopes.Where(s => s.Wording.Equals(sc3.Wording)).FirstOrDefault());
            Assert.IsNotNull(client.Scopes.Where(s => s.Wording.Equals(sc1.Wording)).FirstOrDefault());
            Assert.IsNotNull(client.Scopes.Where(s => s.Wording.Equals(sc2.Wording)).FirstOrDefault());
        }

        [TestMethod]
        public void Delete_Should_Delete_Client_User_Client_Client_Scopes_And_Client_Return_Url()
        {
            _service.Delete(new DeleteClientDto()
            {
                Id = _validClient.Id,
                UserName = _validUserCreator.UserName
            });

            Assert.IsNull(FakeDataBase.Instance.Clients.FirstOrDefault(c => c.Id.Equals(_validClient.Id)));
            Assert.IsNull(FakeDataBase.Instance.UsersClient.FirstOrDefault(uc => uc.ClientId.Equals(_validClient.Id)));
            Assert.IsNull(FakeDataBase.Instance.ClientReturnUrls.FirstOrDefault(ru => ru.ClientId.Equals(_validClient.Id)));
            Assert.IsNull(FakeDataBase.Instance.ClientsScopes.FirstOrDefault(cs => cs.ClientId.Equals(_validClient.Id)));
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Delete_Should_Throw_DaOAuthServiceException_When_Id_Dont_Exists()
        {
            _service.Delete(new DeleteClientDto()
            {
                Id = 952141,
                UserName = _validUserCreator.UserName
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Delete_Should_Throw_DaOAuthServiceException_When_User_Is_Not_Creator()
        {
            _service.Delete(new DeleteClientDto()
            {
                Id = _validClient.Id,
                UserName = _validUserNonCreator.UserName
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Delete_Should_Throw_DaOAuthServiceException_When_User_Is_Not_Valid()
        {
            _service.Delete(new DeleteClientDto()
            {
                Id = _validClient.Id,
                UserName = _invalidUser.UserName
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Delete_Should_Throw_DaOAuthServiceException_When_User_Is_Non_Existing()
        {
            _service.Delete(new DeleteClientDto()
            {
                Id = _validClient.Id,
                UserName = Guid.NewGuid().ToString()
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Delete_Should_Throw_DaOAuthServiceException_When_User_Name_Is_Empty()
        {
            _service.Delete(new DeleteClientDto()
            {
                Id = _validClient.Id,
                UserName = String.Empty
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Update_Should_Throw_DaOAuthServiceException_When_User_Name_Is_Empty()
        {
            _service.Update(new UpdateClientDto()
            {
                Id = _validClient.Id,
                ClientSecret = _validClient.ClientSecret,
                ClientType = ClientTypeName.Confidential,
                Description = _validClient.Description,
                Name = _validClient.Name,
                PublicId = _validClient.PublicId,
                ReturnUrls = FakeDataBase.Instance.ClientReturnUrls.
                    Where(ru => ru.ClientId.Equals(_validClient.Id)).Select(ru => ru.ReturnUrl).ToList(),
                ScopesIds = FakeDataBase.Instance.ClientsScopes.Where(s => s.ClientId.Equals(_validClient.Id)).Select(s => s.ScopeId).ToList(),
                UserName = String.Empty
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Update_Should_Throw_DaOAuthServiceException_When_User_Name_Is_Invalid()
        {
            _service.Update(new UpdateClientDto()
            {
                Id = _validClient.Id,
                ClientSecret = _validClient.ClientSecret,
                ClientType = ClientTypeName.Confidential,
                Description = _validClient.Description,
                Name = _validClient.Name,
                PublicId = _validClient.PublicId,
                ReturnUrls = FakeDataBase.Instance.ClientReturnUrls.
                    Where(ru => ru.ClientId.Equals(_validClient.Id)).Select(ru => ru.ReturnUrl).ToList(),
                ScopesIds = FakeDataBase.Instance.ClientsScopes.Where(s => s.ClientId.Equals(_validClient.Id)).Select(s => s.ScopeId).ToList(),
                UserName = _invalidUser.UserName
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Update_Should_Throw_DaOAuthServiceException_When_User_Name_Is_Not_Creator()
        {
            _service.Update(new UpdateClientDto()
            {
                Id = _validClient.Id,
                ClientSecret = _validClient.ClientSecret,
                ClientType = ClientTypeName.Confidential,
                Description = _validClient.Description,
                Name = _validClient.Name,
                PublicId = _validClient.PublicId,
                ReturnUrls = FakeDataBase.Instance.ClientReturnUrls.
                    Where(ru => ru.ClientId.Equals(_validClient.Id)).Select(ru => ru.ReturnUrl).ToList(),
                ScopesIds = FakeDataBase.Instance.ClientsScopes.Where(s => s.ClientId.Equals(_validClient.Id)).Select(s => s.ScopeId).ToList(),
                UserName = _validUserNonCreator.UserName
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Update_Should_Throw_DaOAuthServiceException_When_Client_Is_Invalid()
        {
            _service.Update(new UpdateClientDto()
            {
                Id = _invalidClient.Id,
                ClientSecret = _invalidClient.ClientSecret,
                ClientType = ClientTypeName.Confidential,
                Description = _invalidClient.Description,
                Name = _invalidClient.Name,
                PublicId = _invalidClient.PublicId,
                ReturnUrls = FakeDataBase.Instance.ClientReturnUrls.
                    Where(ru => ru.ClientId.Equals(_invalidClient.Id)).Select(ru => ru.ReturnUrl).ToList(),
                ScopesIds = FakeDataBase.Instance.ClientsScopes.Where(s => s.ClientId.Equals(_invalidClient.Id)).Select(s => s.ScopeId).ToList(),
                UserName = _validUserCreator.UserName
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Update_Should_Throw_DaOAuthServiceException_When_Client_Id_Non_Existing()
        {
            _service.Update(new UpdateClientDto()
            {
                Id = 44259,
                ClientSecret = _validClient.ClientSecret,
                ClientType = ClientTypeName.Confidential,
                Description = _validClient.Description,
                Name = _validClient.Name,
                PublicId = _validClient.PublicId,
                ReturnUrls = FakeDataBase.Instance.ClientReturnUrls.
                    Where(ru => ru.ClientId.Equals(_validClient.Id)).Select(ru => ru.ReturnUrl).ToList(),
                ScopesIds = FakeDataBase.Instance.ClientsScopes.Where(s => s.ClientId.Equals(_validClient.Id)).Select(s => s.ScopeId).ToList(),
                UserName = _validUserCreator.UserName
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Update_Should_Throw_DaOAuthServiceException_When_Client_Secret_Is_Empty()
        {
            _service.Update(new UpdateClientDto()
            {
                Id = _validClient.Id,
                ClientSecret = String.Empty,
                ClientType = ClientTypeName.Confidential,
                Description = _validClient.Description,
                Name = _validClient.Name,
                PublicId = _validClient.PublicId,
                ReturnUrls = FakeDataBase.Instance.ClientReturnUrls.
                    Where(ru => ru.ClientId.Equals(_validClient.Id)).Select(ru => ru.ReturnUrl).ToList(),
                ScopesIds = FakeDataBase.Instance.ClientsScopes.Where(s => s.ClientId.Equals(_validClient.Id)).Select(s => s.ScopeId).ToList(),
                UserName = _validUserCreator.UserName
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Update_Should_Throw_DaOAuthServiceException_When_Client_Secret_Is_Too_Short()
        {
            _service.Update(new UpdateClientDto()
            {
                Id = _validClient.Id,
                ClientSecret = "short",
                ClientType = ClientTypeName.Confidential,
                Description = _validClient.Description,
                Name = _validClient.Name,
                PublicId = _validClient.PublicId,
                ReturnUrls = FakeDataBase.Instance.ClientReturnUrls.
                    Where(ru => ru.ClientId.Equals(_validClient.Id)).Select(ru => ru.ReturnUrl).ToList(),
                ScopesIds = FakeDataBase.Instance.ClientsScopes.Where(s => s.ClientId.Equals(_validClient.Id)).Select(s => s.ScopeId).ToList(),
                UserName = _validUserCreator.UserName
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Update_Should_Throw_DaOAuthServiceException_When_Client_Type_Is_Empty()
        {
            _service.Update(new UpdateClientDto()
            {
                Id = _validClient.Id,
                ClientSecret = _validClient.ClientSecret,
                ClientType = String.Empty,
                Description = _validClient.Description,
                Name = _validClient.Name,
                PublicId = _validClient.PublicId,
                ReturnUrls = FakeDataBase.Instance.ClientReturnUrls.
                    Where(ru => ru.ClientId.Equals(_validClient.Id)).Select(ru => ru.ReturnUrl).ToList(),
                ScopesIds = FakeDataBase.Instance.ClientsScopes.Where(s => s.ClientId.Equals(_validClient.Id)).Select(s => s.ScopeId).ToList(),
                UserName = _validUserCreator.UserName
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Update_Should_Throw_DaOAuthServiceException_When_Client_Type_Is_Invalid()
        {
            _service.Update(new UpdateClientDto()
            {
                Id = _validClient.Id,
                ClientSecret = _validClient.ClientSecret,
                ClientType = Guid.NewGuid().ToString(),
                Description = _validClient.Description,
                Name = _validClient.Name,
                PublicId = _validClient.PublicId,
                ReturnUrls = FakeDataBase.Instance.ClientReturnUrls.
                    Where(ru => ru.ClientId.Equals(_validClient.Id)).Select(ru => ru.ReturnUrl).ToList(),
                ScopesIds = FakeDataBase.Instance.ClientsScopes.Where(s => s.ClientId.Equals(_validClient.Id)).Select(s => s.ScopeId).ToList(),
                UserName = _validUserCreator.UserName
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Update_Should_Throw_DaOAuthServiceException_When_Client_Name_Is_Empty()
        {
            _service.Update(new UpdateClientDto()
            {
                Id = _validClient.Id,
                ClientSecret = _validClient.ClientSecret,
                ClientType = ClientTypeName.Confidential,
                Description = _validClient.Description,
                Name = String.Empty,
                PublicId = _validClient.PublicId,
                ReturnUrls = FakeDataBase.Instance.ClientReturnUrls.
                    Where(ru => ru.ClientId.Equals(_validClient.Id)).Select(ru => ru.ReturnUrl).ToList(),
                ScopesIds = FakeDataBase.Instance.ClientsScopes.Where(s => s.ClientId.Equals(_validClient.Id)).Select(s => s.ScopeId).ToList(),
                UserName = _validUserCreator.UserName
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Update_Should_Throw_DaOAuthServiceException_When_Client_Name_Already_Used()
        {
            _service.Update(new UpdateClientDto()
            {
                Id = _validClient.Id,
                ClientSecret = _validClient.ClientSecret,
                ClientType = ClientTypeName.Confidential,
                Description = _validClient.Description,
                Name = _invalidClient.Name,
                PublicId = _validClient.PublicId,
                ReturnUrls = FakeDataBase.Instance.ClientReturnUrls.
                    Where(ru => ru.ClientId.Equals(_validClient.Id)).Select(ru => ru.ReturnUrl).ToList(),
                ScopesIds = FakeDataBase.Instance.ClientsScopes.Where(s => s.ClientId.Equals(_validClient.Id)).Select(s => s.ScopeId).ToList(),
                UserName = _validUserCreator.UserName
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Update_Should_Throw_DaOAuthServiceException_When_Public_Id_Is_Empty()
        {
            _service.Update(new UpdateClientDto()
            {
                Id = _validClient.Id,
                ClientSecret = _validClient.ClientSecret,
                ClientType = ClientTypeName.Confidential,
                Description = _validClient.Description,
                Name = _validClient.Name,
                PublicId = String.Empty,
                ReturnUrls = FakeDataBase.Instance.ClientReturnUrls.
                    Where(ru => ru.ClientId.Equals(_validClient.Id)).Select(ru => ru.ReturnUrl).ToList(),
                ScopesIds = FakeDataBase.Instance.ClientsScopes.Where(s => s.ClientId.Equals(_validClient.Id)).Select(s => s.ScopeId).ToList(),
                UserName = _validUserCreator.UserName
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Update_Should_Throw_DaOAuthServiceException_When_Public_Id_Is_Already_Used()
        {
            _service.Update(new UpdateClientDto()
            {
                Id = _validClient.Id,
                ClientSecret = _validClient.ClientSecret,
                ClientType = ClientTypeName.Confidential,
                Description = _validClient.Description,
                Name = _validClient.Name,
                PublicId = _invalidClient.PublicId,
                ReturnUrls = FakeDataBase.Instance.ClientReturnUrls.
                    Where(ru => ru.ClientId.Equals(_validClient.Id)).Select(ru => ru.ReturnUrl).ToList(),
                ScopesIds = FakeDataBase.Instance.ClientsScopes.Where(s => s.ClientId.Equals(_validClient.Id)).Select(s => s.ScopeId).ToList(),
                UserName = _validUserCreator.UserName
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Update_Should_Throw_DaOAuthServiceException_When_Return_Urls_Are_Empty()
        {
            _service.Update(new UpdateClientDto()
            {
                Id = _validClient.Id,
                ClientSecret = _validClient.ClientSecret,
                ClientType = ClientTypeName.Confidential,
                Description = _validClient.Description,
                Name = _validClient.Name,
                PublicId = _invalidClient.PublicId,
                ReturnUrls = null,
                ScopesIds = FakeDataBase.Instance.ClientsScopes.Where(s => s.ClientId.Equals(_validClient.Id)).Select(s => s.ScopeId).ToList(),
                UserName = _validUserCreator.UserName
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Update_Should_Throw_DaOAuthServiceException_When_Return_Urls_Are_Invalid()
        {
            _service.Update(new UpdateClientDto()
            {
                Id = _validClient.Id,
                ClientSecret = _validClient.ClientSecret,
                ClientType = ClientTypeName.Confidential,
                Description = _validClient.Description,
                Name = _validClient.Name,
                PublicId = _invalidClient.PublicId,
                ReturnUrls = new List<string>() { "urlNotvalid" },
                ScopesIds = FakeDataBase.Instance.ClientsScopes.Where(s => s.ClientId.Equals(_validClient.Id)).Select(s => s.ScopeId).ToList(),
                UserName = _validUserCreator.UserName
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Update_Should_Throw_DaOAuthServiceException_When_Scope_Id_Not_Exists()
        {
            _service.Update(new UpdateClientDto()
            {
                Id = _validClient.Id,
                ClientSecret = _validClient.ClientSecret,
                ClientType = ClientTypeName.Confidential,
                Description = _validClient.Description,
                Name = _validClient.Name,
                PublicId = _invalidClient.PublicId,
                ReturnUrls = FakeDataBase.Instance.ClientReturnUrls.
                    Where(ru => ru.ClientId.Equals(_validClient.Id)).Select(ru => ru.ReturnUrl).ToList(),
                ScopesIds = new List<int>() { 54546, 1541 },
                UserName = _validUserCreator.UserName
            });
        }

        [TestMethod]
        public void Update_Should_Update()
        {
            var scopeId = 99999;

            FakeDataBase.Instance.Scopes.Add(new Scope()
            {
                Id = scopeId,
                NiceWording = "new",
                Wording = "new"
            });

            _service.Update(new UpdateClientDto()
            {
                Id = _validClient.Id,
                ClientSecret = "i_am_updated",
                ClientType = ClientTypeName.Public,
                Description = "a brend new description",
                Name = "updated name",
                PublicId = "this is my new public id",
                ReturnUrls = new List<string>() { "http://updated.com" },
                ScopesIds = new List<int>() { scopeId },
                UserName = _validUserCreator.UserName
            });

            var myClient = FakeDataBase.Instance.Clients.Where(c => c.Id.Equals(_validClient.Id)).FirstOrDefault();

            Assert.IsNotNull(myClient);
            Assert.AreEqual("i_am_updated", myClient.ClientSecret);
            Assert.AreEqual(1, myClient.ClientTypeId);
            Assert.AreEqual("a brend new description", myClient.Description);
            Assert.AreEqual("updated name", myClient.Name);
            Assert.AreEqual("this is my new public id", myClient.PublicId);

            var returnUrls = FakeDataBase.Instance.ClientReturnUrls.Where(ru => ru.ClientId.Equals(myClient.Id));

            Assert.IsNotNull(returnUrls);
            Assert.AreEqual(1, returnUrls.Count());
            Assert.AreEqual("http://updated.com", returnUrls.First().ReturnUrl);

            var cScopes = FakeDataBase.Instance.ClientsScopes.Where(cs => cs.ClientId.Equals(myClient.Id));

            Assert.IsNotNull(cScopes);
            Assert.AreEqual(1, cScopes.Count());
            Assert.AreEqual(scopeId, cScopes.First().ScopeId);
        }

        private void InitTestForInvalidRessourceServerScopes(out Client cl, out Scope sc1, out Scope sc2, out Scope sc3)
        {
            FakeDataBase.Instance.Clients.Clear();
            FakeDataBase.Instance.RessourceServers.Clear();
            FakeDataBase.Instance.Scopes.Clear();
            FakeDataBase.Instance.ClientsScopes.Clear();

            cl = new Client()
            {
                ClientSecret = "abc",
                ClientTypeId = 1,
                CreationDate = DateTime.Now,
                Id = 1,
                IsValid = true,
                Name = "cl",
                PublicId = "abc",
                UserCreatorId = _validUserCreator.Id
            };
            FakeDataBase.Instance.Clients.Add(cl);

            FakeDataBase.Instance.RessourceServers.Add(new RessourceServer()
            {
                CreationDate = DateTime.Now,
                Description = "test rs",
                Id = 1,
                IsValid = true,
                Login = "rs_valid",
                Name = "rs valid",
                ServerSecret = new byte[] { 0 }
            });

            FakeDataBase.Instance.RessourceServers.Add(new RessourceServer()
            {
                CreationDate = DateTime.Now,
                Description = "test rs",
                Id = 2,
                IsValid = false,
                Login = "rs_nvalid",
                Name = "rs invalid",
                ServerSecret = new byte[] { 0 }
            });

            sc1 = new Scope()
            {
                Id = 1,
                Wording = "RW_test_1",
                NiceWording = "test_1",
                RessourceServerId = 1
            };
            sc2 = new Scope()
            {
                Id = 2,
                Wording = "RW_test_2",
                NiceWording = "test_2",
                RessourceServerId = 1
            };
            sc3 = new Scope()
            {
                Id = 3,
                Wording = "RW_test_3",
                NiceWording = "test_3",
                RessourceServerId = 2
            };
            FakeDataBase.Instance.Scopes.Add(sc1);
            FakeDataBase.Instance.Scopes.Add(sc2);
            FakeDataBase.Instance.Scopes.Add(sc3);

            FakeDataBase.Instance.ClientsScopes.Add(new ClientScope()
            {
                Id = 1,
                ClientId = cl.Id,
                ScopeId = sc1.Id
            });

            FakeDataBase.Instance.ClientsScopes.Add(new ClientScope()
            {
                Id = 2,
                ClientId = cl.Id,
                ScopeId = sc2.Id
            });

            FakeDataBase.Instance.ClientsScopes.Add(new ClientScope()
            {
                Id = 3,
                ClientId = cl.Id,
                ScopeId = sc3.Id
            });
        }
    }
}

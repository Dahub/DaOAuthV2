using DaOAuthV2.Constants;
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
    public class RessourceServerServiceTest
    {
        private IRessourceServerService _service;
        private CreateRessourceServerDto _createDto;
        private User _adminUser;
        private Role _adminRole;
        private User _normalUser;
        private Role _normalRole;
        private User _invalidUser;
        private Scope _existingScope;
        private RessourceServer _existingRessourceServer;
        private RessourceServer _validRessourceServer;
        private RessourceServer _invalidRessourceServer;

        [TestInitialize]
        public void Init()
        {
            _service = new RessourceServerService()
            {
                Configuration = FakeConfigurationHelper.GetFakeConf(),
                ConnexionString = string.Empty,
                RepositoriesFactory = new FakeRepositoriesFactory(),
                StringLocalizerFactory = new FakeStringLocalizerFactory(),
                Logger = new FakeLogger()
            };

            _adminUser = new User()
            {
                CreationDate = DateTime.Now,
                EMail = "sam@crabe.org",
                FullName = "Samadmin",
                Id = 123,
                IsValid = true,
                Password = new byte[] { 0 },
                UserName = "AdminSam"
            };
            _normalUser = new User()
            {
                CreationDate = DateTime.Now,
                EMail = "sam2@crabe.org",
                FullName = "SamPasadmin",
                Id = 124,
                IsValid = true,
                Password = new byte[] { 0 },
                UserName = "UserSam"
            };
            _invalidUser = new User()
            {
                CreationDate = DateTime.Now,
                EMail = "sam3@crabe.org",
                FullName = "SamPasValid",
                Id = 125,
                IsValid = false,
                Password = new byte[] { 0 },
                UserName = "InvalidSam"
            };

            FakeDataBase.Instance.Users.Add(_adminUser);
            FakeDataBase.Instance.Users.Add(_normalUser);
            FakeDataBase.Instance.Users.Add(_invalidUser);

            _adminRole = new Role()
            {
                Id = (int)ERole.ADMIN,
                Wording = RoleName.Administrator
            };
            _normalRole = new Role()
            {
                Id = (int)ERole.USER,
                Wording = RoleName.User
            };
            FakeDataBase.Instance.Roles.Clear();
            FakeDataBase.Instance.Roles.Add(_adminRole);
            FakeDataBase.Instance.Roles.Add(_normalRole);

            FakeDataBase.Instance.UsersRoles.Add(new UserRole()
            {
                Id = 345,
                RoleId = _adminRole.Id,
                UserId = _adminUser.Id
            });
            FakeDataBase.Instance.UsersRoles.Add(new UserRole()
            {
                Id = 346,
                RoleId = _normalRole.Id,
                UserId = _normalUser.Id
            });

            _existingRessourceServer = new RessourceServer()
            {
                CreationDate = DateTime.Now,
                Description = "I Exist",
                Id = 999,
                IsValid = true,
                Login = "existingRs",
                Name = "existing",
                ServerSecret = new byte[] { 0 }
            };
            _validRessourceServer = new RessourceServer()
            {
                CreationDate = DateTime.Now,
                Description = "I am valid, yeah",
                Id = 1999,
                IsValid = true,
                Login = "i_am_valid_rs",
                Name = "valid_rs_name",
                ServerSecret = new byte[] { 0 }
            };
            _invalidRessourceServer = new RessourceServer()
            {
                CreationDate = DateTime.Now,
                Description = "I am not valid, sad",
                Id = 2000,
                IsValid = false,
                Login = "i_am_not_valid_rs",
                Name = "invalid_rs_name",
                ServerSecret = new byte[] { 0 }
            };
            FakeDataBase.Instance.RessourceServers.Add(_existingRessourceServer);
            FakeDataBase.Instance.RessourceServers.Add(_validRessourceServer);
            FakeDataBase.Instance.RessourceServers.Add(_invalidRessourceServer);

            _existingScope = new Scope()
            {
                Id = 789,
                RessourceServerId = _existingRessourceServer.Id,
                NiceWording = "I Exist",
                Wording = "RW_I_Exist"
            };
            FakeDataBase.Instance.Scopes.Add(_existingScope);

            _createDto = new CreateRessourceServerDto()
            {
                Description = "Test ressource server",
                Login = "logRs",
                Name = "R-Serv",
                Password = "ploploplop",
                RepeatPassword = "ploploplop",
                Scopes = new List<CreateRessourceServerScopesDto>()
                {
                    new CreateRessourceServerScopesDto()
                    {
                        IsReadWrite = true,
                        NiceWording = "scope1 nice wording"
                    },
                    new CreateRessourceServerScopesDto()
                    {
                        IsReadWrite = false,
                        NiceWording = "scope2 another nice wording"
                    },
                    new CreateRessourceServerScopesDto()
                    {
                        IsReadWrite = true,
                        NiceWording = "scope3 juste another nice wording"
                    }
                },
                UserName = _adminUser.UserName
            };
        }

        [TestCleanup]
        public void CleanUp()
        {
            FakeDataBase.Reset();
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Create_Ressource_Server_Should_Throw_DaOAuthServiceException_Where_Name_Is_Empty()
        {
            _createDto.Name = String.Empty;
            _service.CreateRessourceServer(_createDto);
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Create_Ressource_Server_Should_Throw_DaOAuthServiceException_Where_User_Name_Is_Empty()
        {
            _createDto.UserName = String.Empty;
            _service.CreateRessourceServer(_createDto);
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Create_Ressource_Server_Should_Throw_DaOAuthServiceException_Where_User_Name_Is_Invalid()
        {
            _createDto.UserName = _invalidUser.UserName;
            _service.CreateRessourceServer(_createDto);
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Create_Ressource_Server_Should_Throw_DaOAuthServiceException_Where_User_Name_Is_Not_Admin()
        {
            _createDto.UserName = _normalUser.UserName;
            _service.CreateRessourceServer(_createDto);
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Create_Ressource_Server_Should_Throw_DaOAuthServiceException_Where_Login_Is_Empty()
        {
            _createDto.Login = String.Empty;
            _service.CreateRessourceServer(_createDto);
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Create_Ressource_Server_Should_Throw_DaOAuthServiceException_Where_Login_Already_Used()
        {
            _createDto.Login = _existingRessourceServer.Login;
            _service.CreateRessourceServer(_createDto);
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Create_Ressource_Server_Should_Throw_DaOAuthServiceException_Where_Password_Is_Empty()
        {
            _createDto.Password = String.Empty;
            _service.CreateRessourceServer(_createDto);
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Create_Ressource_Server_Should_Throw_DaOAuthServiceException_Where_Password_Dont_Match_Password_Policy()
        {
            _createDto.Password = "short";
            _service.CreateRessourceServer(_createDto);
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Create_Ressource_Server_Should_Throw_DaOAuthServiceException_Where_Password_Dont_Match_Repeat_Password()
        {
            _createDto.Password = "first_password_#123AA";
            _createDto.RepeatPassword = "repeat_password_#123AA";
            _service.CreateRessourceServer(_createDto);
        }

        [TestMethod]
        public void Create_Ressource_Server_Should_Create()
        {
            var rsId = _service.CreateRessourceServer(_createDto);

            var rs = FakeDataBase.Instance.RessourceServers.FirstOrDefault(x => x.Id.Equals(rsId));

            Assert.IsNotNull(rs);
            Assert.IsTrue(rs.IsValid);
            Assert.AreEqual(_createDto.Description, rs.Description);
            Assert.AreEqual(_createDto.Login, rs.Login);
            Assert.AreEqual(_createDto.Name, rs.Name);

            var scopes = FakeDataBase.Instance.Scopes.Where(sc => sc.RessourceServerId.Equals(rsId));

            Assert.IsNotNull(scopes);
            Assert.AreEqual(3, _createDto.Scopes.Count());
        }

        [TestMethod]
        public void Create_Ressource_Server_Should_Create_Read_Write_Scope_With_Name_Starting_With_RW_()
        {
            var rsId = _service.CreateRessourceServer(_createDto);

            var rs = FakeDataBase.Instance.RessourceServers.FirstOrDefault(x => x.Id.Equals(rsId));

            Assert.IsNotNull(rs);
            Assert.IsTrue(rs.IsValid);
            Assert.AreEqual(_createDto.Description, rs.Description);
            Assert.AreEqual(_createDto.Login, rs.Login);
            Assert.AreEqual(_createDto.Name, rs.Name);

            var scopes = FakeDataBase.Instance.Scopes.Where(sc => sc.RessourceServerId.Equals(rsId));

            Assert.IsNotNull(scopes);
            Assert.AreEqual(3, _createDto.Scopes.Count());

            var myScope = scopes.FirstOrDefault(s => s.NiceWording.Equals(_createDto.Scopes.FirstOrDefault(x => x.IsReadWrite.Equals(true)).NiceWording));
            Assert.IsNotNull(myScope);
            Assert.IsTrue(myScope.Wording.StartsWith("RW_"));
        }

        [TestMethod]
        public void Create_Ressource_Server_Should_Create_Read_Scope_With_Name_Starting_With_R_()
        {
            var rsId = _service.CreateRessourceServer(_createDto);

            var rs = FakeDataBase.Instance.RessourceServers.FirstOrDefault(x => x.Id.Equals(rsId));

            Assert.IsNotNull(rs);
            Assert.IsTrue(rs.IsValid);
            Assert.AreEqual(_createDto.Description, rs.Description);
            Assert.AreEqual(_createDto.Login, rs.Login);
            Assert.AreEqual(_createDto.Name, rs.Name);

            var scopes = FakeDataBase.Instance.Scopes.Where(sc => sc.RessourceServerId.Equals(rsId));

            Assert.IsNotNull(scopes);
            Assert.AreEqual(scopes.Count(), _createDto.Scopes.Count());

            var myScope = scopes.FirstOrDefault(s => s.NiceWording.Equals(_createDto.Scopes.FirstOrDefault(x => x.IsReadWrite.Equals(false)).NiceWording));
            Assert.IsNotNull(myScope);
            Assert.IsTrue(myScope.Wording.StartsWith("R_"));
        }

        [TestMethod]
        public void Create_Ressource_Server_Should_Create_Scope_With_Snake_Case_Wording()
        {
            var rsId = _service.CreateRessourceServer(_createDto);

            var rs = FakeDataBase.Instance.RessourceServers.FirstOrDefault(x => x.Id.Equals(rsId));

            Assert.IsNotNull(rs);
            Assert.IsTrue(rs.IsValid);
            Assert.AreEqual(_createDto.Description, rs.Description);
            Assert.AreEqual(_createDto.Login, rs.Login);
            Assert.AreEqual(_createDto.Name, rs.Name);

            var scopes = FakeDataBase.Instance.Scopes.Where(sc => sc.RessourceServerId.Equals(rsId));

            Assert.IsNotNull(scopes);
            Assert.AreEqual(3, _createDto.Scopes.Count());

            var myScope = scopes.FirstOrDefault(s => s.NiceWording.Equals(_createDto.Scopes.FirstOrDefault(x => x.IsReadWrite.Equals(false)).NiceWording));
            Assert.IsNotNull(myScope);
            Assert.IsTrue(myScope.Wording.Contains("_"));
            Assert.IsFalse(myScope.Wording.Contains(" "));
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Create_Ressource_Server_Should_Throw_DaOAuthServiceException_When_There_Is_Empty_Wording_Scope()
        {
            _createDto.Scopes.Add(new CreateRessourceServerScopesDto()
            {
                IsReadWrite = false,
            });
            _service.CreateRessourceServer(_createDto);
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Create_Ressource_Server_Should_Throw_DaOAuthServiceException_When_Contains_Existing_Wording_Scope()
        {
            _createDto.Scopes.Add(new CreateRessourceServerScopesDto()
            {
                IsReadWrite = _existingScope.Wording.Contains("RW_"),
                NiceWording = _existingScope.NiceWording
            });
            _service.CreateRessourceServer(_createDto);
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Create_Ressource_Server_Should_Throw_DaOAuthServiceException_When_Contains_Multiple_Same_Nice_Wording_Scope()
        {
            _createDto.Scopes.Add(new CreateRessourceServerScopesDto()
            {
                IsReadWrite = true,
                NiceWording = "plop plop"
            });
            _createDto.Scopes.Add(new CreateRessourceServerScopesDto()
            {
                IsReadWrite = false,
                NiceWording = "ploP plop"
            });
            _service.CreateRessourceServer(_createDto);
        }

        [TestMethod]
        public void Search_Count_Should_Return_All_Valid_Ressource_Server_Number_Without_Search_Criterias()
        {
            int total = _service.SearchCount(new RessourceServerSearchDto()
            {
                Limit = 50,
                Skip = 0,
                Login = String.Empty,
                Name = String.Empty
            });

            int expected = FakeDataBase.Instance.RessourceServers.Where(r => r.IsValid.Equals(true)).Count();

            Assert.AreEqual(expected, total);
        }

        [TestMethod]
        public void Search_Count_Should_Return_1_With_Valid_Ressource_Server_Name()
        {
            int total = _service.SearchCount(new RessourceServerSearchDto()
            {
                Limit = 50,
                Skip = 0,
                Login = String.Empty,
                Name = _validRessourceServer.Name
            });

            Assert.AreEqual(1, total);
        }

        [TestMethod]
        public void Search_Count_Should_Return_0_With_Invalid_Ressource_Server_Name()
        {
            int total = _service.SearchCount(new RessourceServerSearchDto()
            {
                Limit = 50,
                Skip = 0,
                Login = String.Empty,
                Name = _invalidRessourceServer.Name
            });

            Assert.AreEqual(0, total);
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Search_Should_Throw_DaOAuthServiceException_When_Ask_Too_Much_Results()
        {
            _service.Search(new DTO.RessourceServerSearchDto()
            {
                Skip = 0,
                Limit = 51
            });
        }

        [TestMethod]
        public void Search_Should_Find_All_Valids_Ressource_Server()
        {
            var result = _service.Search(new DTO.RessourceServerSearchDto()
            {
                Skip = 0,
                Limit = 50
            });

            Assert.IsNotNull(result);
            Assert.AreEqual(FakeDataBase.Instance.RessourceServers.Where(rs => rs.IsValid).Count(), result.Count());
        }

        [TestMethod]
        public void Search_Should_Find_First_Ressource_Server()
        {
            var result = _service.Search(new DTO.RessourceServerSearchDto()
            {
                Skip = 0,
                Limit = 1
            });

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.IsTrue(FakeDataBase.Instance.RessourceServers.Where(rs => rs.IsValid).Select(rs => rs.Name).Contains(result.First().Name));
        }

        [TestMethod]
        public void Search_Should_Find_Ressource_Server_By_Login()
        {
            var result = _service.Search(new DTO.RessourceServerSearchDto()
            {
                Login = _existingRessourceServer.Login,
                Skip = 0,
                Limit = 50
            });

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(_existingRessourceServer.Login, result.First().Login);
        }

        [TestMethod]
        public void Search_Should_Not_Find_Invalid_Ressource_Server_By_Login()
        {
            var result = _service.Search(new DTO.RessourceServerSearchDto()
            {
                Login = _invalidRessourceServer.Login,
                Skip = 0,
                Limit = 50
            });

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void Get_By_Id_Should_Return_Ressource_Server_For_Existing_Id()
        {
            FakeDataBase.Instance.Scopes.Add(new Scope()
            {
                Id = 654,
                NiceWording = "test 1",
                RessourceServerId = _validRessourceServer.Id,
                Wording = "test_1"
            });

            FakeDataBase.Instance.Scopes.Add(new Scope()
            {
                Id = 653,
                NiceWording = "test 2",
                RessourceServerId = _validRessourceServer.Id,
                Wording = "test_2"
            });

            var result = _service.GetById(_validRessourceServer.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual(_validRessourceServer.Id, result.Id);
            Assert.AreEqual(_validRessourceServer.Login, result.Login);
            Assert.AreEqual(_validRessourceServer.Name, result.Name);
            Assert.AreEqual(_validRessourceServer.Description, result.Description);
            Assert.AreEqual(FakeDataBase.Instance.Scopes.Where(s => s.RessourceServerId.Equals(_validRessourceServer.Id)).Count(), result.Scopes.Count());
            foreach (var s in FakeDataBase.Instance.Scopes.Where(s => s.RessourceServerId.Equals(_validRessourceServer.Id)).Select(s => s.NiceWording))
            {
                Assert.IsTrue(result.Scopes.Contains(s));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthNotFoundException))]
        public void Get_By_Id_Should_Throw_DaOAuthNotFoundException_For_Non_Existing_Id()
        {
            _service.GetById(96552);
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthNotFoundException))]
        public void Get_By_Id_Should_Throw_DaOAuthNotFoundException_For_Invalid_Ressource_Server_Id()
        {
            _service.GetById(_invalidRessourceServer.Id);
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Update_Should_Throw_DaOAuthServiceException_When_User_Is_Not_Admin()
        {
            _service.Update(new UpdateRessourceServerDto()
            {
                Id = _existingRessourceServer.Id,
                Description = "new description",
                IsValid = true,
                Name = "new name",
                UserName = _normalUser.UserName
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Update_Should_Throw_DaOAuthServiceException_When_User_Is_Invalid()
        {
            _service.Update(new UpdateRessourceServerDto()
            {
                Id = _existingRessourceServer.Id,
                Description = "new description",
                IsValid = true,
                Name = "new name",
                UserName = _invalidUser.UserName
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Update_Should_Throw_DaOAuthServiceException_When_User_Not_Exists()
        {
            _service.Update(new UpdateRessourceServerDto()
            {
                Id = _existingRessourceServer.Id,
                Description = "new description",
                IsValid = true,
                Name = "new name",
                UserName = "i_dont_exists"
            });
        }

        [TestMethod]
        public void Update_Should_Update()
        {
            _service.Update(new UpdateRessourceServerDto()
            {
                Id = _existingRessourceServer.Id,
                Description = "new description",
                IsValid = true,
                Name = "new name",
                UserName = _adminUser.UserName
            });

            var rs = FakeDataBase.Instance.RessourceServers.Where(r => r.Id.Equals(_existingRessourceServer.Id)).FirstOrDefault();

            Assert.IsNotNull(rs);
            Assert.AreEqual("new name", rs.Name);
            Assert.AreEqual("new description", rs.Description);
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Update_Should_Throw_DaOAuthServiceException_When_Name_Is_Empty()
        {
            _service.Update(new UpdateRessourceServerDto()
            {
                Id = _existingRessourceServer.Id,
                Description = "new description",
                IsValid = true,
                Name = String.Empty,
                UserName = _adminUser.UserName
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Update_Should_Throw_DaOAuthServiceException_When_User_Name_Is_Empty()
        {
            _service.Update(new UpdateRessourceServerDto()
            {
                Id = _existingRessourceServer.Id,
                Description = "new description",
                IsValid = true,
                Name = "new name",
                UserName = String.Empty
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Update_Should_Throw_DaOAuthServiceException_When_Id_Dont_Exists()
        {
            _service.Update(new UpdateRessourceServerDto()
            {
                Id = 9521,
                Description = "new description",
                IsValid = true,
                Name = "new name",
                UserName = _adminUser.UserName
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Delete_Should_Throw_DaOAuthServiceException_When_Id_Dont_Exists()
        {
            _service.Delete(new DeleteRessourceServerDto()
            {
                Id = 9521,
                UserName = _adminUser.UserName
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Delete_Should_Throw_DaOAuthServiceException_When_User_Is_Not_Admin()
        {
            _service.Delete(new DeleteRessourceServerDto()
            {
                Id = _existingRessourceServer.Id,
                UserName = _normalUser.UserName
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Delete_Should_Throw_DaOAuthServiceException_When_User_Is_Not_Valid()
        {
            _service.Delete(new DeleteRessourceServerDto()
            {
                Id = _existingRessourceServer.Id,
                UserName = _invalidUser.UserName
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Delete_Should_Throw_DaOAuthServiceException_When_User_Is_Non_Existing()
        {
            _service.Delete(new DeleteRessourceServerDto()
            {
                Id = _existingRessourceServer.Id,
                UserName = "i_dont_exists"
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Delete_Should_Throw_DaOAuthServiceException_When_User_Name_Is_Empty()
        {
            _service.Delete(new DeleteRessourceServerDto()
            {
                Id = _existingRessourceServer.Id,
                UserName = String.Empty
            });
        }

        [TestMethod]
        public void Delete_Should_Delete()
        {
            _service.Delete(new DeleteRessourceServerDto()
            {
                Id = _existingRessourceServer.Id,
                UserName = _adminUser.UserName
            });

            Assert.IsNull(FakeDataBase.Instance.RessourceServers.FirstOrDefault(rs => rs.Id.Equals(_existingRessourceServer.Id)));
        }

        [TestMethod]
        public void Delete_Should_Delete_Scopes_And_Client_Scopes()
        {
            FakeDataBase.Instance.Clients.Add(new Client()
            {
                Id = 7777,
                ClientTypeId = 1,
                CreationDate = DateTime.Now,
                IsValid = true,
                Name = "demo",
                PublicId = "abc"
            });

            FakeDataBase.Instance.ClientsScopes.Add(new ClientScope()
            {
                ClientId = 7777,
                ScopeId = _existingScope.Id,
                Id = 9731
            });

            _service.Delete(new DeleteRessourceServerDto()
            {
                Id = _existingRessourceServer.Id,
                UserName = _adminUser.UserName
            });

            Assert.IsNull(FakeDataBase.Instance.RessourceServers.FirstOrDefault(rs => rs.Id.Equals(_existingRessourceServer.Id)));
            Assert.IsNull(FakeDataBase.Instance.Scopes.FirstOrDefault(s => s.Id.Equals(_existingScope.Id)));
            Assert.IsNull(FakeDataBase.Instance.ClientsScopes.FirstOrDefault(s => s.Id.Equals(9731)));
        }
    }
}


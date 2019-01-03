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
            FakeDataBase.Instance.RessourceServers.Add(_existingRessourceServer);

            _existingScope = new Scope()
            {
                Id = 789,
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
    }
}

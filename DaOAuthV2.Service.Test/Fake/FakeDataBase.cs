using DaOAuthV2.Constants;
using DaOAuthV2.Domain;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace DaOAuthV2.Service.Test.Fake
{
    internal class FakeDataBase
    {
        private static FakeDataBase _instance;

        internal static FakeDataBase Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new FakeDataBase();
                return _instance;
            }
        }

        internal static void Reset()
        {
            _instance = null;
        }

        private FakeDataBase() { }

        internal IList<Code> Codes = new List<Code>();

        internal IList<ClientType> ClientTypes = new List<ClientType>()
        {
            new ClientType()
            {
                Id = 1,
                Wording = ClientTypeName.Public
            },
            new ClientType()
            {
                Id = 2,
                Wording = ClientTypeName.Confidential
            }
        };

        internal IList<Client> Clients = new List<Client>()
        {
            new Client()
            {
                ClientSecret = "10",
                ClientTypeId = 2,
                CreationDate = DateTime.Now,
                Description = "confidential client",
                Id = 1,
                IsValid = true,
                Name = "client_1",
                PublicId = "public_id_1"
            },
            new Client()
            {
                ClientSecret = "11",
                ClientTypeId = 1,
                CreationDate = DateTime.Now,
                Description = "public client",
                Id = 2,
                IsValid = true,
                Name = "client_2",
                PublicId = "public_id_2"

            },
            new Client()
            {
                ClientSecret = "12",
                ClientTypeId = 2,
                CreationDate = DateTime.Now,
                Description = "confidential client invalid",
                Id = 3,
                IsValid = false,
                Name = "client_3",
                PublicId = "public_id_3"
            },
            new Client()
            {
                ClientSecret = "13",
                ClientTypeId = 2,
                CreationDate = DateTime.Now,
                Description = "confidential client valid again",
                Id = 4,
                IsValid = true,
                Name = "client_4",
                PublicId = "public_id_4"
            },
            new Client()
            {
                ClientSecret = "41",
                ClientTypeId = 1,
                CreationDate = DateTime.Now,
                Description = "public client invalid",
                Id = 5,
                IsValid = false,
                Name = "client_5",
                PublicId = "public_id_5"
            }
        };

        internal IList<ClientReturnUrl> ClientReturnUrls = new List<ClientReturnUrl>()
        {
            new ClientReturnUrl()
            {
                Id = 1,
                ClientId = 1,
                ReturnUrl = "http://www.google.fr"
            },
            new ClientReturnUrl()
            {
                Id = 2,
                ClientId = 1,
                ReturnUrl = "http://www.perdu.com"
            },
            new ClientReturnUrl()
            {
                Id = 3,
                ClientId = 2,
                ReturnUrl = "http://www.stackoverflow.com"
            },
            new ClientReturnUrl()
            {
                Id = 4,
                ClientId = 4,
                ReturnUrl = "http://www.perdu.com"
            }
        };

        internal IList<User> Users = new List<User>()
        {
            new User()
            {
                BirthDate = new DateTime(1978, 09 ,16),
                CreationDate = DateTime.Now,
                EMail = "sam@crab.corp",
                FullName = "Sammy le Crabe",
                Id = 1,
                IsValid = true,
                Password = FakeDataBase.HashPassword(String.Concat(FakeConfigurationHelper.GetFakeConf().PasswordSalt, "test")),
                UserName = "Sammy"
            },
            new User()
            {
                BirthDate = new DateTime(1982, 12 ,28),
                CreationDate = DateTime.Now,
                EMail = "john@crab.corp",
                FullName = "Johnny le Crabe",
                Id = 2,
                IsValid = false,
                Password = FakeDataBase.HashPassword(String.Concat(FakeConfigurationHelper.GetFakeConf().PasswordSalt, "test")),
                UserName = "Johnny"
           },
            new User()
            {
                BirthDate = new DateTime(1980, 10 ,04),
                CreationDate = DateTime.Now,
                EMail = "garry@crab.corp",
                FullName = "Garry le Crabe",
                Id = 3,
                IsValid = true,
                Password = FakeDataBase.HashPassword(String.Concat(FakeConfigurationHelper.GetFakeConf().PasswordSalt, "test")),
                UserName = "Gary"
            },
        };

        internal IList<UserClient> UsersClient = new List<UserClient>()
        {
            new UserClient()
            {
                ClientId = 1,
                Id = 1,
                CreationDate = DateTime.Now,
                IsActif = true,
                RefreshToken = String.Empty,
                UserId = 1,
                UserPublicId = Guid.Parse("0e4db8be-267a-48aa-ba4d-2b8f71e9a6d8")
            },
            new UserClient()
            {
                ClientId = 2,
                Id = 2,
                CreationDate = DateTime.Now,
                IsActif = true,
                RefreshToken = String.Empty,
                UserId = 1,
                UserPublicId = Guid.Parse("0e4db8be-267a-48aa-ba4d-2b8f71e9a6d7")
            },
            new UserClient()
            {
                ClientId = 3,
                Id = 3,
                CreationDate = DateTime.Now,
                IsActif = true,
                RefreshToken = String.Empty,
                UserId = 1,
                UserPublicId = Guid.Parse("0e4db8be-267a-48aa-ba4d-2b8f71e9a6d6")
            },
            new UserClient()
            {
                ClientId = 4,
                Id = 4,
                CreationDate = DateTime.Now,
                IsActif = true,
                RefreshToken = String.Empty,
                UserId = 3,
                UserPublicId = Guid.Parse("0e4db8be-268a-48aa-ba4d-2c8f71e9a6d7")
            },
            new UserClient()
            {
                ClientId = 5,
                Id = 5,
                CreationDate = DateTime.Now,
                IsActif = true,
                RefreshToken = String.Empty,
                UserId = 1,
                UserPublicId = Guid.Parse("0e4db7ce-268a-48aa-ba4d-2c8f71e9a6d7")
            },
            new UserClient()
            {
                ClientId = 4,
                Id = 6,
                CreationDate = DateTime.Now,
                IsActif = true,
                RefreshToken = String.Empty,
                UserId = 2,
                UserPublicId = Guid.Parse("0a4db7ce-268a-48aa-ba4d-2c8f71e9a6d7")
            }
        };

        internal IList<Scope> Scopes = new List<Scope>()
        {
            new Scope()
            {
                Id = 1,
                Wording = "scope_un",
                NiceWording = "scope un"
            },
            new Scope()
            {
                Id = 2,
                Wording = "scope_deux",
                NiceWording = "scope deux"
            },
            new Scope()
            {
                Id = 3,
                Wording = "scope_trois",
                NiceWording = "scope trois"
            }
        };

        internal IList<ClientScope> ClientsScopes = new List<ClientScope>()
        {
            new ClientScope()
            {
                Id = 1,
                ClientId = 4,
                ScopeId = 1
            },
            new ClientScope()
            {
                Id = 1,
                ClientId = 4,
                ScopeId = 2
            },
            new ClientScope()
            {
                Id = 1,
                ClientId = 4,
                ScopeId = 3
            }
        };

        private static byte[] HashPassword(string pwd)
        {
            using (SHA256Managed sha256 = new SHA256Managed())
            {
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(pwd));
            }
        }
    }
}

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
           }
        };

        internal IList<UserClient> UsersClient = new List<UserClient>()
        {
            new UserClient()
            {
                ClientId = 1,
                Id = 1,
                CreationDate = DateTime.Now,
                IsValid = true,
                RefreshToken = String.Empty,
                UserId = 1,
                UserPublicId = Guid.Parse("0e4db8be-267a-48aa-ba4d-2b8f71e9a6d8")
            },
            new UserClient()
            {
                ClientId = 2,
                Id = 2,
                CreationDate = DateTime.Now,
                IsValid = true,
                RefreshToken = String.Empty,
                UserId = 1,
                UserPublicId = Guid.Parse("0e4db8be-267a-48aa-ba4d-2b8f71e9a6d7")
            },
            new UserClient()
            {
                ClientId = 3,
                Id = 3,
                CreationDate = DateTime.Now,
                IsValid = true,
                RefreshToken = String.Empty,
                UserId = 1,
                UserPublicId = Guid.Parse("0e4db8be-267a-48aa-ba4d-2b8f71e9a6d6")
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

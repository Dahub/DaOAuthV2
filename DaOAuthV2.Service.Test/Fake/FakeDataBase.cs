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

        internal IList<User> Users = new List<User>()
        {
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
                }
            },
            {
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
           },
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

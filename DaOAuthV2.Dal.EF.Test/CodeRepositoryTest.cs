using DaOAuthV2.Dal.Interface;
using DaOAuthV2.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DaOAuthV2.Dal.EF.Test
{
    [TestClass]
    public class CodeRepositoryTest
    {
        private IRepositoriesFactory _repoFactory = new EfRepositoriesFactory();
        private const string _dbName = "testCodeRepo";

        [TestInitialize]
        public void Init()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
                         .UseInMemoryDatabase(databaseName: _dbName)
                         .Options;

            using (var context = new DaOAuthContext(options))
            {
                context.Clients.Add(new Client()
                {
                    ClientSecret = "7",
                    ClientTypeId = 1,
                    CreationDate = DateTime.Now,
                    Description = "Client test 1",
                    Id = 100,
                    IsValid = true,
                    Name = "CT1",
                    PublicId = "CT1_id"
                });

                context.Codes.Add(new Code()
                {
                    ClientId = 100,
                    Id = 100,
                    CodeValue = "1234",
                    ExpirationTimeStamp = 1,
                    IsValid = true,
                    Scope = "scope",
                    UserName = "user test",
                    UserPublicId = Guid.Parse("7a1165cd-09f8-4a56-9af2-488dca510f6e")
                });

                context.Codes.Add(new Code()
                {
                    ClientId = 100,
                    Id = 101,
                    CodeValue = "5678",
                    ExpirationTimeStamp = 1,
                    IsValid = true,
                    Scope = "scope",
                    UserName = "user test",
                    UserPublicId = Guid.Parse("8a1165cd-09f8-4a56-9af2-488dca510f6e")
                });

                context.SaveChanges();
            }
        }

        [TestCleanup]
        public void CleanUp()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
                         .UseInMemoryDatabase(databaseName: _dbName)
                         .Options;

            using (var context = new DaOAuthContext(options))
            {
                context.Database.EnsureDeleted();
            }
        }

        [TestMethod]
        public void Get_All_By_Client_Id_Should_Return_2_Codes()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
                     .UseInMemoryDatabase(databaseName: _dbName)
                     .Options;

            IEnumerable<Code> c = null;

            using (var context = new DaOAuthContext(options))
            {
                var codeRepo = _repoFactory.GetCodeRepository(context);
                c = codeRepo.GetAllByClientId("CT1_id");

                Assert.IsNotNull(c);
                Assert.AreEqual(2, c.Count());
            }
        }
    }
}

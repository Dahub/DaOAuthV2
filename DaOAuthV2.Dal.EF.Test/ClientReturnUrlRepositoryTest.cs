﻿using DaOAuthV2.Dal.Interface;
using DaOAuthV2.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DaOAuthV2.Dal.EF.Test
{
    [TestClass]
    public class ClientReturnUrlRepositoryTest
    {
        private IRepositoriesFactory _repoFactory = new EfRepositoriesFactory();
        private const string _dbName = "testClientReturnUrlRepo";

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
                    ClientSecret = new byte[] { 0, 1 },
                    ClientTypeId = 1,
                    CreationDate = DateTime.Now,
                    Description = "Client test 1",
                    Id = 100,
                    IsValid = true,
                    Name = "CT1",
                    PublicId = "CT1_id"
                });

                context.ClientReturnUrl.Add(new ClientReturnUrl()
                {
                    ClientId = 100,
                    Id = 100,
                    ReturnUrl = "http://www.perdu.com"
                });

                context.ClientReturnUrl.Add(new ClientReturnUrl()
                {
                    ClientId = 100,
                    Id = 101,
                    ReturnUrl = "http://www.perdu2.com"
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
        public void GetAllByClientIdTest()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
                      .UseInMemoryDatabase(databaseName: _dbName)
                      .Options;

            IEnumerable<ClientReturnUrl> ru = null;

            using (var context = new DaOAuthContext(options))
            {
                var clientRepo = _repoFactory.GetClientReturnUrlRepository(context);
                ru = clientRepo.GetAllByClientId("CT1_id");

                Assert.IsNotNull(ru);
                Assert.AreEqual(2, ru.Count());
            }
        }
    }
}

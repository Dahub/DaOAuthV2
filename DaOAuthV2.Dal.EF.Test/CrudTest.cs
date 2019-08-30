using System;
using System.Linq;
using DaOAuthV2.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DaOAuthV2.Dal.EF.Test
{
    [TestClass]
    public class CrudTest : TestBase
    {
        [TestInitialize]
        public void Init()
        {
            InitDataBase();
        }

        [TestCleanup]
        public void CleanUp()
        {
            CleanDataBase();
        }

        [TestMethod]
        public void Get_By_Existing_Id_Should_Return_An_Entity()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var correctId = _scope1.Id;

                var scopeRepository = _repoFactory.GetScopeRepository(context);
                var scope = scopeRepository.GetById(100);

                Assert.IsNotNull(scope);
                Assert.AreEqual(correctId, scope.Id);
            }
        }

        [TestMethod]
        public void Get_By_Non_Existing_Id_Should_Return_Null()
        {
            var wrongId = 1546;

            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var scopeRepository = _repoFactory.GetScopeRepository(context);
                var scope = scopeRepository.GetById(wrongId);

                Assert.IsNull(scope);
            }
        }

        [TestMethod]
        public void Add_Should_Add_Entity()
        {
            var actualClientCount = 0;

            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                actualClientCount = context.Clients.Count();

                var clientRepository = _repoFactory.GetClientRepository(context);
                clientRepository.Add(new Client()
                {
                    ClientSecret = "6",
                    ClientTypeId = 1,
                    CreationDate = DateTime.Now,
                    Description = "Test 2",
                    IsValid = true,
                    Name = "testeur2",
                    PublicId = "test"
                });
                context.Commit();
            }

            // Use a separate instance of the context to verify correct datas were saved to database
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                Assert.AreEqual(actualClientCount + 1, context.Clients.Count());
            }
        }

        [TestMethod]
        public void Update_Should_Update_Entity()
        {
            Scope scope = null;
            var idScope = _scope1.Id;
            var newWording = "update";

            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var scopeRepository = _repoFactory.GetScopeRepository(context);
                scope = scopeRepository.GetById(idScope);
                scope.Wording = newWording;
                scopeRepository.Update(scope);
                context.Commit();
            }

            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var scopeRepository = _repoFactory.GetScopeRepository(context);
                scope = scopeRepository.GetById(idScope);
            }

            Assert.IsNotNull(scope);
            Assert.AreEqual(newWording, scope.Wording);
        }

        [TestMethod]
        public void Delete_Should_Delete_Entity()
        {
            Scope scope = null;
            var idScope = _scope1.Id;

            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var scopeRepo = _repoFactory.GetScopeRepository(context);
                scope = scopeRepo.GetById(idScope);
                scopeRepo.Delete(scope);
                context.Commit();
            }

            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                Assert.AreEqual(0, context.Scopes.Count(s => s.Id.Equals(idScope)));

                var scopeRepository = _repoFactory.GetScopeRepository(context);
                scope = scopeRepository.GetById(idScope);
            }

            Assert.IsNull(scope);
        }

        [TestMethod]
        public void Get_All_Should_Return_All_Entities()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var clientRepository = _repoFactory.GetClientRepository(context);
                var clients = clientRepository.GetAll();
                Assert.IsNotNull(clients);
                Assert.IsTrue(clients.Count() > 0);
                Assert.AreEqual(context.Clients.Count(), clients.Count());
            }
        }
    }

}

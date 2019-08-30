using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DaOAuthV2.Dal.EF.Test
{
    [TestClass]
    public class ScopeRepositoryTest : TestBase
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
        public void Get_By_Existing_Client_Public_Id_Should_Return_Correct_Number_Of_Scopes()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var expectedScopesNumber = context.ClientsScopes.
                    Where(cs => cs.Client.PublicId.Equals(_clientConfidential1.PublicId)).
                    Select(cs => cs.Scope).Count();

                var scopeRepository = _repoFactory.GetScopeRepository(context);
                var scopes = scopeRepository.GetByClientPublicId(_clientConfidential1.PublicId);

                Assert.IsNotNull(scopes);
                Assert.IsTrue(scopes.Count() > 0);
                Assert.AreEqual(expectedScopesNumber, scopes.Count());
            }
        }

        [TestMethod]
        public void Get_By_Non_Existing_Client_Public_Id_Should_Return_Empty_List()
        {
            var missingClientPublicId = Guid.NewGuid().ToString();

            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var scopeRepository = _repoFactory.GetScopeRepository(context);
                var scopes = scopeRepository.GetByClientPublicId(missingClientPublicId);

                Assert.IsNotNull(scopes);
                Assert.AreEqual(0, scopes.Count());
            }
        }

        [TestMethod]
        public void Get_By_Wording_Should_Return_Scope()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var scopeRepository = _repoFactory.GetScopeRepository(context);
                var scope = scopeRepository.GetByWording(_scope2.Wording);

                Assert.IsNotNull(scope);
            }
        }

        [TestMethod]
        public void Get_By_Wording_With_Case_Change_Should_Return_Scope()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var scopeRepository = _repoFactory.GetScopeRepository(context);                
                var scope = scopeRepository.GetByWording(_scope2.Wording.ToUpper());

                Assert.AreNotEqual(_scope2.Wording, _scope2.Wording.ToUpper());
                Assert.IsNotNull(scope);
            }
        }

        [TestMethod]
        public void Get_All_Should_Return_Scopes_With_Ressource_Server()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var repo = _repoFactory.GetScopeRepository(context);
                var scopes = repo.GetAll();

                Assert.IsTrue(context.Scopes.Count() > 0);
                Assert.IsNotNull(scopes);
                Assert.IsTrue(scopes.Count() > 0);
                Assert.IsNotNull(scopes.First().RessourceServer);
            }
        }
    }
}

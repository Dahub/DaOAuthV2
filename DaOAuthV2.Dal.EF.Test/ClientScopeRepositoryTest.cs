using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DaOAuthV2.Dal.EF.Test
{
    [TestClass]
    public class ClientScopeRepositoryTest : TestBase
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
        public void Get_All_By_Scope_Id_Should_Return_All()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var expectedClientScopeNumber = context.Scopes.Where(s => s.Id.Equals(_scope4.Id)).Select(s => s.ClientsScopes).Count();

                var clientScopeRepository = _repoFactory.GetClientScopeRepository(context);
                var clientScopes = clientScopeRepository.GetAllByScopeId(_scope4.Id);

                Assert.IsNotNull(clientScopes);
                Assert.IsTrue(clientScopes.Count() > 0);
                Assert.AreEqual(expectedClientScopeNumber, clientScopes.Count());
            }
        }

        [TestMethod]
        public void Get_All_By_Client_Id_Should_Return_All()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var expectedScopeNumber = context.ClientsScopes.Count(cs => cs.ClientId.Equals(_clientConfidential1.Id));

                var clientRepository = _repoFactory.GetClientScopeRepository(context);
                var clientScopes = clientRepository.GetAllByClientId(_clientConfidential1.Id);

                Assert.IsNotNull(clientScopes);
                Assert.IsTrue(clientScopes.Count() > 0);
                Assert.AreEqual(expectedScopeNumber, clientScopes.Count());
            }
        }
    }
}

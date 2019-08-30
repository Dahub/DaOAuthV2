using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DaOAuthV2.Dal.EF.Test
{
    [TestClass]
    public class ClientReturnUrlRepositoryTest : TestBase
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
        public void Get_By_Id_Should_Return_Return_Url_With_Client()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var ruRepo = _repoFactory.GetClientReturnUrlRepository(context);
                var returnUrl = ruRepo.GetById(_clientReturnUrl1ForClient1.Id);
                Assert.IsNotNull(returnUrl);
                Assert.IsNotNull(returnUrl.Client);
            }
        }

        [TestMethod]
        public void Get_All_By_Client_Id_Should_Return_2_Client_Return_Url()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var returnUrlNumberFromDb = context.ClientReturnUrls.Count(ru => ru.ClientId.Equals(_clientConfidential1.Id));

                var clientRepo = _repoFactory.GetClientReturnUrlRepository(context);
                var returnsUrls = clientRepo.GetAllByClientPublicId(_clientConfidential1.PublicId);
                Assert.IsNotNull(returnsUrls);
                Assert.IsTrue(returnsUrls.Count() > 0);
                Assert.AreEqual(returnUrlNumberFromDb, returnsUrls.Count());
            }
        }
    }
}

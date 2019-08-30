using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DaOAuthV2.Dal.EF.Test
{
    [TestClass]
    public class CodeRepositoryTest : TestBase
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
        public void Get_All_By_Client_Id_And_User_Name_Should_Return_Correct_Number_Of_Codes()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var exectedCodeNumber = context.UsersClients.
                    Where(uc => uc.User.UserName.Equals(_user1.UserName) &&
                                uc.Client.PublicId.Equals(_clientConfidential1.PublicId)).SelectMany(uc => uc.Codes).Count();

                var codeRepository = _repoFactory.GetCodeRepository(context);
                var codes = codeRepository.GetAllByClientPublicIdAndUserName(_clientConfidential1.PublicId, _user1.UserName);
                Assert.IsNotNull(codes);
                Assert.IsTrue(codes.Count() > 0);
                Assert.AreEqual(exectedCodeNumber, codes.Count());
            }
        }

        [TestMethod]
        public void Get_By_Code_Should_Return_Code()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var codeRepo = _repoFactory.GetCodeRepository(context);
                var c = codeRepo.GetByCode(_code1.CodeValue);

                Assert.IsNotNull(c);
            }
        }
    }
}

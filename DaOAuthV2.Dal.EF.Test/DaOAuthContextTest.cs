using System;
using System.Linq;
using System.Threading.Tasks;
using DaOAuthV2.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DaOAuthV2.Dal.EF.Test
{
    [TestClass]
    public class DaOAuthContextTest : TestBase
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
        public async Task Commit_Async_Should_Commit()
        {
            var clientName = Guid.NewGuid().ToString();

            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                context.Clients.Add(new Client()
                {
                    ClientSecret = "6",
                    ClientTypeId = 1,
                    CreationDate = DateTime.Now,
                    Description = "Test 2",
                    IsValid = true,
                    Name = clientName,
                    PublicId = "test"
                });
                await context.CommitAsync();
            }

            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                Assert.IsNotNull(context.Clients.SingleOrDefault(c => c.Name.Equals(clientName)));
            }
        }
    }
}

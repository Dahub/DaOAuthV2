using DaOAuthV2.Service.Interface;
using DaOAuthV2.Service.Test.Fake;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;

namespace DaOAuthV2.Service.Test
{
    [TestClass]
    public class JwtServiceTest
    {
        private IJwtService _service;
        private string _tokenName = "tokenName";
        private string _longLifeToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJjbGllbnRfaWQiOiJjbGllbnRJZCIsInRva2VuX25hbWUiOiJ0b2tlbk5hbWUiLCJpc3N1ZWQiOiIxNTQyMjMxMDc4IiwidXNlcl9wdWJsaWNfaWQiOiIwMjA0YmE0NC02NWRhLTQ4ZjMtOWE2OC03ZjdlMWIxNmQ1MmQiLCJuYW1lIjoidXNlck5hbWUiLCJzY29wZSI6InNjb3BlIiwiZXhwIjoxODU3NTg3NDc4LCJpc3MiOiJ0ZXN0LWlzc3VlciIsImF1ZCI6InRlc3QtYXVkaWVuY2UifQ.jEUu9UWgEJ0bsSdnUgR8ylxzjwCx909GfisBPDg0A78";
        private string _shortLifeToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJjbGllbnRfaWQiOiJjbGllbnRJZCIsInRva2VuX25hbWUiOiJ0b2tlbk5hbWUiLCJpc3N1ZWQiOiIxNTQyMjMxMTk4IiwidXNlcl9wdWJsaWNfaWQiOiI4MjM0MmU0Yi01YmI0LTQ5M2YtYjA4NC0xM2Y5MjhmNGU1YmMiLCJuYW1lIjoidXNlck5hbWUiLCJzY29wZSI6InNjb3BlIiwiZXhwIjoxNTQyMjI3NTk4LCJpc3MiOiJ0ZXN0LWlzc3VlciIsImF1ZCI6InRlc3QtYXVkaWVuY2UifQ.2Csoj8pKMBuW6Qpcqiv7sbYM5odmN7sxjYi8FFwiESM";
        
        [TestInitialize]
        public void Init()
        {
            AppConfiguration conf = new AppConfiguration()
            {
                PasswordSalt = "salt",
                SecurityKey = "not-too-short-key",
                Audience = "test-audience",
                Issuer = "test-issuer"
            };

            _service = new JwtService()
            {
                Configuration = conf,
                RepositoriesFactory = new FakeRepositoriesFactory(),
                StringLocalizerFactory = new FakeStringLocalizerFactory(),
                Logger = new FakeLogger()
            };
        }

        [TestMethod]
        public void Generate_Mail_Token_Should_Return_Valid_Token()
        {
            var t = _service.GenerateMailToken("Sammy");
            Assert.IsTrue(!String.IsNullOrWhiteSpace(t.Token));
            Assert.IsTrue(t.IsValid);
        }

        [TestMethod]
        public void Generate_Token_Should_Return_Valid_Token()
        {
            var t = _service.GenerateToken(new DTO.CreateTokenDto()
            {
                ClientPublicId = "clientId",
                SecondsLifeTime = 60,
                Scope = "scope",
                TokenName = _tokenName,
                UserName = "userName"
            });
            Assert.IsTrue(!String.IsNullOrWhiteSpace(t.Token));
            Assert.IsTrue(t.IsValid);
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Generate_Token_With_Empty_Token_Name_Should_Throw_Exception()
        {
            var t = _service.GenerateToken(new DTO.CreateTokenDto()
            {
                ClientPublicId = "clientId",
                SecondsLifeTime = 60,
                Scope = "scope",
                TokenName = String.Empty,
                UserName = "userName"
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Generate_Token_With_Empty_Client_Id_Should_Throw_Exception()
        {
            var t = _service.GenerateToken(new DTO.CreateTokenDto()
            {
                ClientPublicId = String.Empty,
                SecondsLifeTime = 60,
                Scope = "scope",
                TokenName = _tokenName,
                UserName = "userName"
            });
        }

        [TestMethod]
        public void Extract_Mail_Token_Should_Return_Valid()
        {
            var t = _service.ExtractMailToken(_service.GenerateMailToken("Sammy").Token);
            Assert.IsTrue(t.IsValid);
        }

        [TestMethod]
        public void Extract_Token_Should_Return_Valid_Infos_For_Long_Life_Token_Token()
        {
            var t = _service.ExtractToken(new DTO.ExtractTokenDto()
            {
                Token = _longLifeToken,
                TokenName = _tokenName
            });
            Assert.IsTrue(t.IsValid);
        }

        [TestMethod]
        public void Extract_Token_Should_Return_Invalid_Infos_For_Short_Life_Token_Token()
        {
            Thread.Sleep(100);
            var t = _service.ExtractToken(new DTO.ExtractTokenDto()
            {
                Token = _shortLifeToken,
                TokenName = _tokenName
            });
            Assert.IsFalse(t.IsValid);
        }
    }
}

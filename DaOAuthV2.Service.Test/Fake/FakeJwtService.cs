using DaOAuthV2.Service.DTO;
using DaOAuthV2.Service.Interface;
using System.Globalization;
using System.Security.Cryptography;

namespace DaOAuthV2.Service.Test.Fake
{
    public class FakeJwtService : IJwtService
    {
        private readonly JwtTokenDto _token;
        private readonly MailJwtTokenDto _mailToken;

        public FakeJwtService(MailJwtTokenDto mailToken)
        {
            _mailToken = mailToken;
        }

        public FakeJwtService(JwtTokenDto token)
        {
            _token = token;
        }

        public JwtTokenDto ExtractToken(ExtractTokenDto tokenInfo)
        {
            return _token;
        }

        public JwtTokenDto GenerateToken(CreateTokenDto value)
        {
            _token.Token = GenerateRandomString(20);
            return _token;
        }

        private static string GenerateRandomString(int length)
        {
            var valids = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            var s = "";
            using (var provider = new RNGCryptoServiceProvider())
            {
                while (s.Length != length)
                {
                    var oneByte = new byte[1];
                    provider.GetBytes(oneByte);
                    var character = (char)oneByte[0];
                    if (valids.Contains(character.ToString(CultureInfo.CurrentCulture)))
                    {
                        if (s.Length != 0 || character != '0') // don't add 0 for first character
                        {
                            s += character;
                        }
                    }
                }
            }
            return s;
        }

        public MailJwtTokenDto GenerateMailToken(string userName)
        {
            _mailToken.Token = GenerateRandomString(20);
            return _mailToken;
        }

        public MailJwtTokenDto ExtractMailToken(string token)
        {
            return _mailToken;
        }
    }
}
using DaOAuthV2.Service.DTO;
using DaOAuthV2.Service.Interface;
using System;
using System.Globalization;
using System.Security.Cryptography;

namespace DaOAuthV2.Service.Test.Fake
{
    public class FakeJwtService : IJwtService
    {
        private JwtTokenDto _token;

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
            string valids = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            string s = "";
            using (RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider())
            {
                while (s.Length != length)
                {
                    byte[] oneByte = new byte[1];
                    provider.GetBytes(oneByte);
                    char character = (char)oneByte[0];
                    if (valids.Contains(character.ToString(CultureInfo.CurrentCulture)))
                    {
                        if (s.Length != 0 || character != '0') // don't add 0 for first character
                            s += character;
                    }
                }
            }
            return s;
        }
    }
}
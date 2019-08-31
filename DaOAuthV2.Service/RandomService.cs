using DaOAuthV2.Service.Interface;
using System;
using System.Globalization;
using System.Security.Cryptography;

namespace DaOAuthV2.Service
{
    public class RandomService : IRandomService
    {
        private const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        private const string validInt = "0123456789";

        public int GenerateRandomInt(int digits)
        {
            return Int32.Parse(GenerateRandom(digits, validInt), NumberFormatInfo.InvariantInfo);
        }

        public string GenerateRandomString(int stringLenght)
        {
            return GenerateRandom(stringLenght, valid);
        }

        private static string GenerateRandom(int length, string valids)
        {
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
                        if(s.Length != 0 || character != '0') // don't add 0 for first character
                        {
                            s += character;
                        }
                    }
                }
            }
            return s;
        }
    }
}

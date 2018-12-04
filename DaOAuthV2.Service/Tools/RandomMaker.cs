using System;
using System.Globalization;
using System.Security.Cryptography;

namespace DaOAuthV2.Service
{
    internal static class RandomMaker
    {
        const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        const string validInt = "0123456789";

        internal static int GenerateRandomInt(int digits)
        {
            return Int32.Parse(GenerateRandom(digits, validInt), NumberFormatInfo.InvariantInfo);
        }

        internal static string GenerateRandomString(int stringLenght)
        {
            return GenerateRandom(stringLenght, valid);
        }

        private static string GenerateRandom(int length, string valids)
        {
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
                        s += character;
                    }
                }
            }
            return s;
        }
    }
}

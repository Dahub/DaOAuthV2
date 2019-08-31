using DaOAuthV2.Service.Interface;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace DaOAuthV2.Service.Test.Fake
{
    public class FakeEncryptionService : IEncryptionService
    {
        public bool AreEqualsSha256(string toCompare, byte[] hash)
        {
            var toReturn = false;

            using (var sha256 = new SHA256Managed())
            {
                var hashed = sha256.ComputeHash(Encoding.UTF8.GetBytes(toCompare));
                toReturn = hashed.SequenceEqual(hash);
            }

            return toReturn;
        }

        public byte[] Sha256Hash(string toHash)
        {
            using (var sha256 = new SHA256Managed())
            {
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(toHash));
            }
        }
    }
}

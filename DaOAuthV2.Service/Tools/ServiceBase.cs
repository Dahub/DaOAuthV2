using DaOAuthV2.Dal.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace DaOAuthV2.Service
{
    public abstract class ServiceBase
    {
        public AppConfiguration Configuration { get; set; }
        public IRepositoriesFactory Factory { get; set; }
        public string ConnexionString { get; set; }

        protected static bool AreEqualsSha256(string toCompare, byte[] hash)
        {
            bool toReturn = false;

            using (SHA256Managed sha256 = new SHA256Managed())
            {
                var hashed = sha256.ComputeHash(Encoding.UTF8.GetBytes(toCompare));
                toReturn = hashed.SequenceEqual(hash);
            }
            return toReturn;
        }

        protected static byte[] Sha256Hash(string toHash)
        {
            using (SHA256Managed sha256 = new SHA256Managed())
            {
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(toHash));
            }
        }

        protected void Validate(object toValidate)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(toValidate, null, null);
            if (!Validator.TryValidateObject(toValidate, context, results, true))
            {
                StringBuilder msg = new StringBuilder();
                foreach (var error in results)
                {
                    msg.AppendLine(error.ErrorMessage);
                }
                throw new DaOAuthServiceException(msg.ToString());
            }
        }
    }
}

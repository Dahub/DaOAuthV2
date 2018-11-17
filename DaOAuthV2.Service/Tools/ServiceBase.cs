using DaOAuthV2.Constants;
using DaOAuthV2.Dal.Interface;
using Microsoft.Extensions.Localization;
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
        public IRepositoriesFactory RepositoriesFactory { get; set; }
        public IStringLocalizerFactory StringLocalizerFactory { get; set; }
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

        protected void Validate<T>(T toValidate, Func<T, IList<ValidationResult>> extendValidate)
        {
            var results = extendValidate(toValidate);
            var context = new ValidationContext(toValidate, null, null);
            var local = this.GetDtoStringLocalizer();

            if (!Validator.TryValidateObject(toValidate, context, results, true) || results.Count > 0)
            {
                StringBuilder msg = new StringBuilder();

                foreach (var error in results)
                {
                    msg.AppendLine(local[error.ErrorMessage]);
                }

                throw new DaOAuthServiceException(msg.ToString());
            }
        }

        protected void Validate(object toValidate)
        {
            Validate(toValidate, x =>  new List<ValidationResult>());
        }

        protected IStringLocalizer GetErrorStringLocalizer()
        {
            return GetStringLocalizer(ResourceConstant.ErrorResource);
        }

        protected IStringLocalizer GetDtoStringLocalizer()
        {
            return GetStringLocalizer(ResourceConstant.DtoResource);
        }

        private IStringLocalizer GetStringLocalizer(string resourceName)
        {
            return StringLocalizerFactory.Create(resourceName, typeof(Program).Assembly.FullName);
        }
    }
}

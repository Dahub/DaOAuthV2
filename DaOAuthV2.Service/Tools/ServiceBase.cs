using DaOAuthV2.Constants;
using DaOAuthV2.Dal.Interface;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DaOAuthV2.Service
{
    public abstract class ServiceBase
    {
        public AppConfiguration Configuration { get; set; }
        public IRepositoriesFactory RepositoriesFactory { get; set; }
        public IStringLocalizerFactory StringLocalizerFactory { get; set; }
        public ILogger Logger { get; set; }
        public string ConnexionString { get; set; }

        protected void Validate<T>(T toValidate, Func<T, IList<ValidationResult>> extendValidate)
        {
            var local = this.GetDtoStringLocalizer();

            if (toValidate == null)
                throw new DaOAuthServiceException(local["ModelNull"]);

            var results = extendValidate(toValidate);
            if (results == null)
                results = new List<ValidationResult>();

            var context = new ValidationContext(toValidate, null, null);           

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

        protected IStringLocalizer GetMailStringLocalizer()
        {
            return GetStringLocalizer(ResourceConstant.MailResource);
        }

        protected int? GetClientTypeId(string clientType)
        {
            int? clientTypeId = null;
            if (!String.IsNullOrEmpty(clientType))
            {
                if (clientType.Equals(ClientTypeName.Confidential, StringComparison.OrdinalIgnoreCase))
                    clientTypeId = (int)EClientType.CONFIDENTIAL;
                else if (clientType.Equals(ClientTypeName.Public, StringComparison.OrdinalIgnoreCase))
                    clientTypeId = (int)EClientType.PUBLIC;
            }

            return clientTypeId;
        }

        private IStringLocalizer GetStringLocalizer(string resourceName)
        {
            return StringLocalizerFactory.Create(resourceName, typeof(Program).Assembly.FullName);
        }       
    }
}

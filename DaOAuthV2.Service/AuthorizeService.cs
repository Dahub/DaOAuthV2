using DaOAuthV2.Service.DTO;
using DaOAuthV2.Service.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace DaOAuthV2.Service
{
    public class AuthorizeService : ServiceBase, IAuthorizeService
    {
        public async Task<Uri> GenererateUriForAutorize(AskAuthorizeDto authorizeInfo)
        {
            IList<ValidationResult> ExtendValidation(AskAuthorizeDto toValidate)
            {
                var resource = this.GetErrorStringLocalizer();
                IList<ValidationResult> result = new List<ValidationResult>();

                if(!String.IsNullOrEmpty(toValidate.RedirectUri) && !IsUriCorrect(toValidate.RedirectUri))
                    result.Add(new ValidationResult(resource["AuthorizeRedirectUrlIncorrect"]));

                return result;
            }

            Validate(authorizeInfo, ExtendValidation);

            throw new NotImplementedException();
        }

        private static bool IsUriCorrect(string uri)
        {
            return Uri.TryCreate(uri, UriKind.Absolute, out Uri u);
        }
    }
}

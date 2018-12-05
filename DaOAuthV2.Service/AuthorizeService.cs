﻿using DaOAuthV2.Constants;
using DaOAuthV2.Service.DTO;
using DaOAuthV2.Service.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DaOAuthV2.Service
{
    public class AuthorizeService : ServiceBase, IAuthorizeService
    {
        public async Task<Uri> GenererateUriForAuthorize(AskAuthorizeDto authorizeInfo)
        {
            IList<ValidationResult> ExtendValidation(AskAuthorizeDto toValidate)
            {
                var resource = this.GetErrorStringLocalizer();
                IList<ValidationResult> result = new List<ValidationResult>();

                if(!String.IsNullOrEmpty(toValidate.RedirectUri) && !IsUriCorrect(toValidate.RedirectUri))
                    result.Add(new ValidationResult(resource["AuthorizeAuthorizeRedirectUrlIncorrect"]));

                return result;
            }

            Logger.LogInformation($"Ask for authorize, client : {authorizeInfo.ClientId} - response_type : {authorizeInfo.ResponseType}");

            var errorLocal = GetErrorStringLocalizer();
            Validate(authorizeInfo, ExtendValidation);

            // check for response_type
            if (String.IsNullOrEmpty(authorizeInfo.ResponseType))
                throw new DaOAuthRedirectException()
                {
                    RedirectUri = GenerateRedirectErrorMessage(
                        authorizeInfo.RedirectUri, 
                        OAuthConvention.ErrorNameInvalidRequest,
                        errorLocal["AuthorizeResponseTypeParameterMandatory"], 
                        authorizeInfo.State)
                };
            if(!authorizeInfo.ResponseType.Equals(OAuthConvention.ResponseTypeCode, StringComparison.Ordinal)
                && !authorizeInfo.ResponseType.Equals(OAuthConvention.ResponseTypeToken, StringComparison.Ordinal))
                throw new DaOAuthRedirectException()
                {
                    RedirectUri = GenerateRedirectErrorMessage(
                       authorizeInfo.RedirectUri,
                       OAuthConvention.ErrorNameUnsupportedResponseType,
                       errorLocal["AuthorizeUnsupportedResponseType"],
                       authorizeInfo.State)
                };

            // check client Id
            if (String.IsNullOrEmpty(authorizeInfo.ClientId))
                throw new DaOAuthRedirectException()
                {
                    RedirectUri = GenerateRedirectErrorMessage(
                        authorizeInfo.RedirectUri,
                        OAuthConvention.ErrorNameInvalidRequest,
                        errorLocal["AuthorizeClientIdParameterMandatory"],
                        authorizeInfo.State)
                };

            throw new NotImplementedException();
        }

        private static bool IsUriCorrect(string uri)
        {
            return Uri.TryCreate(uri, UriKind.Absolute, out Uri u);
        }

        private static Uri GenerateRedirectErrorMessage(string redirectUri, string errorName, string errorDescription, string stateInfo)
        {
            if (String.IsNullOrEmpty(stateInfo))
            {
                return new Uri($"{redirectUri}?error={errorName}&error_description={errorDescription}");
            }
            else
            {
                return new Uri($"{redirectUri}?error={errorName}&error_description={errorDescription}&state={stateInfo}");
            }
        }
    }
}
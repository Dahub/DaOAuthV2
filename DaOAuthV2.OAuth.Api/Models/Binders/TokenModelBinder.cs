﻿using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading.Tasks;

namespace DaOAuthV2.OAuth.Api.Models.Binders
{
    public class TokenModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var result = new TokenModel()
            {
                GrantType = bindingContext.ValueProvider.GetValue("grant_type").FirstValue,
                Code = bindingContext.ValueProvider.GetValue("code").FirstValue,
                ClientId = bindingContext.ValueProvider.GetValue("client_id").FirstValue,
                RefreshToken = bindingContext.ValueProvider.GetValue("refresh_token").FirstValue,
                Password = bindingContext.ValueProvider.GetValue("password").FirstValue,
                Username = bindingContext.ValueProvider.GetValue("username").FirstValue,
                Scope = bindingContext.ValueProvider.GetValue("scope").FirstValue,
                RedirectUrl = bindingContext.ValueProvider.GetValue("redirect_uri").FirstValue
            };

            bindingContext.Result = ModelBindingResult.Success(result);

            return Task.CompletedTask;
        }
    }
}
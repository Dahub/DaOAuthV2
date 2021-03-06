﻿using DaOAuthV2.Constants;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace DaOAuthV2.ApiTools
{
    public class TestAuthenticationOptions : AuthenticationSchemeOptions
    {
        public virtual ClaimsIdentity Identity { get; } = new ClaimsIdentity(
            new Claim[] 
            {
                new Claim(ClaimTypes.Name, "Sammy"),
                new Claim(ClaimTypes.Role, RoleName.Administrator),
                new Claim(ClaimTypes.Role, RoleName.User)
            }, 
            "test");
    }
}

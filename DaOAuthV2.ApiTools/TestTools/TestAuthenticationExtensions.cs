using Microsoft.AspNetCore.Authentication;
using System;

namespace DaOAuthV2.ApiTools
{
    public static class TestAuthenticationExtensions
    {
        public static AuthenticationBuilder AddTestAuth(
            this AuthenticationBuilder builder,
            Action<TestAuthenticationOptions> configureOptions)
        {
            return builder.AddScheme<TestAuthenticationOptions, TestAuthenticationHandler>("Test Scheme", "Test Auth", configureOptions);
        }
    }
}

using Microsoft.AspNetCore.Mvc;

namespace DaOAuthV2.OAuth.Api.Models
{
    /// <summary>
    /// Model to use to get a token
    /// </summary>
    [ModelBinder(BinderType = typeof(Binders.TokenModelBinder), Name = "TokenModel")]
    public class TokenModel
    {  
        /// <summary>
        /// Grant type
        /// </summary>
        public string GrantType { get; set; }

        /// <summary>
        /// Code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Redirect url
        /// </summary>
        public string RedirectUrl { get; set; }

        /// <summary>
        /// Client Id
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Refresh token
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Scope
        /// </summary>
        public string Scope { get; set; }
    }
}

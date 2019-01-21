namespace DaOAuthV2.OAuth.Api.Models
{
    /// <summary>
    /// Model to use to introspect a token
    /// </summary>
    public class IntrospectTokenModel
    {
        /// <summary>
        /// Token value
        /// </summary>
        public string Token { get; set; }
    }
}

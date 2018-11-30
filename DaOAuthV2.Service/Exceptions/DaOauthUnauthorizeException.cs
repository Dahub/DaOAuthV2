using System;

namespace DaOAuthV2.Service
{
    public class DaOauthUnauthorizeException : Exception
    {
        public DaOauthUnauthorizeException() : base() { }
        public DaOauthUnauthorizeException(string msg) : base(msg) { }
        public DaOauthUnauthorizeException(string msg, Exception ex) : base(msg, ex) { }
    }
}

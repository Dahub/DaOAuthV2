using System;

namespace DaOAuthV2.Service
{
    public class DaOAuthServiceException : Exception
    {
        public DaOAuthServiceException() : base() { }

        public DaOAuthServiceException(string msg) : base(msg) { }

        public DaOAuthServiceException(string msg, Exception ex) : base(msg, ex) { }
    }
}

using System;

namespace DaOAuthV2.Service
{
    public class DaOAuthNotFoundException : Exception
    {
        public DaOAuthNotFoundException() : base() { }

        public DaOAuthNotFoundException(string msg) : base(msg) { }

        public DaOAuthNotFoundException(string msg, Exception ex) : base(msg, ex) { }
    }
}

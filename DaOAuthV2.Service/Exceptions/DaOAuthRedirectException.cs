using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace DaOAuthV2.Service
{
    [Serializable]
    public class DaOAuthRedirectException : Exception
    {
        public Uri RedirectUri { get; set; }

        public DaOAuthRedirectException() : base() { }
        public DaOAuthRedirectException(string msg) : base(msg) { }
        public DaOAuthRedirectException(string msg, Exception ex) : base(msg, ex) { }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public DaOAuthRedirectException(SerializationInfo info, StreamingContext context): base(info, context) { }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("DaOAuthRedirectException.RedirectUri", this.RedirectUri);
        }
    }
}

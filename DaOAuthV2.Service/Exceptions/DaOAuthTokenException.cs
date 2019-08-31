using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace DaOAuthV2.Service
{
    [Serializable]
    public class DaOAuthTokenException : Exception
    {
        public string Error { get; set; }

        public string Description { get; set; }

        public string State { get; set; }

        public DaOAuthTokenException() : base() { }

        public DaOAuthTokenException(string msg) : base(msg) { }

        public DaOAuthTokenException(string msg, Exception ex) : base(msg, ex) { }


        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public DaOAuthTokenException(SerializationInfo info, StreamingContext context) : base(info, context) { }


        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("DaOAuthTokenException.Error", this.Error);
            info.AddValue("DaOAuthTokenException.Description", this.Description);
            info.AddValue("DaOAuthTokenException.State", this.State);
        }
    }
}

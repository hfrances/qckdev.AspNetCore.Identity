using System;
using System.Runtime.Serialization;

namespace qckdev.AspNetCore.Identity.AuthorizationFlow
{

    [Serializable]
    public class AuthorizationFlowException : Exception
    {

        public string Error { get; }
        public string ErrorDescription { get; }
        public string ErrorUri { get; }

        public AuthorizationFlowException(string error, string errorDescription, string errorUri) : base(error)
        {
            this.Error = error;
            this.ErrorDescription = errorDescription;
            this.ErrorUri = errorUri;
        }

        public AuthorizationFlowException(string error, string errorDescription, string errorUri, Exception inner) : base(error, inner)
        {
            this.Error = error;
            this.ErrorDescription = errorDescription;
            this.ErrorUri = errorUri;
        }

    }
}

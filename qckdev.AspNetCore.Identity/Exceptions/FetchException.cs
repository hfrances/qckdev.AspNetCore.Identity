using System;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;

namespace qckdev.AspNetCore.Identity.Exceptions
{

    [Serializable]
    public class FetchFailedException<TError> : HttpRequestException
    {

        public HttpStatusCode ResponseCode { get; }
        public TError Error { get; }

        public FetchFailedException(HttpStatusCode responseCode, string message, TError error) : base(message)
        {
            this.ResponseCode = responseCode;
            this.Error = error;
        }


        public FetchFailedException(HttpStatusCode responseCode, TError error, string message, Exception inner) : base(message, inner)
        {
            this.ResponseCode = responseCode;
            this.Error = error;
        }

    }
}

using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace qckdev.AspNetCore.Identity.Exceptions
{

    public class IdentityException : Exception
    {

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<IdentityError> Errors { get; }

        public IdentityException(string message)
            : this(message, new IdentityError[] { })
        {

        }

        public IdentityException(string message, IEnumerable<IdentityError> errors) 
            : base(message)
        {
            this.Errors = errors;
        }

    }
}

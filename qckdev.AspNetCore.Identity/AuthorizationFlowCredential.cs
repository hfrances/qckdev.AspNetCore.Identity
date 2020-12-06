﻿using System;

namespace qckdev.AspNetCore.Identity
{
    public sealed class AuthorizationFlowCredential
    {

        public string UserId { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string TokenType { get; set; }
        public string IdToken { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime IssuedUtc { get; set; }
        public long? ExpiresInSeconds { get; set; }

        public string Scope { get; set; }

    }
}

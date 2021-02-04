using Microsoft.Extensions.Configuration;
using System;

namespace qckdev.AspNetCore.Identity.JwtBearer
{
    public class JwtTokenConfiguration
    {

        public string Key { get; set; }
        public string ClientId { get; set; }
        public string Issuer { get; set; }
        public double AccessExpireSeconds { get; set; } = TimeSpan.FromDays(1).TotalSeconds;

    }
}

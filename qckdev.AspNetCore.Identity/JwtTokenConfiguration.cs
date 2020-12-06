using Microsoft.Extensions.Configuration;
using System;

namespace qckdev.AspNetCore.Identity
{
    public class JwtTokenConfiguration
    {

        public string Key { get; set; }
        public string ClientId { get; set; }
        public string Issuer { get; set; }
        public double AccessExpireSeconds { get; set; }


        public static JwtTokenConfiguration Get(IConfiguration configuration, string sectionName)
        {
            return new JwtTokenConfiguration
            {
                Key = configuration.GetSection(sectionName)["Key"],
                ClientId = configuration.GetSection(sectionName)["clientId"],
                Issuer = configuration.GetSection(sectionName)["Issuer"],
                AccessExpireSeconds = Convert.ToDouble(configuration.GetSection(sectionName)["AccessExpireSeconds"])
            };
        }

    }
}

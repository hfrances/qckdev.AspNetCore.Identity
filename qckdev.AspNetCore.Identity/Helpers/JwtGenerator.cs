using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using qckdev.Linq;

namespace qckdev.AspNetCore.Identity.Helpers
{
    static class JwtGenerator
    {

        public static dynamic CreateToken(SecurityKey key, IdentityUser user, IEnumerable<string> roles = null, IEnumerable<Claim> claims = null, TimeSpan? lifespan = null)
        {
            var tokenClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.UserName)
            };
            roles?.ForEach(rol => tokenClaims.Add(new Claim(ClaimTypes.Role, rol)));
            claims?.ForEach(val => tokenClaims.Add(val));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(tokenClaims),
                Expires = DateTime.UtcNow.Add(lifespan ?? TimeSpan.FromDays(1)),
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescription);

            return new
            {
                AccessToken = tokenHandler.WriteToken(token),
                Expired = token.ValidTo,
                RefreshToken = CreateGenericToken()
            };
        }

        public static ClaimsPrincipal GetPrincipalFromExpiredToken(string token, JwtTokenConfiguration configuration)
        {
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration.Key));
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false, //you might want to validate the audience and issuer depending on your use case
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters,
                out SecurityToken securityToken);

            if (!(securityToken is JwtSecurityToken jwtSecurityToken)
                || !jwtSecurityToken.Header.Alg.Equals(
                        SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }

        public static string CreateGenericToken()
        {
            var randomNumber = GetRandomSerie();

            return Convert.ToBase64String(randomNumber)
                //Avoid trouble with routes
                .Replace("/", "!");
        }

        private static byte[] GetRandomSerie()
        {
            var randomNumber = new byte[32];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }
            return randomNumber;
        }

    }
}

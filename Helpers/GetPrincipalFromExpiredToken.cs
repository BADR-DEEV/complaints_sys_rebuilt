using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using complaints_back.models;
using Microsoft.IdentityModel.Tokens;

namespace complaints_back.Helpers
{
    public class GetPrincipalFromExpiredToken
    {
        public PrincipalResult GetPrincipal(string? token, IConfiguration _config)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return new PrincipalResult { ErrorMessage = "Token is missing or empty." };
            }

            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(_config["AppSettings:Token"])),
                    ValidateLifetime = false // allow expired tokens
                };

                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

                if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                    !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return new PrincipalResult { ErrorMessage = "Invalid token algorithm." };
                }

                return new PrincipalResult { Principal = principal };
            }
            catch (SecurityTokenMalformedException)
            {
                return new PrincipalResult { ErrorMessage = "Malformed JWT — ensure it has 3 parts." };
            }
            catch (SecurityTokenInvalidSignatureException)
            {
                return new PrincipalResult { ErrorMessage = "Invalid token signature." };
            }
            catch (SecurityTokenException ex)
            {
                return new PrincipalResult { ErrorMessage = "Invalid token: " + ex.Message };
            }
            catch (Exception)
            {
                return new PrincipalResult { ErrorMessage = "Unexpected error during token validation." };
            }
        }
    }

}
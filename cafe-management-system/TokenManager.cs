using cafe_management_system.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web;

namespace cafe_management_system
{
    public class TokenManager
    {
        public static string Secret = "sdlfsknvkdosjfwprkslfjsodfiewjfwfjsldfjkfo132432lsdjfoweirwjfoiw";
        public static string GenerateToken(string email, string role)
        {
            // Token generation logic here
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret));
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                        new Claim(ClaimTypes.Email, email),
                        new Claim(ClaimTypes.Role, role)
                    }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
            };
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public static ClaimsPrincipal GetPrincipal(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(Secret);
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                };

                // Validate the token and extract the ClaimsPrincipal
                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                // Ensure the token is a valid JWT token
                if (validatedToken is JwtSecurityToken jwtToken)
                {
                    return principal;
                }

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static TokenClaim ValidateToken(string RawToken)
        {
            string[] arr = RawToken.Split(' ');
            var token = arr[1];
            ClaimsPrincipal principal = GetPrincipal(token);
            if(principal != null)
            {
                var email = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                var role = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                return new TokenClaim
                {
                    Email = email,
                    Role = role
                };
            }
            else
            {
                return null;
            }
        }
    }
}

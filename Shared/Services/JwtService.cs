using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shared.Model;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Services
{
    public interface IJwtService
    {
        public string GenerateToken(ClaimsPrincipal user);
        public JwtModel ValidateToken(string token);
        public JwtModel ValidateToken(HttpRequest req);
        public ClaimsPrincipal GenerateClaimsPrincipalFromUser(User user);
    }

    public class JwtService : IJwtService
    {
        private readonly IOptions<JwtSettings> _jwtSettings;
        public JwtService(IOptions<JwtSettings> settings)
        {
            _jwtSettings = settings;
        }
        public JwtModel ValidateToken(string token)
        {
            var jwtModel = new JwtModel();
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Value.Secret);
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);
                var jwt = (JwtSecurityToken)validatedToken;
                try
                {
                    jwtModel.Id = jwt.Claims.First(x => x.Type == "id").Value;
                }
                catch (Exception ex)
                {
                    jwtModel.Id = string.Empty;
                }

                jwtModel.Email = jwt.Claims.First(x => x.Type == "email").Value;
                jwtModel.Name = jwt.Claims.First(x => x.Type == "name").Value;
                return jwtModel;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public JwtModel ValidateToken(HttpRequest req)
        {
            var token = req.Headers["Authorization"].FirstOrDefault().Split(" ").Last();
            return ValidateToken(token);
        }
        public string GenerateToken(ClaimsPrincipal user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var secret = _jwtSettings.Value.Secret;
            var key = Encoding.ASCII.GetBytes(secret);
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = (ClaimsIdentity)user.Identity,
                Expires = DateTime.UtcNow.AddDays(3),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public ClaimsPrincipal GenerateClaimsPrincipalFromUser(User user)
        {

            var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim("name", user.Name));
            identity.AddClaim(new Claim("id", user.Id));
            identity.AddClaim(new Claim("email", user.Email));
            return new ClaimsPrincipal(identity);
        }
    }

    public static class JWTHelper
    {
        public static string GetToken(HttpRequest req)
        {
            var token = req.Headers["Authorization"].FirstOrDefault().Split(" ").Last();
            return token;
        }

        public static void AddTokenToHttpRequestMessage(string token, HttpRequestMessage msg)
        {
            msg.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}

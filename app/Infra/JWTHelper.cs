using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace app.Infra
{
    public static class JWTHelper
    {
        //public static IConfiguration Configuration { get; set; }
        public static string GenerateJwtToken(this Claim[] claims)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("THisISMySceret12");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(30),
                Audience = "*",
                Issuer = "*",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public static string Email(this ClaimsPrincipal User)
        {
           return User.Claims.FirstOrDefault(o => o.Type == "emailid").Value;
        }
        public static string UserName(this ClaimsPrincipal User)
        {
            return User.Claims.FirstOrDefault(o => o.Type == "UserName").Value;
        }
        public static bool IsConsumer(this ClaimsPrincipal User)
        {
            return User.Claims.FirstOrDefault(o => o.Type == "TypeUser").Value=="0";
        }
        public static bool IsServiceProvider(this ClaimsPrincipal User)
        {
            return User.Claims.FirstOrDefault(o => o.Type == "TypeUser").Value == "1";
        }
    
    }
}

using CurrencyExchange.Core.Entities.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace CurrencyExchange.Core.HelperFunctions
{
    public static class CreateToken
    {
        //public static string CreateUserToken(User user)
        //{
        //    List<Claim> claims = new List<Claim>
        //    {
        //        new Claim(ClaimTypes.Name, user.UserEmail)
        //    };
        //    var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
        //        _configuration.GetSection("AppSettings:Token").Value));

        //    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        //    var token = new JwtSecurityToken(
        //        claims: claims,
        //        expires: DateTime.Now.AddMonths(1),
        //        signingCredentials: creds
        //        );
        //    var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        //    return jwt;
        //}
        public static string GenerateToken(User user)
        {
            var mySecret = "asdv234234^&%&^%&^hjsdfb2%%%";
            var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(mySecret));

            var myIssuer = "http://mysite.com";
            var myAudience = "http://myaudience.com";

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
            new Claim(ClaimTypes.Name, user.UserEmail, ClaimTypes.Expiration),
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = myIssuer,
                Audience = myAudience,
                SigningCredentials = new SigningCredentials(mySecurityKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var token2 = tokenHandler.WriteToken(token);
            var jwtToken = new JwtSecurityToken(token2);
            var a =jwtToken.Claims;
            return tokenHandler.WriteToken(token);
        }
        public static string ParseToken(string token)
        {
            try
            {
                var jwtToken = new JwtSecurityToken(token);
                var expDate = jwtToken.Claims.ToList()[2].Value;
                if (expDate == null)
                    return null;
                var dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(Int64.Parse(expDate));
                if (DateTime.Now > dateTimeOffset)
                {
                    return "Token expired";
                }
                var userEmail = jwtToken.Claims.ToList()[0].Value;
                if (userEmail != null)
                    return userEmail;
                return null;

            }
            catch (Exception e)
            {
                return null;
            }
           
        }
    }
}

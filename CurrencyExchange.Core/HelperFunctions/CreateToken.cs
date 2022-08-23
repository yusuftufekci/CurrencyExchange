using System.IdentityModel.Tokens.Jwt;

namespace CurrencyExchange.Core.HelperFunctions
{
    public static class CreateToken
    {
        public static string ParseToken(string token)
        {
            try
            {
                var jwtToken = new JwtSecurityToken(token);
                var expDate = jwtToken.Claims.ToList()[2].Value;
                if (expDate == null) return null;
                var dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(Int32.Parse(expDate));
                if (DateTime.Now > dateTimeOffset)
                {
                    return "Token expired";
                }

                var userEmail = jwtToken.Claims.ToList()[0].Value;
                if (userEmail != null) return userEmail;
                return null;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
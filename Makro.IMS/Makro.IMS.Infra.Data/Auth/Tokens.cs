using Makro.IMS.Infra.Data.Models;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Makro.IMS.Infra.Data.Auth
{        
    public class Tokens
    {
        public static async Task<string> GenerateJwt(ClaimsIdentity identity, IJwtFactory jwtFactory, string userName, string refreshToken,
            JwtIssuerOptions jwtOptions, JsonSerializerSettings serializerSettings)
        {
            var response = new
            {                
                id=identity.Claims.Single(c=>c.Type =="id").Value,  
                AccessToken = await jwtFactory.GenerateEncodedToken(userName,identity),
                ExpiresIn = (int)jwtOptions.ValidFor.TotalSeconds,
                RefreshToken = refreshToken
            };

            return JsonConvert.SerializeObject(response, serializerSettings);
        }

        public static async Task<string> GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return await Task.FromResult(Convert.ToBase64String(randomNumber));
            }
        }

    }
}

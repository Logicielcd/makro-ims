using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Makro.IMS.Infra.Data.Auth
{
    public interface IJwtFactory
    {
        Task<string> GenerateEncodedToken(string userName, ClaimsIdentity identity);
        ClaimsIdentity GenerateClaimsIdentity(IEnumerable<Claim> userClaims, string userName, string id);
    }
}
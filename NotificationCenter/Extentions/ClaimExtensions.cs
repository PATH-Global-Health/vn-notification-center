using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NotificationCenter.Extentions
{
    public static class ClaimExtensions
    {
        public static Guid? GetId(this ClaimsPrincipal user)
        {
            var idClaim = user.Claims.FirstOrDefault(i => i.Type.Equals("Id"));
            if (idClaim != null)
            {
                return Guid.Parse(idClaim.Value);
            }
            return null;
        }

        public static bool IsHCDC(this ClaimsPrincipal user)
        {
            var idClaim = user.Claims.FirstOrDefault(i => i.Type.Equals("Username"));
            if (idClaim != null)
            {
                return (idClaim.Value.Equals("hcdc") || idClaim.Value.Equals("hcdc.dtr"));
            }
            return false;
        }
    }
}

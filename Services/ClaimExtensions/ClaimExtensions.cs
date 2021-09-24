using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Services.ClaimExtensions
{
    public static class ClaimExtensions
    {
        public static string GetId(this ClaimsPrincipal user)
        {
            var idClaim = user.Claims.FirstOrDefault(i => i.Type.Equals("Id"));
            if (idClaim != null)
            {
                return idClaim.Value;
            }
            return "";
        }
    }
}

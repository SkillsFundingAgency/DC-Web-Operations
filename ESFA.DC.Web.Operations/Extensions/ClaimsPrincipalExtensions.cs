using System.Linq;
using System.Security.Claims;
using ESFA.DC.Web.Operations;

namespace ESFA.DC.Web.Operations.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static long Ukprn(this ClaimsPrincipal claimsPrincipal)
        {
            long.TryParse(GetClaimValue(claimsPrincipal, IdamsClaimTypes.Ukprn), out var result);
            return result;
        }

        public static long Upin(this ClaimsPrincipal claimsPrincipal)
        {
            long.TryParse(GetClaimValue(claimsPrincipal, IdamsClaimTypes.Upin), out var result);
            return result;
        }

        public static string DisplayName(this ClaimsPrincipal claimsPrincipal)
        {
            return GetClaimValue(claimsPrincipal, IdamsClaimTypes.DisplayName);
        }

        public static string Name(this ClaimsPrincipal claimsPrincipal)
        {
            return GetClaimValue(claimsPrincipal, IdamsClaimTypes.Name);
        }

        public static string NameIdentifier(this ClaimsPrincipal claimsPrincipal)
        {
            return GetClaimValue(claimsPrincipal, IdamsClaimTypes.NameIdentifier);
        }

        public static string Email(this ClaimsPrincipal claimsPrincipal)
        {
            return GetClaimValue(claimsPrincipal, IdamsClaimTypes.Email);
        }

        private static string GetClaimValue(ClaimsPrincipal claimsPrincipal, string claimType)
        {
            return claimsPrincipal?.Claims?.FirstOrDefault(claim => claim.Type == claimType)?.Value;
        }
    }
}

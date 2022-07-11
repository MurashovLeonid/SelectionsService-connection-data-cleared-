using System;
using System.Security.Claims;

namespace Superbrands.Selection.WebApi.Extensions
{
    public static class IdentityUserExtensions
    {
        public static string GetUserId(this ClaimsPrincipal principal) =>
            principal.GetUserPropertyByClaim(ClaimTypes.NameIdentifier);

        private static string GetUserPropertyByClaim(this ClaimsPrincipal principal, string claim)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            return principal.FindFirstValue(claim);
        }
    }
}
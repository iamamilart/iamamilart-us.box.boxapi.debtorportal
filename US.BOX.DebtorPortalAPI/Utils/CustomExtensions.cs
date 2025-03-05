using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace US.BOX.DebtorPortalAPI.Utils
{
    public static class CustomExtensions
    {
        public static string GetUserName(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.FindFirst("username")?.Value;
        }

        public static int GetCaseNumber(this ClaimsPrincipal claimsPrincipal)
        {

            // Retrieve the username claim
            var caseNumberClaim = claimsPrincipal.FindFirst("username")?.Value;

            // Check if the claim is null or empty
            if (string.IsNullOrEmpty(caseNumberClaim) || !int.TryParse(caseNumberClaim, out var caseNumber) || caseNumber <= 0)
            {
                throw new SecurityTokenException("Invalid token: case number is missing or invalid.");
            }

            return caseNumber; // Return the valid case number
        }


    }
}

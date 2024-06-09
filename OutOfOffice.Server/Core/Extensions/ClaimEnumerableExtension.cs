using System.Security.Claims;

namespace OutOfOffice.Server.Core.Extensions;

public static class ClaimEnumerableExtension
{
    public static ulong GetUserId(this IEnumerable<Claim> claims) => 
        Convert.ToUInt64(claims.First(p=>p.Type == "userId").Value);

    public static string GetUserRole(this IEnumerable<Claim> claims) =>
        claims.First(p => p.Type == ClaimTypes.Role).Value;
}
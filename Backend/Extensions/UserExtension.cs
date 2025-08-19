using MongoDB.Bson;
using System.Security.Claims;

namespace Backend.Extensions;
public static class UserExtensions
{
    //public static string GetUserId(this ClaimsPrincipal user)
    //    => ObjectId.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)
    //                       ?? user.FindFirstValue(ClaimTypes.Name)
    //                       ?? user.FindFirstValue("sub")!);
    public static string GetUserId(this ClaimsPrincipal user)
    {
        return user.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}

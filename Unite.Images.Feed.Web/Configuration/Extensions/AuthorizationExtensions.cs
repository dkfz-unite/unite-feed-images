using Microsoft.AspNetCore.Authorization;
using Unite.Images.Feed.Web.Configuration.Constants;

namespace Unite.Images.Feed.Web.Configuration.Extensions;

public static class AuthorizationExtensions
{
    private const string PermissionClaimType = "permission";

    public static void AddAuthorizationOptions(this AuthorizationOptions options)
    {
        options.AddPolicy(Policies.Data.Writer, policy => policy
            .RequireClaim(PermissionClaimType, Permissions.Data.Write)
        );
    }
}

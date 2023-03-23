using CommonServer.Asp.AuthorizationHandlers;
using Microsoft.AspNetCore.Http;
using Types.Dtos;

namespace CommonServer.Utils.Extensions;

public static class HttpContextExtensions
{
    public static AccessJwtTokenContent? GetJwt(this HttpContext context) =>
        (AccessJwtTokenContent?)context.Items[JwtBearerAdsToContextHandler.UserItem];
}

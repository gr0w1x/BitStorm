using Microsoft.AspNetCore.SignalR;

namespace CommonServer.Asp.UserIdProviders;

public class UserIdByIdProvider : IUserIdProvider
{
    public virtual string GetUserId(HubConnectionContext connection)
    {
        return connection.User?.FindFirst(claim => claim.Type == "id")?.Value!;
    }
}

using Microsoft.AspNetCore.SignalR.Client;

namespace WebClient.Extensions;

public static class HubConnectionExtensions
{
    public static async Task<bool> StartWithRetry(this HubConnection connection, CancellationToken token, TimeSpan? delay = null)
    {
        delay ??= TimeSpan.FromSeconds(5);
        while(true)
        {
            try
            {
                await connection.StartAsync(token);
                return true;
            }
            catch when (token.IsCancellationRequested)
            {
                return false;
            }
            catch
            {
                await Task.Delay(delay.Value);
            }
        }
    }
}

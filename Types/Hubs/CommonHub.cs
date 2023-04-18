using Types.Dtos;

namespace Types.Hubs;

public interface ICommonHubClient
{
    public Task OnError(ErrorDto error);
}

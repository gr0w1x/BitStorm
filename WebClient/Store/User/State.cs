using Fluxor;
using Types.Entities;
using Types.Dtos;

namespace WebClient.Store.User;

[FeatureState]
public record UserState
{
    public AccessRefreshTokensDto? Tokens { get; init; } = null;

    public PublicUser? User { get; init; } = null;

    public bool Initialized { get; init; } = false;

    public bool Authorized =>
        Tokens != null;

    public bool HasAccess =>
        Authorized && Tokens.Access.Expires > DateTime.UtcNow;

    public bool CanRefresh =>
        Authorized && Tokens.Refresh.Expires > DateTime.UtcNow;
}

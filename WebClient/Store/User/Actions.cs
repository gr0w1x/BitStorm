using Types.Dtos;
using Types.Entities;

namespace WebClient.Store.User;

public record InitiateAction;

public record SetTokensAction(AccessRefreshTokensDto Tokens);

public record SetUserAction(PublicUser User);

public record LoadUserAction(Guid UserId);

public record SignOutAction;

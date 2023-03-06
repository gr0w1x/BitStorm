using Types.Dtos;
using Types.Entities;

namespace WebClient.Store.User;

public record SetTokensAction(AccessRefreshTokensDto Tokens);

public record SetUserAction(IUser User);

public class SignOutAction {};

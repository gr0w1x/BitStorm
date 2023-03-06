using Fluxor;

namespace WebClient.Store.User;

public static class UserReducers
{
    [ReducerMethod]
    public static UserState SetAccessReducer(UserState state, SetTokensAction action) =>
        state with
        {
            Tokens = action.Tokens
        };

    [ReducerMethod]
    public static UserState SetUser(UserState state, SetUserAction action) =>
        state with
        {
            User = state.HasAccess || state.CanRefresh ? action.User : null,
        };

    [ReducerMethod]
    public static UserState SignOut(UserState state, SignOutAction _) =>
        state with
        {
            Tokens = null,
            User = null,
        };
}

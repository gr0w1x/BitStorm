using Fluxor;
using WebClient.Store.Common;

namespace WebClient.Store.UserMenu;

public static class UserMenuReducers
{
    [ReducerMethod]
    public static UserMenuState SetState(UserMenuState state, SetUxState<UserMenuState> action) =>
        state with
        {
            UxState = action.State
        };
}

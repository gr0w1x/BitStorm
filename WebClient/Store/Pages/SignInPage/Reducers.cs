using Fluxor;
using WebClient.Store.Common;

namespace WebClient.Store.Pages.SignInPage;

public static class SignInPageReducers
{
    [ReducerMethod]
    public static SignInPageState SetState(SignInPageState state, SetUxState<SignInPageState> action) =>
        state with
        {
            UxState = action.State
        };

    [ReducerMethod]
    public static SignInPageState SetError(SignInPageState state, SetError<SignInPageState> action) =>
        state with
        {
            ServerError = action.Error
        };
}

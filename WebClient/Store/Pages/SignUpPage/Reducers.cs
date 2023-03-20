using Fluxor;
using WebClient.Store.Common;

namespace WebClient.Store.Pages.SignUpPage;

public static class SignUpPageReducers
{
    [ReducerMethod]
    public static SignUpPageState SetUxState(SignUpPageState state, SetUxState<SignUpPageState> action) =>
        state with
        {
            UxState = action.State
        };

    [ReducerMethod]
    public static SignUpPageState SetError(SignUpPageState state, SetError<SignUpPageState> action) =>
        state with
        {
            ServerError = action.Error
        };
}

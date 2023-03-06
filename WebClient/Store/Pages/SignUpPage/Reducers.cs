using Fluxor;
using WebClient.Store.Common;
using WebClient.Store.Pages.SignUpPage;

namespace WebClient.Store.Pages.SignPageUp;

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

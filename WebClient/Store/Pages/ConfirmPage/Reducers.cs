using Fluxor;
using WebClient.Store.Common;

namespace WebClient.Store.Pages.ConfirmPage;

public static class ConfirmPageReducers
{
    [ReducerMethod]
    public static ConfirmPageState SetState(ConfirmPageState state, SetUxState<ConfirmPageState> action) =>
        state with
        {
            UxState = action.State
        };

    [ReducerMethod]
    public static ConfirmPageState SetError(ConfirmPageState state, SetError<ConfirmPageState> action) =>
        state with
        {
            ServerError = action.Error
        };
}

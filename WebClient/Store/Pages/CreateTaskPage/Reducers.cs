using Fluxor;
using WebClient.Store.Common;

namespace WebClient.Store.Pages.CreateTaskPage;

public static class CreateTaskPageReducers
{
    [ReducerMethod]
    public static CreateTaskPageState SetState(CreateTaskPageState state, SetUxState<CreateTaskPageState> action) =>
        state with
        {
            UxState = action.State
        };

    [ReducerMethod]
    public static CreateTaskPageState SetError(CreateTaskPageState state, SetError<CreateTaskPageState> action) =>
        state with
        {
            ServerError = action.Error
        };
}

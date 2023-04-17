using Fluxor;
using WebClient.Store.Common;

namespace WebClient.Store.Pages.UpdateTaskPage;

public static class UpdateTaskPageReducers
{
    [ReducerMethod]
    public static UpdateTaskPageState SetState(UpdateTaskPageState state, SetUxState<UpdateTaskPageState> action) =>
        state with
        {
            UxState = action.State
        };

    [ReducerMethod]
    public static UpdateTaskPageState SetError(UpdateTaskPageState state, SetError<UpdateTaskPageState> action) =>
        state with
        {
            ServerError = action.Error
        };

    [ReducerMethod]
    public static UpdateTaskPageState SetTask(UpdateTaskPageState state, SetTaskAction action) =>
        state with
        {
            Task = action.Task
        };
}

using Fluxor;
using WebClient.Store.Common;

namespace WebClient.Store.Pages.TaskPage;

public static class TaskPageReducers
{
    [ReducerMethod]
    public static TaskPageState SetUxState(TaskPageState state, SetUxState<TaskPageState> action) =>
        state with
        {
            UxState = action.State
        };

    [ReducerMethod]
    public static TaskPageState SetError(TaskPageState state, SetError<TaskPageState> action) =>
        state with
        {
            ServerError = action.Error
        };

    [ReducerMethod]
    public static TaskPageState SetTask(TaskPageState state, SetTaskAction action) =>
        state with
        {
            Task = action.Task
        };
}

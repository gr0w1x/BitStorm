using Fluxor;
using WebClient.Store.Common;

namespace WebClient.Store.Pages.TasksSearchPage;

public static class TasksSearchPageReducers
{
    [ReducerMethod]
    public static TasksSearchPageState SetState(TasksSearchPageState state, SetUxState<TasksSearchPageState> action) =>
        state with
        {
            UxState = action.State
        };

    [ReducerMethod]
    public static TasksSearchPageState SetError(TasksSearchPageState state, SetError<TasksSearchPageState> action) =>
        state with
        {
            ServerError = action.Error
        };
}

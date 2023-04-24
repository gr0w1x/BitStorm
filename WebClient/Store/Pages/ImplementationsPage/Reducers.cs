using Fluxor;
using WebClient.Store.Common;

namespace WebClient.Store.Pages.ImplementationsPage;

public static class ImplementationsPageReducers
{
    [ReducerMethod]
    public static ImplementationsPageState SetState(ImplementationsPageState state, SetUxState<ImplementationsPageState> action) =>
        state with
        {
            UxState = action.State
        };

    [ReducerMethod]
    public static ImplementationsPageState SetError(ImplementationsPageState state, SetError<ImplementationsPageState> action) =>
        state with
        {
            ServerError = action.Error
        };

    [ReducerMethod]
    public static ImplementationsPageState SetTests(ImplementationsPageState state, SetOutputAction action) =>
        state with
        {
            Output = action.Output
        };

    [ReducerMethod]
    public static ImplementationsPageState SetOutput(ImplementationsPageState state, SetConnected action) =>
        state with
        {
            Connected = action.Connected
        };

    [ReducerMethod]
    public static ImplementationsPageState SetTask(ImplementationsPageState state, SetTaskAction action) =>
        state with
        {
            Task = action.Task
        };
}

using Blazorise;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Types.Dtos;
using Types.Entities;
using Types.Hubs;
using Types.Languages;
using WebClient.Constants;
using WebClient.Extensions;
using WebClient.Services;
using WebClient.Store.Common;
using WebClient.Store.Pages.ImplementationsPage;
using WebClient.Typing;
using WebClient.Views.Components;

namespace WebClient.Views.Pages.Tasks.ImplementationsPage;

public partial class ImplementationsPage: ReduxComponent<ImplementationsPageState>
{
    [Parameter]
    public List<TaskImplementation> ExistedImplementations { get; set; }

    [Inject]
    protected ApiClient Client { get; set; }
    [Inject]
    protected TasksService TasksService { get; set; }
    [Inject]
    protected TaskImplementationsService TaskImplementationsService { get; set; }

    protected IDictionary<SelectedLanguageVersion, SaveImplementationCodeDto> Implementations =
        new Dictionary<SelectedLanguageVersion, SaveImplementationCodeDto>();
    protected SelectedLanguageVersion LanguageVersion { get; set; }
    protected Task OnLanguageVersionChanged(SelectedLanguageVersion languageVersion)
    {
        if (!Implementations!.ContainsKey(languageVersion))
        {
            var existed = ExistedImplementations.Find(
                impl =>
                    impl.Language == languageVersion.Language &&
                    impl.Version == languageVersion.Version
            );
            if (existed != null)
            {
                Implementations[languageVersion] =
                    new SaveImplementationCodeDto()
                    {
                        TaskId = existed.TaskId,
                        Language = existed.Language,
                        Version = existed.Version,
                        Details = existed.Details,
                        InitialSolution = existed.InitialSolution,
                        CompletedSolution = existed.CompletedSolution,
                        PreloadedCode = existed.PreloadedCode,
                        ExampleTests = existed.ExampleTests,
                        Tests = existed.Tests
                    };
            }
            else
            {
                Implementations[languageVersion] =
                    new SaveImplementationCodeDto()
                    {
                        TaskId = ComponentState.Value.Task!.Id,
                        Language = languageVersion.Language,
                        Version = languageVersion.Version
                    };
            }
        }
        LanguageVersion = languageVersion;
        return Task.CompletedTask;
    }
    protected SaveImplementationCodeDto CurrentImplementation => Implementations![LanguageVersion!];

    protected readonly CancellationTokenSource tokenSource = new ();
    protected HubConnection? Connection;

    protected string DetailsTab { get; set; } = "details";
    protected string SolutionsTab { get; set; } = "completed";
    protected bool Submitable =>
        !ComponentState.Value.UxState.Is(UxState.Loading) && ComponentState.Value.Connected;
    protected Modal deleteModalRef;

    protected override async Task OnInitializedAsync()
    {
        var languageVersion = ExistedImplementations.Any()
            ?
                new SelectedLanguageVersion()
                {
                    Language = ExistedImplementations[0].Language,
                    Version = ExistedImplementations[0].Version
                }
            :
                new SelectedLanguageVersion()
                {
                    Language = CodeLanguages.Languages[0].Code,
                    Version = CodeLanguages.Languages[0].DefaultVersion.Version
                };
        await OnLanguageVersionChanged(languageVersion);

        base.OnInitialized();

        Connection = new HubConnectionBuilder()
            .WithAutomaticReconnect()
            .WithUrl(new Uri(Client.BaseAddress!, "/api/executions/hub"), options => {
                options.AccessTokenProvider =
                    async () => {
                        if (!Client.UserState.Value.HasAccess)
                        {
                            await Client.TryRefresh();
                        }
                        return Client.UserState.Value.Tokens?.Access.Token;
                    };
            })
            .Build();

        Connection.Closed += Connect;

        Connection.On<ExecuteCodeResultDto>(nameof(IExecutionsHubClient.OnCodeExecuted), OnCodeExecuted);
        Connection.On<ErrorDto>(nameof(IExecutionsHubClient.OnError), OnError);
        Connection.On<TaskImplementationWithSecretDto>(nameof(IExecutionsHubClient.OnImplementationSaved), OnImplementationSaved);

        await Connect(null);
    }

    protected async Task Connect(Exception? _e)
    {
        Dispatcher.Dispatch(new SetConnected(false));

        if (await Connection!.StartWithRetry(tokenSource.Token))
        {
            Dispatcher.Dispatch(new SetConnected(true));
        }
    }

    protected Task OnCodeExecuted(ExecuteCodeResultDto results)
    {
        Dispatcher.Dispatch(new SetOutputAction(results));
        return Task.CompletedTask;
    }

    protected async Task OnImplementationSaved(TaskImplementationWithSecretDto _)
    {
        await NotificationService.Success("implementation successfully changes");
        Dispatcher.Dispatch(new SetUxState<ImplementationsPageState>(UxState.Success));
    }

    protected async Task OnError(ErrorDto error)
    {
        await NotificationService.Error(error.Message, SnackbarConstants.DefaultErrorHeader);
        Dispatcher.Dispatch(new SetUxState<ImplementationsPageState>(UxState.Error));
    }

    protected async Task OnSave()
    {
        DetailsTab = "output";
        Dispatcher.Dispatch(new SetOutputAction(null));
        Dispatcher.Dispatch(new SetUxState<ImplementationsPageState>(UxState.Loading));

        await Connection!.SendAsync(
            nameof(IExecutionsHubServer.SaveImplementationCode),
            CurrentImplementation with
            {
                Details = string.IsNullOrWhiteSpace(CurrentImplementation.Details)
                    ? null
                    : CurrentImplementation.Details,
                PreloadedCode = string.IsNullOrWhiteSpace(CurrentImplementation.PreloadedCode)
                    ? null
                    : CurrentImplementation.PreloadedCode,
            }
        );
    }

    protected async Task OnDelete()
    {
        Dispatcher.Dispatch(new SetUxState<ImplementationsPageState>(UxState.Loading));
        await deleteModalRef.Hide();
        try
        {
            await TaskImplementationsService.Delete(new TaskImplementationId(
                ComponentState.Value.Task!.Id,
                LanguageVersion.Language,
                LanguageVersion.Version
            ));
            await NotificationService.Success("Deleted!");
            Dispatcher.Dispatch(new SetUxState<ImplementationsPageState>(UxState.Success));
        }
        catch (Exception e)
        {
            await NotificationService.Error(e.Message, SnackbarConstants.DefaultErrorHeader);
            Dispatcher.Dispatch(new SetUxState<ImplementationsPageState>(UxState.Error));
        }
    }

    public override void Dispose()
    {
        if (Connection != null)
        {
            Connection.Closed -= Connect;
            Connection.Remove(nameof(IExecutionsHubClient.OnCodeExecuted));
            Connection.StopAsync(tokenSource.Token);
        }
        GC.SuppressFinalize(this);
        base.Dispose();
    }
}

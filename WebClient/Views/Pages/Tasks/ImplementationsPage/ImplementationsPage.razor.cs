using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Types.Dtos;
using Types.Entities;
using Types.Hubs;
using Types.Languages;
using WebClient.Extensions;
using WebClient.Services;
using WebClient.Store.Common;
using WebClient.Store.Pages.ImplementationsPage;
using WebClient.Typing;
using WebClient.Views.Components;

namespace WebClient.Views.Pages.Tasks.ImplementationsPage;

public record ImplementationDto
{
    public string Details { get; set; } = "";
    public string TestCases { get; set; } = "";
    public string ExampleTestCases { get; set; } = "";
    public string CompletedSolution { get; set; } = "";
    public string InitialSolution { get; set; } = "";
    public string Preloaded { get; set; } = "";
}

public partial class ImplementationsPage: PageComponent<ImplementationsPageState>, IDisposable
{
    [Parameter]
    public string TaskId { get; set; }

    public string DetailsTab { get; set; } = "details";
    public string SolutionsTab { get; set; } = "completed";

    private IDictionary<SelectedLanguageVersion, ImplementationDto> Implementations =
        new Dictionary<SelectedLanguageVersion, ImplementationDto>();

    public SelectedLanguageVersion LanguageVersion { get; set; }
    public ImplementationDto CurrentImplementation => Implementations![LanguageVersion!];

    public Task OnLanguageVersionChanged(SelectedLanguageVersion languageVersion)
    {
        if (!Implementations!.ContainsKey(languageVersion))
        {
            Implementations[languageVersion] = new ImplementationDto();
        }
        LanguageVersion = languageVersion;
        return Task.CompletedTask;
    }

    private readonly CancellationTokenSource tokenSource = new ();
    protected HubConnection? Connection;

    [Inject]
    private ApiClient Client { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var first = CodeLanguages.Languages[0];
        LanguageVersion = new SelectedLanguageVersion()
        {
            Language = first.Code,
            Version = first.DefaultVersion.Version,
        };
        Implementations[LanguageVersion] = new ImplementationDto();
        base.OnInitialized();

        if (Client.UserState.Value.Authorized)
        {
            Connection = new HubConnectionBuilder()
                .WithAutomaticReconnect()
                .WithUrl(new Uri(Client.BaseAddress!, "/api/executions/hub"), options => {
                    options.AccessTokenProvider =
                        () => Task.FromResult(Client.UserState.Value.Tokens?.Access.Token);
                })
                .Build();

            Connection.Closed += OnConnectionClosed;

            Connection.On<ExecuteCodeResultDto>(nameof(IExecutionsClient.OnImplementationCodeSaved),
                results =>
                {
                    Dispatcher.Dispatch(new SetOutputAction(results));
                    Dispatcher.Dispatch(new SetUxState<ImplementationsPageState>(results.ExitStatus == 0 ? UxState.Success : UxState.Error));
                }
            );

            await OnConnectionClosed(null);
        }
    }

    protected async Task OnConnectionClosed(Exception? _e)
    {
        Dispatcher.Dispatch(new SetConnected(false));

        if (!Client.UserState.Value.HasAccess)
        {
            await Client.TryRefresh();
        }

        if (await Connection.StartWithRetry(tokenSource.Token))
        {
            Dispatcher.Dispatch(new SetConnected(true));
        }
    }

    protected bool Submitable =>
        !ComponentState.Value.UxState.Is(UxState.Loading) && ComponentState.Value.Connected;

    protected async Task OnSave()
    {
        DetailsTab = "output";
        Dispatcher.Dispatch(new SetOutputAction(null));
        Dispatcher.Dispatch(new SetUxState<ImplementationsPageState>(UxState.Loading));

        await Connection.SendAsync(nameof(IExecutionsServer.SaveImplementationCode), new SaveImplementationCodeDto()
        {
            TaskId = new Guid(TaskId),
            Language = LanguageVersion.Language,
            Version = LanguageVersion.Version,
            InitialSolution = CurrentImplementation.InitialSolution,
            CompletedSolution = CurrentImplementation.CompletedSolution,
            Preloaded = CurrentImplementation.Preloaded,
            ExampleTestCases = CurrentImplementation.ExampleTestCases,
            TestCases = CurrentImplementation.TestCases
        });
    }

    public override void Dispose()
    {
        if (Connection != null)
        {
            Connection.Closed -= OnConnectionClosed;
            Connection.StopAsync(tokenSource.Token);
            Connection.Remove(nameof(IExecutionsClient.OnImplementationCodeSaved));
        }
        GC.SuppressFinalize(this);
        base.Dispose();
    }
}

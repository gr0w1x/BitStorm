using Types.Dtos;
using WebClient.Models;
using WebClient.Services;
using Microsoft.AspNetCore.Components;
using System.Text.Json;
using WebClient.Store.Pages.TasksSearchPage;
using WebClient.Store.Common;
using WebClient.Typing;
using WebClient.Extensions;
using WebClient.Views.Components;
using WebClient.Views.Components.InfiniteLoaderList;
using WebClient.Constants;
using System.Web;

namespace WebClient.Views.Pages.Tasks.TasksSearchPage;

public partial class TasksSearchPage
{
    [Inject]
    TasksService TasksService { get; set; }

    [Parameter]
    [SupplyParameterFromQuery(Name = "query")]
    public string? Query { get; set; }

    protected override PageAccessType PageAccess => PageAccessType.ForAll;

    private InfiniteLoaderList<TaskCardModel> tasksList;

    private GetTasksInfoDto GetTasksInfo { get; set; } =
        new GetTasksInfoDto()
        {
            Query = "",
            Languages = Array.Empty<string>(),
            Status = GetTasksInfoDto.StatusOptions.All,
            Levels = Array.Empty<int>(),
            Tags = Array.Empty<string>()
        };

    private TasksInfoDto TasksInfo { get; set; } = new TasksInfoDto()
    {
        Total = 0,
        Tags = new Dictionary<string, int>()
    };

    private GetTasksDto.SortStrategy SortStrategy { get; set; } = GetTasksDto.SortStrategy.LastUpdated;
    private bool SortInversed { get; set; } = false;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        if (Query != null)
        {
            try
            {
                var dto = JsonSerializer.Deserialize<GetTasksDto>(HttpUtility.HtmlDecode(Query!))!;
                GetTasksInfo = dto;
                if (dto.Sort != null)
                {
                    SortStrategy = dto.Sort.Value;
                }
                if (dto.Inversed != null)
                {
                    SortInversed = dto.Inversed.Value;
                }
            }
            catch { }
        }
        await LoadTasksInfo(GetTasksInfo);
    }

    private bool Disabled => ComponentState.Value.UxState.Is(UxState.Loading);

    async Task LoadTasksInfo(GetTasksInfoDto dto)
    {
        try
        {
            Dispatcher.Dispatch(new SetUxState<TasksSearchPageState>(UxState.Loading));
            GetTasksInfo = dto;
            TasksInfo = await TasksService.GetTasksInfo(GetTasksInfo);
            Navigation.NavigateTo(Routes.TasksSearchPageWith(GetTasks(0)));
            Dispatcher.Dispatch(new SetUxState<TasksSearchPageState>(UxState.Success));
        }
        catch (Exception ex)
        {
            Dispatcher.Dispatch(new SetUxState<TasksSearchPageState>(UxState.Error));
            Dispatcher.Dispatch(new SetError<TasksSearchPageState>(ex.Message));
        }
    }

    async Task ChangeSortStrategy(GetTasksDto.SortStrategy strategy, bool inversed)
    {
        Dispatcher.Dispatch(new SetUxState<TasksSearchPageState>(UxState.Loading));
        SortStrategy = strategy;
        SortInversed = inversed;
        Navigation.NavigateTo(Routes.TasksSearchPageWith(GetTasks(0)));
        await tasksList.Clear();
        Dispatcher.Dispatch(new SetUxState<TasksSearchPageState>(UxState.Success));
    }

    private GetTasksDto GetTasks(int skip) => new()
    {
        Query = GetTasksInfo.Query,
        Status = GetTasksInfo.Status,
        Languages = GetTasksInfo.Languages,
        Levels = GetTasksInfo.Levels,
        Tags = GetTasksInfo.Tags,
        Sort = SortStrategy,
        Inversed = SortInversed,
        Skip = skip,
        Take = 10
    };

    async Task<IEnumerable<TaskCardModel>> GetItems(int skip, CancellationToken _)
    {
        try
        {
            return (await TasksService.GetTasks(GetTasks(skip)))
                .Select(task => new TaskCardModel()
                {
                    Task = task
                });
        }
        catch (Exception ex)
        {
            Dispatcher.Dispatch(new SetError<TasksSearchPageState>(ex.Message));
            return Array.Empty<TaskCardModel>();
        }
    }
}

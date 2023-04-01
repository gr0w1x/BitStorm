using Types.Dtos;
using Types.Languages;
using Types.Entities;
using WebClient.Models;

namespace WebClient.Views.Pages.Tasks.TasksSearchPage;

public partial class TasksSearchPage
{
    private static readonly string[] _tagItems = new string[] {};

    private static readonly GetTasksInfoDto.StatusOptions[] _statusOptionsItems = new GetTasksInfoDto.StatusOptions[]
    {
        GetTasksInfoDto.StatusOptions.OnlyApproved,
        GetTasksInfoDto.StatusOptions.OnlyBeta,
        GetTasksInfoDto.StatusOptions.All
    };

    private static readonly int[] _rankItems = Enumerable.Range(1, 9).ToArray();

    private static readonly LanguageDto _all = new LanguageDto()
    {
        Code = "all",
        Name = "All Languages"
    };

    private LanguageDto CurrentLanguage => _languageItems[_dto.Languages!.First()];

    private static readonly Dictionary<string, LanguageDto> _languageItems =
        new LanguageDto[] { _all }
            .Concat(CodeLanguages.Languages)
            .ToDictionary(language => language.Code);

    private GetTasksInfoDto _dto { get; set; } =
        new GetTasksInfoDto()
        {
            Query = "",
            Languages = new string[] { _all.Code },
            Status = GetTasksInfoDto.StatusOptions.All,
            Levels = Array.Empty<int>(),
            Tags = Array.Empty<string>()
        };

    async Task<IEnumerable<TaskCardModel>> GetItems(int skip, CancellationToken token)
    {
        await Task.Delay(1000, token);
        return Enumerable.Range(skip, 50).Select(id => new TaskCardModel() {
            Task = new Task_() {
                Title = $"Task # {id + 1}",
                Level = (id % 9) + 1,
                Beta = (id % 10) == 0,
                Tags = new List<TaskTag>()
                {
                    new TaskTag() { Id = "Tag 1" },
                    new TaskTag() { Id = "Tag 2" },
                    new TaskTag() { Id = "Tag 3" },
                },
                Visibility = TaskVisibility.Public,
                Likes = id * 100,
                CreatedAt = DateTime.Now.AddDays(-3),
                UpdatedAt = (id % 5) == 0 ? DateTime.Now.AddHours(-3) : null,
                Implementations = new List<TaskImplementation> ()
            }
        });
    }
}

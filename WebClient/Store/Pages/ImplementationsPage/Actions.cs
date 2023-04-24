using Types.Dtos;
using Types.Entities;

namespace WebClient.Store.Pages.ImplementationsPage;

public record SetOutputAction(ExecuteCodeResultDto? Output);

public record SetConnected(bool Connected);

public record SetTaskAction(Task_? Task);

using Types.Dtos;

namespace WebClient.Store.Pages.ImplementationsPage;

public record SetOutputAction(ExecuteCodeResultDto? Output);

public record SetConnected(bool Connected);

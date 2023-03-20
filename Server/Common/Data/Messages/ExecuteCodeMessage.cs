namespace CommonServer.Data.Messages;

public record ExecuteCodeMessage(string Preloaded, string Solution, string Tests);

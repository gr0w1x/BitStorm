namespace CommonServer.Data.Messages;

public record MailMessage(
    string? Title,
    string Body,
    string? SenderName,
    string[] Receivers
);

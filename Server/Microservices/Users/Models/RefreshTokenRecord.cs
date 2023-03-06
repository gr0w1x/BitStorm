using Microsoft.EntityFrameworkCore;

namespace Users.Models;

[PrimaryKey(nameof(Token))]
public record RefreshTokenRecord(string Token, Guid UserId, DateTimeOffset Expired);

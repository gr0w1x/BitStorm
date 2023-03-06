namespace Types.Entities;

[Flags]
public enum UserRoles
{
    None      = 0,
    Common    = 1 << 0,
    Moderator = 1 << 1
}

public interface IUser: IHasId
{
    UserRoles Roles { get; set; }

    string Username { get; set; }
    string? Name { get; set; }

    string? Avatar { get; set; }
    int Trophies { get; }

    public DateTimeOffset Registered { get; }
    public DateTimeOffset LastSeen { get; }
}

public interface IUserCredentials
{
    string Email { get; set; }
    string Password { get; set; }
    bool Confirmed { get; set; }
}

public interface IUserSocial
{
    string? GitHubProfile { get; set; }
    string? TwitterUsername { get; set; }
    string? LinkedIdProfile { get; set; }
    string? StackOverflow { get; set; }
}

public static class UserConstants
{
    public const int UsernameLengthMin = 1;
    public const int UsernameLenghtMax = 50;
    public const int EmailLengthMin = 1;
    public const int EmailLengthMax = 100;
    public const int PasswordLengthMin = 1;
    public const int PasswordLengthMax = 256;
    public static readonly TimeSpan ConfirmPeriod = TimeSpan.FromDays(3);
}

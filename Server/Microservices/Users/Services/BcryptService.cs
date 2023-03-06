namespace Users.Services;

public interface IHasher
{
    string Hash(string origin);
    bool Compare(string text, string hashed);
}

public class BcryptService: IHasher
{
    private readonly int _salt;

    public BcryptService(int salt)
    {
        _salt = salt;
    }

    public string Hash(string origin) => BCrypt.Net.BCrypt.HashPassword(origin, BCrypt.Net.BCrypt.GenerateSalt(_salt));
    public bool Compare(string text, string hashed)
    {
        try
        {
            return BCrypt.Net.BCrypt.Verify(text, hashed);
        }
        catch
        {
            return false;
        }
    }
}

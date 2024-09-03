using Domain;

namespace Application;

public interface IAuthService
{
    Task<User?> GetUserByUsername(string username);

    Task<User> RegisterUser(string username, string passwordHash, string passwordSalt);
}
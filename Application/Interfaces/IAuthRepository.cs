using Domain;

namespace Application;

public interface IAuthRepository
{
    Task<User?> GetUserByUsername(string userName);
    Task<User> RegisterUser(string username, string passwordHash, string passwordSalt);
}
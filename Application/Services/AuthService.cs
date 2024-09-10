using Domain;

namespace Application;

public class AuthService(IAuthRepository authRepository) : IAuthService
{
    public async Task<User?> GetUserByUsername(string username)
    {
        return await authRepository.GetUserByUsername(username);
    }

    public async Task<User> RegisterUser(string username, string passwordHash, string passwordSalt)
    {
        return await authRepository.RegisterUser(username, passwordHash, passwordSalt);
    }

    public async Task SetUserRefreshToken(Guid userId, string refreshToken)
    {
         await authRepository.SetUserRefreshToken(userId, refreshToken);
    }
}
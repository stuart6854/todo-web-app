using Application;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class AuthRepository(AppDbContext dbContext) : IAuthRepository
{
    public async Task<User?> GetUserByUsername(string userName)
    {
        return await dbContext.Users.FirstOrDefaultAsync(u => u.Username == userName);
    }

    public async Task<User> RegisterUser(string username, string passwordHash, string passwordSalt)
    {
        var user = new User
        {
            Username = username,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
        };
        dbContext.Users.Attach(user);
        await dbContext.SaveChangesAsync();
        return user;
    }
}
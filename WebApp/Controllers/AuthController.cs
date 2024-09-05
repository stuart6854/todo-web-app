using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application;
using Domain;
using Domain.Models;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace WebApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService authService, IConfiguration configuration) : ControllerBase
{
    private const int HashSize = 64;
    private const int Iterations = 35000;
    private static readonly HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA256;

    [HttpPost("[action]")]
    public async Task<IActionResult> Register([FromBody] RegisterModel registerModel)
    {
        var response = new ApiResponse<LoginResponseModel>();

        var user = await authService.GetUserByUsername(registerModel.Username);
        if (user != null)
        {
            response.Success = false;
            response.ErrorMessage = "Username is already taken!";
        }
        else
        {
            var passwordSalt = GeneratePasswordSaltHash();
            var passwordHash = HashPassword(registerModel.Password, passwordSalt);
            user = await authService.RegisterUser(registerModel.Username, passwordHash, Convert.ToHexString(passwordSalt));

            var token = GenerateJwtToken(user.Id, user.Username);
            response.Data = new LoginResponseModel { Token = token };
        }

        return Ok(response);
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
    {
        var response = new ApiResponse<LoginResponseModel>();

        var user = await authService.GetUserByUsername(loginModel.Username);
        if (user == null)
        {
            response.Success = false;
            response.ErrorMessage = "Invalid username or password.";
        }
        else
        {
            var storedSalt = Convert.FromHexString(user.PasswordSalt);
            var hashedPassword = HashPassword(loginModel.Password, storedSalt);
            if (hashedPassword != user.PasswordHash)
            {
                response.Success = false;
                response.ErrorMessage = "Invalid username or password.";
            }
            else
            {
                var token = GenerateJwtToken(user.Id, user.Username);
                response.Data = new LoginResponseModel { Token = token };
            }
        }

        return Ok(response);
    }

    private string GenerateJwtToken(Guid userId, string username)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, username == "Admin" ? "Admin" : "User"),
        };
        var secret = configuration.GetValue<string>(Constants.JwtSecretName);
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "stuartmillman",
            audience: "stuartmillman",
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creds
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static byte[] GeneratePasswordSaltHash()
    {
        return RandomNumberGenerator.GetBytes(HashSize);
    }

    private static string HashPassword(string password, byte[] salt)
    {
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            salt,
            Iterations,
            HashAlgorithm,
            HashSize
        );
        return Convert.ToHexString(hash);
    }
}
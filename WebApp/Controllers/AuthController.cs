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

            var token = GenerateJwtToken(user.Id, user.Username, isRefreshToken: false);
            var refreshToken = GenerateJwtToken(user.Id, user.Username, isRefreshToken: true);

            // Save refresh token to database
            await authService.SetUserRefreshToken(user.Id, refreshToken);

            response.Data = new LoginResponseModel
            {
                Token = token,
                TokenExpired = DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds(),
                RefreshToken = refreshToken,
            };
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
                var token = GenerateJwtToken(user.Id, user.Username, isRefreshToken: false);
                var refreshToken = GenerateJwtToken(user.Id, user.Username, isRefreshToken: true);

                // Save refresh token to database
                await authService.SetUserRefreshToken(user.Id, refreshToken);

                response.Data = new LoginResponseModel
                {
                    Token = token,
                    // TokenExpired = DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds(),
                    TokenExpired = DateTimeOffset.UtcNow.AddMinutes(9).ToUnixTimeSeconds(),
                    RefreshToken = refreshToken,
                };
            }
        }

        return Ok(response);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> LoginByRefreshToken(string refreshToken)
    {
        var secret = configuration.GetValue<string>(Constants.JwtRefreshSecretName);
        var claimsPrincipal = GetClaimsPrincipalFromToken(refreshToken, secret);
        if (claimsPrincipal == null)
        {
            return BadRequest();
        }

        var userId = Guid.Parse(claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty);
        var username = claimsPrincipal.FindFirstValue(ClaimTypes.Name) ?? string.Empty;
        var user = await authService.GetUserByUsername(username);
        if (user == null)
        {
            return BadRequest();
        }

        if (user.RefreshToken != refreshToken)
        {
            return BadRequest();
        }

        var newToken = GenerateJwtToken(userId, username, isRefreshToken: false);
        var newRefreshToken = GenerateJwtToken(userId, username, isRefreshToken: true);

        // Save refresh token to database
        await authService.SetUserRefreshToken(userId, newRefreshToken);

        var response = new ApiResponse<LoginResponseModel>();
        response.Data = new LoginResponseModel
        {
            Token = newToken,
            TokenExpired = DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds(),
            RefreshToken = newRefreshToken,
        };
        return Ok(response);
    }

    private string GenerateJwtToken(Guid userId, string username, bool isRefreshToken)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, username == "Admin" ? "Admin" : "User"),
        };
        var secret = configuration.GetValue<string>(isRefreshToken ? Constants.JwtRefreshSecretName : Constants.JwtSecretName);
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "stuartmillman",
            audience: "stuartmillman",
            claims: claims,
            // expires: DateTime.Now.AddHours(isRefreshToken ? 24 : 1),
            expires: DateTime.Now.AddMinutes(isRefreshToken ? 24 * 60 : 9),
            signingCredentials: creds
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static ClaimsPrincipal GetClaimsPrincipalFromToken(string token, string secret)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(secret);
        try
        {
            var principal = tokenHandler.ValidateToken(
                token,
                new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidAudience = "stuartmillman",
                    ValidateIssuer = true,
                    ValidIssuer = "stuartmillman",
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                },
                out var validatedToken
            );
            return principal;
        }
        catch (Exception ex)
        {
            return null;
        }
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
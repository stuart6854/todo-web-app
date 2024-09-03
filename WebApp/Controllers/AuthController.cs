using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application;
using Azure.Identity;
using Domain.Models;
using Microsoft.AspNetCore.Identity.Data;
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
        var user = await authService.GetUserByUsername(registerModel.Username);
        if (user != null)
        {
            return BadRequest();
        }

        var passwordSalt = GeneratePasswordSaltHash();
        var passwordHash = HashPassword(registerModel.Password, passwordSalt);
        user = await authService.RegisterUser(registerModel.Username, passwordHash, Convert.ToHexString(passwordSalt));

        var token = GenerateJwtToken(registerModel.Username);
        return Ok(new LoginResponseModel { Token = token });
    }

    [HttpPost("[action]")]
    public async Task<ActionResult<LoginResponseModel>> Login([FromBody] LoginModel loginModel)
    {
        var user = await authService.GetUserByUsername(loginModel.Username);
        if (user == null)
        {
            return Unauthorized();
        }

        var storedSalt = Convert.FromHexString(user.PasswordSalt);
        var hashedPassword = HashPassword(loginModel.Password, storedSalt);
        if (hashedPassword != user.PasswordHash)
        {
            return Unauthorized();
        }

        var token = GenerateJwtToken(loginModel.Username);
        return Ok(new LoginResponseModel { Token = token });
    }

    private string GenerateJwtToken(string username)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, username == "Admin" ? "Admin" : "User"),
        };
        var secret = configuration.GetValue<string>("Jwt:Secret");
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
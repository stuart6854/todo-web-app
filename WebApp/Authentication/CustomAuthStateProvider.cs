using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Task = System.Threading.Tasks.Task;

namespace WebApp.Authentication;

public class CustomAuthStateProvider(ProtectedLocalStorage localStorage) : AuthenticationStateProvider
{
    public Guid UserId { get; private set; }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = (await localStorage.GetAsync<string>("authToken")).Value;
        var identity = string.IsNullOrEmpty(token) ? new ClaimsIdentity() : GetClaimsIdentity(token);
        
        var nameIdClaim = identity.FindFirst(ClaimTypes.NameIdentifier);
        if (nameIdClaim != null)
        {
            var userIdString = nameIdClaim.Value;
            UserId = Guid.Parse(userIdString);
        }

        var user = new ClaimsPrincipal(identity);
        return new AuthenticationState(user);
    }

    public async Task MarkUserAsAuthenticated(string token)
    {
        await localStorage.SetAsync("authToken", token);
        var identity = GetClaimsIdentity(token);
        var user = new ClaimsPrincipal(identity);
        UserId = Guid.Parse((ReadOnlySpan<char>)user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    public async Task MarkUserAsLoggedOut()
    {
        await localStorage.DeleteAsync("authToken");
        var identity = new ClaimsIdentity();
        var user = new ClaimsPrincipal(identity);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    private ClaimsIdentity GetClaimsIdentity(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        var claims = jwtToken.Claims;
        return new ClaimsIdentity(claims, "jwt");
    }
}
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace BlazorApp2.Services
{
    public class AuthService : AuthenticationStateProvider
    {
        private readonly ProtectedSessionStorage _sessionStorage;
        private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

        public AuthService(ProtectedSessionStorage sessionStorage)
        {
            _sessionStorage = sessionStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var userSessionStorageResult = await _sessionStorage.GetAsync<UserSession>("UserSession");
                var userSession = userSessionStorageResult.Success ? userSessionStorageResult.Value : null;
                if (userSession == null)
                    return await Task.FromResult(new AuthenticationState(_anonymous));
                var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userSession.UserName),
                    new Claim(ClaimTypes.Role, userSession.Role)
                }, "CustomAuth"));
                return await Task.FromResult(new AuthenticationState(claimsPrincipal));
            }
            catch
            {
                return await Task.FromResult(new AuthenticationState(_anonymous));
            }
        }

        public async Task<bool> LoginAsync(string userName, string password)
        {
            try
            {
                // Hardcoded doctor credentials for demo
                var doctors = new Dictionary<string, string>
                {
                    { "dr.smith", "password123" },
                    { "dr.johnson", "password123" },
                    { "dr.williams", "password123" },
                    { "admin", "admin123" }
                };

                // Log login attempt (remove in production)
                Console.WriteLine($"Login attempt - Username: {userName}, Password length: {password?.Length ?? 0}");

                if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
                {
                    Console.WriteLine("Login failed: Empty username or password");
                    return false;
                }

                var normalizedUsername = userName.ToLower().Trim();
                if (doctors.ContainsKey(normalizedUsername) && doctors[normalizedUsername] == password)
                {
                    var userSession = new UserSession
                    {
                        UserName = userName,
                        Role = "Doctor"
                    };
                    
                    await _sessionStorage.SetAsync("UserSession", userSession);
                    NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
                    
                    Console.WriteLine($"Login successful for: {userName}");
                    return true;
                }
                
                Console.WriteLine($"Login failed: Invalid credentials for {userName}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login error: {ex.Message}");
                return false;
            }
        }

        public async Task LogoutAsync()
        {
            await _sessionStorage.DeleteAsync("UserSession");
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public class UserSession
        {
            public string UserName { get; set; } = "";
            public string Role { get; set; } = "";
        }
    }
}

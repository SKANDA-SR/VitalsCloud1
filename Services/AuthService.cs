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
            // Hardcoded doctor credentials for demo
            var doctors = new Dictionary<string, string>
            {
                { "dr.smith", "password123" },
                { "dr.johnson", "password123" },
                { "dr.williams", "password123" },
                { "admin", "admin123" }
            };

            if (doctors.ContainsKey(userName.ToLower()) && doctors[userName.ToLower()] == password)
            {
                var userSession = new UserSession
                {
                    UserName = userName,
                    Role = "Doctor"
                };
                await _sessionStorage.SetAsync("UserSession", userSession);
                NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
                return true;
            }
            return false;
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

using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Blazored.LocalStorage;

namespace BeautyCenterFrontend.Services
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        private readonly HttpClient _httpClient;

        public CustomAuthStateProvider(ILocalStorageService localStorage, HttpClient httpClient)
        {
            _localStorage = localStorage;
            _httpClient = httpClient;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var token = await _localStorage.GetItemAsync<string>("authToken");
                Console.WriteLine($"CustomAuthStateProvider: Token retrieved: {(string.IsNullOrEmpty(token) ? "null/empty" : "exists")}");
                
                if (string.IsNullOrEmpty(token))
                {
                    Console.WriteLine("CustomAuthStateProvider: No token found, returning unauthenticated state");
                    return CreateUnauthenticatedState();
                }

                var handler = new JwtSecurityTokenHandler();
                
                if (!handler.CanReadToken(token))
                {
                    Console.WriteLine("CustomAuthStateProvider: Invalid token format");
                    await ClearAuthenticationAsync();
                    return CreateUnauthenticatedState();
                }

                var jwt = handler.ReadJwtToken(token);
                Console.WriteLine($"CustomAuthStateProvider: Token expires at: {jwt.ValidTo}, Current time: {DateTime.UtcNow}");

                if (jwt.ValidTo < DateTime.UtcNow)
                {
                    Console.WriteLine("CustomAuthStateProvider: Token expired, clearing storage");
                    await ClearAuthenticationAsync();
                    return CreateUnauthenticatedState();
                }

                var claims = jwt.Claims.ToList();
                var identity = new ClaimsIdentity(claims, "Bearer", ClaimTypes.Name, ClaimTypes.Role);
                var principal = new ClaimsPrincipal(identity);
                
                Console.WriteLine($"CustomAuthStateProvider: User authenticated: {principal.Identity.IsAuthenticated}");
                Console.WriteLine($"CustomAuthStateProvider: User name: {principal.Identity.Name}");
                Console.WriteLine($"CustomAuthStateProvider: Claims count: {claims.Count}");

                return new AuthenticationState(principal);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"CustomAuthStateProvider: LocalStorage not available: {ex.Message}");
                // LocalStorage not available during prerendering
                return CreateUnauthenticatedState();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CustomAuthStateProvider: Error: {ex.Message}");
                await ClearAuthenticationAsync();
                return CreateUnauthenticatedState();
            }
        }

        private AuthenticationState CreateUnauthenticatedState()
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        private async Task ClearAuthenticationAsync()
        {
            try
            {
                await _localStorage.RemoveItemAsync("authToken");
                await _localStorage.RemoveItemAsync("currentUser");
            }
            catch (InvalidOperationException)
            {
                // LocalStorage not available during prerendering
            }
        }

        public void NotifyAuthenticationStateChanged()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public async Task<bool> IsUserAuthenticated()
        {
            var authState = await GetAuthenticationStateAsync();
            return authState.User.Identity?.IsAuthenticated ?? false;
        }
    }
}
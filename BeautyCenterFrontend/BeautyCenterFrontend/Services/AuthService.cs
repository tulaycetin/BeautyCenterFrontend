using BeautyCenterFrontend.Models;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BeautyCenterFrontend.Services
{
    public class AuthService
    {
        private readonly ApiService _apiService;
        private readonly ILocalStorageService _localStorage;
        private readonly AuthenticationStateProvider _authStateProvider;

        public AuthService(ApiService apiService, ILocalStorageService localStorage, AuthenticationStateProvider authStateProvider)
        {
            _apiService = apiService;
            _localStorage = localStorage;
            _authStateProvider = authStateProvider;
        }

        public async Task<bool> LoginAsync(LoginRequest request)
        {
            try
            {
                // Trim username and password to remove any extra spaces
                request.Username = request.Username?.Trim();
                request.Password = request.Password?.Trim();
                
                Console.WriteLine($"Attempting login with username: {request.Username}");
                var response = await _apiService.PostAsync<LoginResponse>("api/auth/login", request);
                
                if (response != null)
                {
                    Console.WriteLine($"Login response received. Token exists: {!string.IsNullOrEmpty(response.Token)}");
                    if (!string.IsNullOrEmpty(response.Token))
                    {
                        try
                        {
                            await _localStorage.SetItemAsync("authToken", response.Token);
                            await _localStorage.SetItemAsync("currentUser", response.User);
                            Console.WriteLine("Token and user saved to localStorage successfully");
                            
                            // Force update authentication state
                            if (_authStateProvider is CustomAuthStateProvider customProvider)
                            {
                                customProvider.NotifyAuthenticationStateChanged();
                                Console.WriteLine("Authentication state change notified");
                            }
                            
                            return true;
                        }
                        catch (InvalidOperationException ex)
                        {
                            Console.WriteLine($"LocalStorage error: {ex.Message}");
                            // Continue anyway as the token might still be valid
                            return true;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Login response is null");
                }
                
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login Error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> RegisterAsync(RegisterRequest request)
        {
            try
            {
                var response = await _apiService.PostAsync<object>("api/auth/register", request);
                return response != null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Register Error: {ex.Message}");
                return false;
            }
        }

        public async Task LogoutAsync()
        {
            try
            {
                await _localStorage.RemoveItemAsync("authToken");
                await _localStorage.RemoveItemAsync("currentUser");
                Console.WriteLine("Logout: Tokens removed from localStorage");
                
                // Force update authentication state
                if (_authStateProvider is CustomAuthStateProvider customProvider)
                {
                    customProvider.NotifyAuthenticationStateChanged();
                    Console.WriteLine("Logout: Authentication state change notified");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Logout Error: {ex.Message}");
            }
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            return !string.IsNullOrEmpty(token) && !IsTokenExpired(token);
        }

        public async Task<UserInfo?> GetCurrentUserAsync()
        {
            return await _localStorage.GetItemAsync<UserInfo>("currentUser");
        }

        private bool IsTokenExpired(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);
                return jwt.ValidTo < DateTime.UtcNow;
            }
            catch
            {
                return true;
            }
        }
    }
}
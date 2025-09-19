using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using BeautyCenterFrontend.Models;
using Blazored.LocalStorage;

namespace BeautyCenterFrontend.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly JsonSerializerOptions _jsonOptions;

        public ApiService(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
            
            // Ensure BaseAddress is set
            if (_httpClient.BaseAddress == null)
            {
                _httpClient.BaseAddress = new Uri("http://localhost:5001/");
            }
            
            Console.WriteLine($"ApiService initialized with BaseAddress: {_httpClient.BaseAddress}");
            
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }

        private async Task SetAuthorizationHeaderAsync()
        {
            try
            {
                var token = await _localStorage.GetItemAsync<string>("authToken");
                Console.WriteLine($"Retrieved token from localStorage: {(string.IsNullOrEmpty(token) ? "null/empty" : "exists")}");
                
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    Console.WriteLine("Authorization header set with token");
                }
                else
                {
                    // Clear any existing authorization header
                    _httpClient.DefaultRequestHeaders.Authorization = null;
                    Console.WriteLine("No token found, authorization header cleared");
                }
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"LocalStorage not available: {ex.Message}");
                // LocalStorage is not available during static rendering
                // This is expected and can be ignored for login requests
            }
        }

        public async Task<T?> GetAsync<T>(string endpoint)
        {
            try
            {
                await SetAuthorizationHeaderAsync();
                var response = await _httpClient.GetAsync(endpoint);
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<T>(content, _jsonOptions);
                }
                
                return default(T);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GET Error: {ex.Message}");
                return default(T);
            }
        }

        public async Task<T?> PostAsync<T>(string endpoint, object data)
        {
            try
            {
                // Don't set auth header for login endpoint
                if (!endpoint.Contains("auth/login"))
                {
                    await SetAuthorizationHeaderAsync();
                }
                
                var json = JsonSerializer.Serialize(data, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var fullUrl = new Uri(_httpClient.BaseAddress, endpoint).ToString();
                Console.WriteLine($"POST to {fullUrl} with data: {json}");
                var response = await _httpClient.PostAsync(endpoint, content);
                Console.WriteLine($"Response status: {response.StatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Response content: {responseContent}");
                    return JsonSerializer.Deserialize<T>(responseContent, _jsonOptions);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error response: {response.StatusCode} - {errorContent}");
                }
                
                return default(T);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"POST Error: {ex.Message}");
                return default(T);
            }
        }

        public async Task<bool> PutAsync(string endpoint, object data)
        {
            try
            {
                await SetAuthorizationHeaderAsync();
                var json = JsonSerializer.Serialize(data, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PutAsync(endpoint, content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"PUT Error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(string endpoint)
        {
            try
            {
                await SetAuthorizationHeaderAsync();
                var response = await _httpClient.DeleteAsync(endpoint);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DELETE Error: {ex.Message}");
                return false;
            }
        }

        public async Task<ApiResult<T>> PostWithErrorAsync<T>(string endpoint, object data)
        {
            try
            {
                // Don't set auth header for login endpoint
                if (!endpoint.Contains("auth/login"))
                {
                    await SetAuthorizationHeaderAsync();
                }
                
                var json = JsonSerializer.Serialize(data, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync(endpoint, content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<T>(responseContent, _jsonOptions);
                    return ApiResult<T>.SuccessResult(result);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    
                    // Try to parse error message from JSON response
                    string errorMessage = "An error occurred";
                    try
                    {
                        var errorJson = JsonSerializer.Deserialize<JsonElement>(errorContent);
                        if (errorJson.TryGetProperty("message", out var messageElement))
                        {
                            errorMessage = messageElement.GetString() ?? errorMessage;
                        }
                    }
                    catch
                    {
                        // If parsing fails, use the raw content
                        errorMessage = errorContent;
                    }
                    
                    return ApiResult<T>.ErrorResult(errorMessage, (int)response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"POST Error: {ex.Message}");
                return ApiResult<T>.ErrorResult(ex.Message, 500);
            }
        }
    }
}
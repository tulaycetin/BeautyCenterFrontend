using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;
using BeautyCenterFrontend.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Add HttpClient for API calls
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7185/") });

// Add LocalStorage
builder.Services.AddBlazoredLocalStorage();

// Add Authorization
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<CustomAuthStateProvider>());

// Add Services
builder.Services.AddScoped<ApiService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<AppointmentService>();
builder.Services.AddScoped<ServiceTypeService>();
builder.Services.AddScoped<PaymentService>();

await builder.Build().RunAsync();

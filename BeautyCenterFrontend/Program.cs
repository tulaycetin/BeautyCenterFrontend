using BeautyCenterFrontend.Client.Pages;
using BeautyCenterFrontend.Components;
using BeautyCenterFrontend.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;

var builder = WebApplication.CreateBuilder(args);

// Add console logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add HttpClient for API calls
builder.Services.AddHttpClient<ApiService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7185/"); // API base address
});

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Browser Link is disabled to prevent JavaScript errors
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

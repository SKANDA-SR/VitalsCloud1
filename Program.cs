// Add these 'using' statements for the database context
using Microsoft.EntityFrameworkCore;
using BlazorApp2.Data; 
using BlazorApp2.Models;

// Your existing 'using' statements
using BlazorApp2.Components;
using BlazorApp2.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add authentication services
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options => {
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/";
    });
builder.Services.AddScoped<ProtectedSessionStorage>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<AuthenticationStateProvider>(s => s.GetRequiredService<AuthService>());
builder.Services.AddAuthorizationCore();

// ---> START: ADD THE DATABASE CODE HERE
// 1. Get the connection string from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. Register the DbContext for Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
// ---> END: DATABASE CODE

// ---> REPLACE the line below
// builder.Services.AddSingleton<ClinicService>();
// WITH THIS ONE:
builder.Services.AddScoped<ClinicService>(); // Use 'Scoped' for services that use the database


var app = builder.Build();

// Test database operations (remove this in production)
// await DatabaseTest.TestDatabaseAsync();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

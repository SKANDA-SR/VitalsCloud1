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

// Initialize database on startup
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        // Ensure database is created
        await context.Database.EnsureCreatedAsync();
        
        // Add some sample data if database is empty
        if (!context.Patients.Any())
        {
            var samplePatients = new[]
            {
                new Patient { Name = "John Smith", Contact = "+1-555-0101", DateOfBirth = new DateTime(1980, 5, 15) },
                new Patient { Name = "Mary Johnson", Contact = "+1-555-0102", DateOfBirth = new DateTime(1975, 8, 22) },
                new Patient { Name = "Robert Davis", Contact = "+1-555-0103", DateOfBirth = new DateTime(1992, 12, 3) }
            };
            
            context.Patients.AddRange(samplePatients);
            await context.SaveChangesAsync();
        }
    }
    catch (Exception ex)
    {
        // Log the error but don't stop the application
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error initializing database");
    }
}

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
using InventoryManagementSystem2.DATA;
using InventoryManagementSystem2.Models;
using InventoryManagementSystem2.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Register the database connection
builder.Services.AddDbContext<InventoryManagementContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Add session support
builder.Services.AddDistributedMemoryCache(); // To store session data in memory
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add controllers with views
builder.Services.AddControllersWithViews();

// Register authentication services (cookie authentication)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Home/Index"; // The login page route
        options.LogoutPath = "/Authenticator/Logout"; // The logout path
        options.AccessDeniedPath = "/Home/AccessDenied"; // Access denied path
    });

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

var dbConnectionTester = new DBConnectionTester(connectionString);
dbConnectionTester.TestConnection();

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();


// Enable session
app.UseSession();

// Enable authentication
app.UseAuthentication();
app.UseAuthorization();

// Define routing for the controllers
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Login}/{id?}");

app.Run();
app.UseStaticFiles();

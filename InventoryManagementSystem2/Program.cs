using Microsoft.EntityFrameworkCore;
using InventoryManagementSystem2.DATA;
using Microsoft.Extensions.DependencyInjection;
using InventoryManagementSystem2.Services;
var builder = WebApplication.CreateBuilder(args);

// Register the database connection
builder.Services.AddDbContext<InventoryManagementContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Add controllers with views
builder.Services.AddControllersWithViews();


// Instantiate the DBConnectionTester and test the connection
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
app.UseAuthorization();

// Define routing for the controllers
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

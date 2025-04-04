using InventoryManagementSystem2.Models;
using Microsoft.EntityFrameworkCore;
using InventoryManagementSystem2.Models; // Replace with your actual namespace

namespace InventoryManagementSystem2.DATA
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Add DbSets for your existing tables
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductIngredient> ProductIngredient { get; set; }
        public DbSet<Orders> Orders { get; set; }
        public DbSet<Customers> Customers { get; set; }
        public DbSet<Bundle> Bundle { get; set; }
        public DbSet<OrderList> OrderList { get; set; }
        public DbSet<Replenishment> Replenishment { get; set; }
        public DbSet<Ingredient> Ingredient { get; set; }
        public DbSet<Users> Users { get; set; }
    }

}
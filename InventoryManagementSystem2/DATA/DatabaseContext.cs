using InventoryManagementSystem2.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem2.DATA
{
    public class InventoryManagementContext : DbContext
    {
        public DbSet<Bundle> Bundles { get; set; }
        public DbSet<Customers> Customers { get; set; }
        public DbSet<Ingredient> Ingredient { get; set; }
        public DbSet<OrderList> OrderLists { get; set; }
        public DbSet<Orders> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductIngredient> ProductIngredients { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<Replenishment> Replenishments { get; set; }
        public DbSet<Category> Category { get; set; }

        // Constructor to accept options
        public InventoryManagementContext(DbContextOptions<InventoryManagementContext> options) : base(options)
        {
        }

        // Override OnModelCreating to configure relationships and other settings
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure composite key for ProductIngredient
            modelBuilder.Entity<ProductIngredient>()
                .HasKey(pi => new { pi.IngredientID, pi.ProductID });

            modelBuilder.Entity<ProductIngredient>()
                .HasOne(pi => pi.Ingredient)
                .WithMany()
                .HasForeignKey(pi => pi.IngredientID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProductIngredient>()
                .HasOne(pi => pi.Product)
                .WithMany()
                .HasForeignKey(pi => pi.ProductID)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure decimal precision for relevant fields
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<OrderList>()
                .Property(ol => ol.UnitPrice)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<OrderList>()
                .Property(ol => ol.SubTotal)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<Bundle>()
                .Property(b => b.Price)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<ProductIngredient>()
                .Property(pi => pi.Quantity)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<Replenishment>()
                .Property(r => r.Cost)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<Replenishment>()
                .Property(r => r.Quantity)
                .HasColumnType("decimal(10,2)");

            // Configure relationships with delete behavior
            modelBuilder.Entity<Orders>()
                .HasOne(o => o.Customer)
                .WithMany()
                .HasForeignKey(o => o.CustomerID)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Orders>()
                .HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.UserID)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<OrderList>()
                .HasOne(ol => ol.Product)
                .WithMany()
                .HasForeignKey(ol => ol.ProductID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<OrderList>()
                .HasOne(ol => ol.Bundle)
                .WithMany()
                .HasForeignKey(ol => ol.BundleID)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}

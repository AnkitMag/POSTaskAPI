using Microsoft.EntityFrameworkCore;
using POSTaskAPI.Models;

namespace POSTaskAPI.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Order Configuration
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");

                // Storing Enum as a string in SQL Server (e.g., "Delivery" instead of 0)
                entity.Property(e => e.Type)
                      .HasConversion<string>()
                      .HasMaxLength(20);
            });

            // 2. OrderItem Configuration (One-to-Many with Order)
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");

                entity.HasOne(d => d.Order)
                      .WithMany(p => p.OrderItems)
                      .HasForeignKey(d => d.OrderId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Product)
                      .WithMany()
                      .HasForeignKey(d => d.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // 3. Product & ProductType Configuration
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");

                entity.HasOne(d => d.Category)
                      .WithMany(p => p.Items)
                      .HasForeignKey(d => d.CategoryId);
            });

            // 4. User Configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            });
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Order.API.Models;

namespace Order.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Models.Order> Orders => Set<Models.Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<ShippingAddress> ShippingAddresses => Set<ShippingAddress>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Models.Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");

            entity.HasMany(e => e.Items)
                  .WithOne()
                  .HasForeignKey(e => e.OrderId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.ShippingAddress)
                  .WithOne()
                  .HasForeignKey<ShippingAddress>(e => e.OrderId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ProductId).IsRequired();
            entity.Property(e => e.ProductName).IsRequired();
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Quantity).IsRequired();
        });

        modelBuilder.Entity<ShippingAddress>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Street).IsRequired();
            entity.Property(e => e.City).IsRequired();
            entity.Property(e => e.State).IsRequired();
            entity.Property(e => e.ZipCode).IsRequired();
            entity.Property(e => e.PhoneNumber).IsRequired();
        });
    }
}

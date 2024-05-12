using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;

namespace AzureCosmosDbNoSql;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultContainer("orders");

        builder.Entity<Order>()
         .HasPartitionKey(c => c.UserId);
    }
}

public class Order
{
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public double Total { get; set; }
    public string Currency { get; set; }
    public string Address { get; set; }
}
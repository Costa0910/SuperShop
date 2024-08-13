using System.Linq;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data.Entities;

namespace WebApplication.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DbSet<Product> Products { set; get; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<OrderDetailsTemp> OrderDetailsTemps { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<City> Cities { get; set; }
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        // protected override void OnModelCreating(ModelBuilder builder)
        // {
        //     var fks = builder.Model
        //                      .GetEntityTypes()
        //                      .SelectMany(t => t.GetForeignKeys())
        //                      .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);
        //
        //     foreach (var fk in fks)
        //     {
        //         fk.DeleteBehavior = DeleteBehavior.Restrict;
        //     }
        //
        //     base.OnModelCreating(builder);
        // }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Country>().HasIndex(c => c.Name).IsUnique();
            builder.Entity<Product>().Property(p => p.Price).HasColumnType("decimal(18,2)");
            builder.Entity<OrderDetails>().Property(p => p.Price).HasColumnType("decimal(18,2)");
            builder.Entity<OrderDetailsTemp>().Property(p => p.Price).HasColumnType("decimal(18,2)");
            base.OnModelCreating(builder);
        }
    }
}
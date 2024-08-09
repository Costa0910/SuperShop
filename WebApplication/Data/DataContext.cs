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
    }
}
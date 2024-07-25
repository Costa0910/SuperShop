using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data.Entities;

namespace WebApplication.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DbSet<Product> Products { set; get; }
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
    }
}
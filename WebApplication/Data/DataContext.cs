﻿using Microsoft.EntityFrameworkCore;
using WebApplication.Data.Entities;

namespace WebApplication.Data
{
    public class DataContext : DbContext
    {
        DbSet<Product> Products { set; get; }
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            
        }
    }
}

﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ActiveX_Api.Models
{
    public class AppDbContext : IdentityDbContext<ApiUser> 
    {
        public AppDbContext (DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>()
                .HasKey(e => e.Id);
            modelBuilder.Entity<Product>()
                .HasMany(e => e.Reviews)
                .WithOne(e => e.Product)
                .HasForeignKey(e => e.ProductId);
            modelBuilder.Entity<Product>()
                .HasOne(e => e.Category);

            modelBuilder.Entity<Review>()
                .HasOne(e => e.Product)
                .WithMany(e => e.Reviews)
                .HasForeignKey (e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Review> Reviews { get; set; }
    }
}

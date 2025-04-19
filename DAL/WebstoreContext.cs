using Microsoft.EntityFrameworkCore;
using Model;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class WebstoreContext:DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductGroup> ProductGroups { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<BasketPosition> BasketPositions { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderPosition> OrderPositions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<BasketPosition>().HasKey(table => new {
                table.ProductID,
                table.UserID
            });
            builder.Entity<OrderPosition>().HasKey(table => new {
                table.ProductID,
                table.OrderID
            });

            builder.Entity<Product>()
                .HasOne(p=>p.ProductGroup)
                .WithMany(pg=>pg.Products)
                .HasForeignKey(p=>p.GroupID)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<ProductGroup>()
                .HasOne(pg => pg.ParentGroup)
                .WithMany(pg => pg.ChildColletion)
                .HasForeignKey(pg => pg.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<User>()
                .HasOne(u => u.UserGroup)
                .WithMany(ug => ug.Users)
                .HasForeignKey(u => u.GroupId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<User>()
                 .HasMany(u => u.Orders)
                 .WithOne(o => o.User)
                 .HasForeignKey(o => o.UserID)
                 .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<User>()
                 .HasMany(u => u.BasketPositions)
                 .WithOne(bp => bp.User)
                 .HasForeignKey(bp => bp.UserID)
                 .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<OrderPosition>()
                 .HasOne(op => op.Order)
                 .WithMany(p => p.OrderPositions)
                 .HasForeignKey(op => op.OrderID)
                 .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<OrderPosition>()
                 .HasOne(op => op.Product)
                 .WithMany(p => p.OrderPositions)
                 .HasForeignKey(op => op.ProductID)
                 .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<BasketPosition>()
                 .HasOne(bp => bp.Product)
                 .WithMany(p => p.BasketPositions)
                 .HasForeignKey(bp => bp.ProductID)
                 .OnDelete(DeleteBehavior.Cascade);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=SklepMongo;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
        }
    }
}

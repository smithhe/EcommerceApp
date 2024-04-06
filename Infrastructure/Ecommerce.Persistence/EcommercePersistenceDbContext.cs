using System;
using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Persistence
{
    public class EcommercePersistenceDbContext(DbContextOptions<EcommercePersistenceDbContext> options)
        : IdentityDbContext<EcommerceUser, IdentityRole<Guid>, Guid>(options)
    {
        public DbSet<CartItem> CartItems { get; init; } = null!;
        public DbSet<Category> Categories { get; init; } = null!;
        public DbSet<Order> Orders { get; init; } = null!;
        public DbSet<OrderItem> OrderItems { get; init; } = null!;
        public DbSet<OrderKey> OrderKeys { get; init; } = null!;
        public DbSet<Product> Products { get; init; } = null!;
        public DbSet<Review> Reviews { get; init; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //-----------------------------------------------------------------------------------------------------------
            // Cart Item
            //-----------------------------------------------------------------------------------------------------------
            modelBuilder.Entity<CartItem>()
                .ToTable(t =>
                    {
                        t.HasCheckConstraint(
                            "CHK_CILastModifiedDate",
                            "LastModifiedDate IS NULL OR CreatedDate <= LastModifiedDate"
                        );

                        t.HasCheckConstraint(
                            "CHK_CICreatedDate",
                            "CreatedDate <= LastModifiedDate"
                        );

                    }
                )
                .HasOne<EcommerceUser>()
                .WithMany()
                .HasForeignKey(ci => ci.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<CartItem>()
                .HasOne<Product>()
                .WithMany()
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
            
            //-----------------------------------------------------------------------------------------------------------
            // Category
            //-----------------------------------------------------------------------------------------------------------
            modelBuilder.Entity<Category>()
                .ToTable(t =>
                    {
                        t.HasCheckConstraint(
                            "CHK_CLastModifiedDate",
                            "LastModifiedDate IS NULL OR CreatedDate <= LastModifiedDate"
                        );

                        t.HasCheckConstraint(
                            "CHK_CCreatedDate",
                            "CreatedDate <= LastModifiedDate"
                        );
                    }
                );
            
            //-----------------------------------------------------------------------------------------------------------
            // Order
            //-----------------------------------------------------------------------------------------------------------
            modelBuilder.Entity<Order>()
                .ToTable(t =>
                    {
                        t.HasCheckConstraint(
                            "CHK_OLastModifiedDate",
                            "LastModifiedDate IS NULL OR CreatedDate <= LastModifiedDate"
                        );

                        t.HasCheckConstraint(
                            "CHK_OCreatedDate",
                            "CreatedDate <= LastModifiedDate"
                        );
                    }
                )
                .HasOne<EcommerceUser>()
                .WithMany()
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            //-----------------------------------------------------------------------------------------------------------
            // Order Item
            //-----------------------------------------------------------------------------------------------------------
            modelBuilder.Entity<OrderItem>()
                .ToTable(t =>
                    {
                        t.HasCheckConstraint(
                            "CHK_OILastModifiedDate",
                            "LastModifiedDate IS NULL OR CreatedDate <= LastModifiedDate"
                        );

                        t.HasCheckConstraint(
                            "CHK_OICreatedDate",
                            "CreatedDate <= LastModifiedDate"
                        );
                    }
                )
                .HasOne<Order>()
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
            
            //-----------------------------------------------------------------------------------------------------------
            // Product
            //-----------------------------------------------------------------------------------------------------------
            modelBuilder.Entity<Product>()
                .ToTable(t =>
                    {
                        t.HasCheckConstraint(
                            "CHK_PLastModifiedDate",
                            "LastModifiedDate IS NULL OR CreatedDate <= LastModifiedDate"
                        );

                        t.HasCheckConstraint(
                            "CHK_PCreatedDate",
                            "CreatedDate <= LastModifiedDate"
                        );
                    }
                );
            
            //-----------------------------------------------------------------------------------------------------------
            // Review
            //-----------------------------------------------------------------------------------------------------------
            modelBuilder.Entity<Review>()
                .ToTable(t =>
                    {
                        t.HasCheckConstraint(
                            "CHK_RLastModifiedDate",
                            "LastModifiedDate IS NULL OR CreatedDate <= LastModifiedDate"
                        );

                        t.HasCheckConstraint(
                            "CHK_RCreatedDate",
                            "CreatedDate <= LastModifiedDate"
                        );
                    }
                )
                .HasOne<EcommerceUser>()
                .WithMany()
                .HasForeignKey(r => r.UserName)
                .HasPrincipalKey(u => u.UserName)
                .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<Review>()
                .HasOne<Product>()
                .WithMany(p => p.CustomerReviews)
                .HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
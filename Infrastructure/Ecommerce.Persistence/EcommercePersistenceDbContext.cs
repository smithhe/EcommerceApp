using System;
using System.Linq;
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
        public virtual DbSet<CartItem> CartItems { get; init; } = null!;
        public virtual DbSet<Category> Categories { get; init; } = null!;
        public virtual DbSet<Order> Orders { get; init; } = null!;
        public virtual DbSet<OrderItem> OrderItems { get; init; } = null!;
        public virtual DbSet<OrderKey> OrderKeys { get; init; } = null!;
        public virtual DbSet<Product> Products { get; init; } = null!;
        public virtual DbSet<Review> Reviews { get; init; } = null!;

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
                )
                .HasIndex(c => c.Name);

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
                )
                .HasIndex(p => p.Name);

            modelBuilder.Entity<Product>()
                .HasOne<Category>()
                .WithMany()
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

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

            //Finish by seeding the data
            this.SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            //-----------------------------------------------------------------------------------------------------------
            // Categories
            //-----------------------------------------------------------------------------------------------------------
            Category laptops = new Category
            {
                Id = 1, Name = "Laptops", Summary = "Explore our range of laptops.", CreatedBy = "Harold",
                CreatedDate = DateTime.Now
            };
            Category phones = new Category
            {
                Id = 2, Name = "Phones", Summary = "Discover the latest smartphones.", CreatedBy = "Harold",
                CreatedDate = DateTime.Now
            };
            Category tablets = new Category
            {
                Id = 3, Name = "Tablets", Summary = "Browse our collection of tablets.", CreatedBy = "Harold",
                CreatedDate = DateTime.Now
            };

            modelBuilder.Entity<Category>().HasData(
                laptops,
                phones,
                tablets
            );

            //-----------------------------------------------------------------------------------------------------------
            // Products
            //-----------------------------------------------------------------------------------------------------------
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1, Name = "Laptop 1", 
                    Description = "This is a killer laptop that can handle all your home needs", Price = 299.99, 
                    CategoryId = laptops.Id, CreatedBy = "Harold", AverageRating = 0, QuantityAvailable = 5,
                    ImageUrl = "https://smith-ecommerce-app.s3.amazonaws.com/laptop1.jpg", CreatedDate = DateTime.Now
                },
                new Product
                {
                    Id = 2, Name = "Laptop 2",
                    Description = "This is a killer laptop that can handle all your home needs", Price = 499.99,
                    CategoryId = laptops.Id, CreatedBy = "Harold", AverageRating = 0, QuantityAvailable = 5,
                    ImageUrl = "https://smith-ecommerce-app.s3.amazonaws.com/laptop2.jpg", CreatedDate = DateTime.Now
                },
                new Product
                {
                    Id = 3, Name = "Laptop 3",
                    Description = "This is a killer laptop that can handle all your home needs", Price = 999.99,
                    CategoryId = laptops.Id, CreatedBy = "Harold", AverageRating = 0, QuantityAvailable = 5,
                    ImageUrl = "https://smith-ecommerce-app.s3.amazonaws.com/laptop3.jpg", CreatedDate = DateTime.Now
                }
            );
        }

        public void InitializeDatabase()
        {
        }
    }
}
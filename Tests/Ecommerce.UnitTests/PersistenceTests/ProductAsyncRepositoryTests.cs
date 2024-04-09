using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Domain.Entities;
using Ecommerce.Persistence;
using Ecommerce.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Moq;
// ReSharper disable PossibleMultipleEnumeration

namespace Ecommerce.UnitTests.PersistenceTests
{
    public class ProductAsyncRepositoryTests
    {
        
        private readonly Product _productOne = new Product
        {
            Id = 1,
            CategoryId = 1,
            Name = "Product One",
            Description = "Product One Description",
            Price = 100.00,
            AverageRating = 4.5m,
            QuantityAvailable = 10,
            ImageUrl = "https://www.example.com/product-one.jpg",
            CreatedBy = "Test User",
            CreatedDate = DateTime.MinValue
        };
        
        private readonly Product _productTwo = new Product
        {
            Id = 2,
            CategoryId = 1,
            Name = "Product Two",
            Description = "Product Two Description",
            Price = 200.00,
            AverageRating = 4.0m,
            QuantityAvailable = 20,
            ImageUrl = "https://www.example.com/product-two.jpg",
            CreatedBy = "Test User",
            CreatedDate = DateTime.MinValue
        };
        
        private readonly Product _productThree = new Product
        {
            Id = 3,
            CategoryId = 2,
            Name = "Product Three",
            Description = "Product Three Description",
            Price = 300.00,
            AverageRating = 3.5m,
            QuantityAvailable = 30,
            ImageUrl = "https://www.example.com/product-three.jpg",
            CreatedBy = "Test User",
            CreatedDate = DateTime.MinValue
        };
        
        private EcommercePersistenceDbContext _dbContext = null!;

        [SetUp]
        public void Setup()
        {
            DbContextOptions<EcommercePersistenceDbContext> options =
                new DbContextOptionsBuilder<EcommercePersistenceDbContext>()
                    .UseInMemoryDatabase(databaseName: "Ecommerce")
                    .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                    .Options;

            this._dbContext = new EcommercePersistenceDbContext(options);
            
            //Seed the database
            this._dbContext.Products.AddRange(
                this._productOne,
                this._productTwo
            );
            this._dbContext.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
            this._dbContext.Database.EnsureDeleted();
            this._dbContext.Dispose();
        }

        #region GetByIdAsync Tests

        [Test]
        public async Task GetByIdAsync_WithIdOfExistingProduct_ReturnsProduct()
        {
            // Arrange
            Product expectedProduct = this._productOne;
            ProductAsyncRepository repository = new ProductAsyncRepository(Mock.Of<ILogger<ProductAsyncRepository>>(), this._dbContext);

            // Act
            Product? result = await repository.GetByIdAsync(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.Id, Is.EqualTo(expectedProduct.Id));
                Assert.That(result.CategoryId, Is.EqualTo(expectedProduct.CategoryId));
                Assert.That(result.Name, Is.EqualTo(expectedProduct.Name));
                Assert.That(result.Description, Is.EqualTo(expectedProduct.Description));
                Assert.That(result.Price, Is.EqualTo(expectedProduct.Price));
                Assert.That(result.AverageRating, Is.EqualTo(expectedProduct.AverageRating));
                Assert.That(result.QuantityAvailable, Is.EqualTo(expectedProduct.QuantityAvailable));
                Assert.That(result.ImageUrl, Is.EqualTo(expectedProduct.ImageUrl));
                Assert.That(result.CreatedBy, Is.EqualTo(expectedProduct.CreatedBy));
                Assert.That(result.CreatedDate, Is.EqualTo(expectedProduct.CreatedDate));
            });
        }
        
        [Test]
        public async Task GetByIdAsync_WithIdOfNonExistingProduct_ReturnsNull()
        {
            // Arrange
            ProductAsyncRepository repository = new ProductAsyncRepository(Mock.Of<ILogger<ProductAsyncRepository>>(), this._dbContext);

            // Act
            Product? result = await repository.GetByIdAsync(3);

            // Assert
            Assert.That(result, Is.Null);
        }
        
        [Test]
        public async Task GetByIdAsync_WhenExceptionThrown_ReturnsNull()
        {
            // Arrange
            DbContextOptions<EcommercePersistenceDbContext> options =
                new DbContextOptionsBuilder<EcommercePersistenceDbContext>()
                    .UseInMemoryDatabase(databaseName: "Ecommerce")
                    .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                    .Options;
            
            Mock<EcommercePersistenceDbContext> mockDbContext = new Mock<EcommercePersistenceDbContext>(options);
            Mock<DbSet<CartItem>> mockSet = new Mock<DbSet<CartItem>>();

            mockDbContext.Setup(x => x.CartItems).Returns(mockSet.Object);
            
            
            ProductAsyncRepository repository = new ProductAsyncRepository(Mock.Of<ILogger<ProductAsyncRepository>>(), mockDbContext.Object);

            // Act
            Product? result = await repository.GetByIdAsync(1);

            // Assert
            Assert.That(result, Is.Null);
        }
        
        #endregion

        #region AddAsync Tests

        [Test]
        public async Task AddAsync_WithValidProduct_ReturnsIdOfNewProduct()
        {
            // Arrange
            ProductAsyncRepository repository = new ProductAsyncRepository(Mock.Of<ILogger<ProductAsyncRepository>>(), this._dbContext);

            // Act
            int result = await repository.AddAsync(this._productThree);

            // Assert
            Assert.That(result, Is.EqualTo(3));
        }
        
        [Test]
        public async Task AddAsync_WithExistingProduct_ReturnsMinusOne()
        {
            // Arrange
            ProductAsyncRepository repository = new ProductAsyncRepository(Mock.Of<ILogger<ProductAsyncRepository>>(), this._dbContext);

            // Act
            int result = await repository.AddAsync(this._productOne);

            // Assert
            Assert.That(result, Is.EqualTo(-1));
        }
        
        [Test]
        public async Task AddAsync_WhenExceptionThrown_ReturnsMinusOne()
        {
            // Arrange
            DbContextOptions<EcommercePersistenceDbContext> options =
                new DbContextOptionsBuilder<EcommercePersistenceDbContext>()
                    .UseInMemoryDatabase(databaseName: "Ecommerce")
                    .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                    .Options;

            Mock<EcommercePersistenceDbContext> mockDbContext = new Mock<EcommercePersistenceDbContext>(options);
            Mock<DbSet<Product>> mockSet = new Mock<DbSet<Product>>();
            Mock<DatabaseFacade> mockDatabase = new Mock<DatabaseFacade>(mockDbContext.Object);
            
            mockDatabase.Setup(d => d.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(Mock.Of<IDbContextTransaction>());
            mockSet.Setup(m => m.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Test exception"));

            mockDbContext.Setup(x => x.Products).Returns(mockSet.Object);
            mockDbContext.Setup(x => x.Database).Returns(mockDatabase.Object);
            
            ProductAsyncRepository repository = new ProductAsyncRepository(Mock.Of<ILogger<ProductAsyncRepository>>(), mockDbContext.Object);

            // Act
            int result = await repository.AddAsync(this._productThree);

            // Assert
            Assert.That(result, Is.EqualTo(-1));
        }

        #endregion

        #region UpdateAsync Tests

        [Test]
        public async Task UpdateAsync_WithExistingProduct_ReturnsTrue()
        {
            // Arrange
            ProductAsyncRepository repository = new ProductAsyncRepository(Mock.Of<ILogger<ProductAsyncRepository>>(), this._dbContext);
            this._productOne.Description = "Updated Description";

            // Act
            bool result = await repository.UpdateAsync(this._productOne);

            // Assert
            Assert.That(result, Is.True);
        }
        
        [Test]
        public async Task UpdateAsync_WithNonExistingProduct_ReturnsFalse()
        {
            // Arrange
            ProductAsyncRepository repository = new ProductAsyncRepository(Mock.Of<ILogger<ProductAsyncRepository>>(), this._dbContext);

            // Act
            bool result = await repository.UpdateAsync(this._productThree);

            // Assert
            Assert.That(result, Is.False);
        }
        
        [Test]
        public async Task UpdateAsync_WhenExceptionThrown_ReturnsFalse()
        {
            // Arrange
            DbContextOptions<EcommercePersistenceDbContext> options =
                new DbContextOptionsBuilder<EcommercePersistenceDbContext>()
                    .UseInMemoryDatabase(databaseName: "Ecommerce")
                    .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                    .Options;

            Mock<EcommercePersistenceDbContext> mockDbContext = new Mock<EcommercePersistenceDbContext>(options);
            Mock<DbSet<Product>> mockSet = new Mock<DbSet<Product>>();
            Mock<DatabaseFacade> mockDatabase = new Mock<DatabaseFacade>(mockDbContext.Object);
            
            mockDatabase.Setup(d => d.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(Mock.Of<IDbContextTransaction>());

            mockDbContext.Setup(x => x.Products).Returns(mockSet.Object);
            mockDbContext.Setup(x => x.Database).Returns(mockDatabase.Object);
            
            ProductAsyncRepository repository = new ProductAsyncRepository(Mock.Of<ILogger<ProductAsyncRepository>>(), mockDbContext.Object);

            // Act
            bool result = await repository.UpdateAsync(this._productOne);

            // Assert
            Assert.That(result, Is.False);
        }

        #endregion

        #region DeleteAsync Tests

        [Test]
        public async Task DeleteAsync_WithExistingProduct_ReturnsTrue()
        {
            // Arrange
            ProductAsyncRepository repository = new ProductAsyncRepository(Mock.Of<ILogger<ProductAsyncRepository>>(), this._dbContext);

            // Act
            bool result = await repository.DeleteAsync(this._productOne);

            // Assert
            Assert.That(result, Is.True);
        }
        
        [Test]
        public async Task DeleteAsync_WithNonExistingProduct_ReturnsFalse()
        {
            // Arrange
            ProductAsyncRepository repository = new ProductAsyncRepository(Mock.Of<ILogger<ProductAsyncRepository>>(), this._dbContext);

            // Act
            bool result = await repository.DeleteAsync(this._productThree);

            // Assert
            Assert.That(result, Is.False);
        }
        
        [Test]
        public async Task DeleteAsync_WhenExceptionThrown_ReturnsFalse()
        {
            // Arrange
            DbContextOptions<EcommercePersistenceDbContext> options =
                new DbContextOptionsBuilder<EcommercePersistenceDbContext>()
                    .UseInMemoryDatabase(databaseName: "Ecommerce")
                    .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                    .Options;

            Mock<EcommercePersistenceDbContext> mockDbContext = new Mock<EcommercePersistenceDbContext>(options);
            Mock<DbSet<Product>> mockSet = new Mock<DbSet<Product>>();
            Mock<DatabaseFacade> mockDatabase = new Mock<DatabaseFacade>(mockDbContext.Object);
            
            mockDatabase.Setup(d => d.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(Mock.Of<IDbContextTransaction>());
            mockSet.Setup(m => m.Remove(It.IsAny<Product>()))
                .Throws(new Exception("Test exception"));

            mockDbContext.Setup(x => x.Products).Returns(mockSet.Object);
            mockDbContext.Setup(x => x.Database).Returns(mockDatabase.Object);
            
            ProductAsyncRepository repository = new ProductAsyncRepository(Mock.Of<ILogger<ProductAsyncRepository>>(), mockDbContext.Object);

            // Act
            bool result = await repository.DeleteAsync(this._productOne);

            // Assert
            Assert.That(result, Is.False);
        }

        #endregion

        #region ListAllAsync Tests

        [Test]
        public async Task ListAllAsync_WithCategoryIdOfExistingProducts_ReturnsProducts()
        {
            // Arrange
            ProductAsyncRepository repository = new ProductAsyncRepository(Mock.Of<ILogger<ProductAsyncRepository>>(), this._dbContext);

            // Act
            IEnumerable<Product> result = await repository.ListAllAsync(1);

            // Assert
            Assert.That(result, Is.Not.Empty);
            Assert.That(result, Has.Exactly(2).Items);
        }
        
        [Test]
        public async Task ListAllAsync_WithCategoryIdOfNonExistingProducts_ReturnsEmptyList()
        {
            // Arrange
            ProductAsyncRepository repository = new ProductAsyncRepository(Mock.Of<ILogger<ProductAsyncRepository>>(), this._dbContext);

            // Act
            IEnumerable<Product> result = await repository.ListAllAsync(3);

            // Assert
            Assert.That(result, Is.Empty);
        }
        
        [Test]
        public async Task ListAllAsync_WhenExceptionThrown_ReturnsEmptyList()
        {
            // Arrange
            DbContextOptions<EcommercePersistenceDbContext> options =
                new DbContextOptionsBuilder<EcommercePersistenceDbContext>()
                    .UseInMemoryDatabase(databaseName: "Ecommerce")
                    .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                    .Options;

            Mock<EcommercePersistenceDbContext> mockDbContext = new Mock<EcommercePersistenceDbContext>(options);
            Mock<DbSet<Product>> mockSet = new Mock<DbSet<Product>>();
            
            mockDbContext.Setup(x => x.Products).Returns(mockSet.Object);
            
            ProductAsyncRepository repository = new ProductAsyncRepository(Mock.Of<ILogger<ProductAsyncRepository>>(), mockDbContext.Object);

            // Act
            IEnumerable<Product> result = await repository.ListAllAsync(1);

            // Assert
            Assert.That(result, Is.Empty);
        }

        #endregion
        
        
    }
}
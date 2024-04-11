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
    public class OrderItemAsyncRepositoryTests
    {
        private const string _userName = "Test User";
        
        private readonly OrderItem _orderItemOne = new OrderItem
        {
            OrderId = 1,
            ProductName = "Test Product",
            ProductDescription = "Test Description",
            ProductSku = "Test SKU",
            Quantity = 1,
            Price = 10.00,
            CreatedBy = _userName,
            CreatedDate = DateTime.MinValue
        };
        
        private readonly OrderItem _orderItemTwo = new OrderItem
        {
            OrderId = 2,
            ProductName = "Test Product 2",
            ProductDescription = "Test Description 2",
            ProductSku = "Test SKU 2",
            Quantity = 2,
            Price = 20.00,
            CreatedBy = _userName,
            CreatedDate = DateTime.MinValue
        };
        
        private readonly OrderItem _orderItemThree = new OrderItem
        {
            OrderId = 3,
            ProductName = "Test Product 3",
            ProductDescription = "Test Description 3",
            ProductSku = "Test SKU 3",
            Quantity = 3,
            Price = 30.00,
            CreatedBy = _userName,
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
            this._dbContext.OrderItems.AddRange(
                this._orderItemOne,
                this._orderItemTwo
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
        public async Task GetByIdAsync_WhenOrderItemExists_ReturnsOrderItem()
        {
            // Arrange
            OrderItem expectedOrderItem = this._orderItemOne;
            
            OrderItemAsyncRepository repository = new OrderItemAsyncRepository(Mock.Of<ILogger<OrderItemAsyncRepository>>() ,this._dbContext);
            
            // Act
            OrderItem? result = await repository.GetByIdAsync(this._orderItemOne.Id);
            
            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result!.Id, Is.EqualTo(expectedOrderItem.Id));
                Assert.That(result.OrderId, Is.EqualTo(expectedOrderItem.OrderId));
                Assert.That(result.ProductName, Is.EqualTo(expectedOrderItem.ProductName));
                Assert.That(result.ProductDescription, Is.EqualTo(expectedOrderItem.ProductDescription));
                Assert.That(result.ProductSku, Is.EqualTo(expectedOrderItem.ProductSku));
                Assert.That(result.Quantity, Is.EqualTo(expectedOrderItem.Quantity));
                Assert.That(result.Price, Is.EqualTo(expectedOrderItem.Price));
                Assert.That(result.CreatedBy, Is.EqualTo(expectedOrderItem.CreatedBy));
                Assert.That(result.CreatedDate, Is.EqualTo(expectedOrderItem.CreatedDate));
            });
        }
        
        [Test]
        public async Task GetByIdAsync_WhenOrderItemDoesNotExist_ReturnsNull()
        {
            // Arrange
            OrderItemAsyncRepository repository = new OrderItemAsyncRepository(Mock.Of<ILogger<OrderItemAsyncRepository>>() ,this._dbContext);
            
            // Act
            OrderItem? result = await repository.GetByIdAsync(100);
            
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
            Mock<DbSet<OrderItem>> mockSet = new Mock<DbSet<OrderItem>>();

            mockDbContext.Setup(x => x.OrderItems).Returns(mockSet.Object);

            OrderItemAsyncRepository repository = new OrderItemAsyncRepository(Mock.Of<ILogger<OrderItemAsyncRepository>>(), mockDbContext.Object);

            // Act
            OrderItem? result = await repository.GetByIdAsync(1);

            // Assert
            Assert.That(result, Is.Null);
        }

        #endregion

        #region AddAsync Tests

        [Test]
        public async Task AddAsync_WithNewOrderItem_ReturnsId()
        {
            // Arrange
            OrderItemAsyncRepository repository = new OrderItemAsyncRepository(Mock.Of<ILogger<OrderItemAsyncRepository>>() ,this._dbContext);
            
            // Act
            int result = await repository.AddAsync(this._orderItemThree);
            
            // Assert
            Assert.That(result, Is.EqualTo(3));
        }
        
        [Test]
        public async Task AddAsync_WithExistingOrderItem_ReturnsMinusOne()
        {
            // Arrange
            OrderItemAsyncRepository repository = new OrderItemAsyncRepository(Mock.Of<ILogger<OrderItemAsyncRepository>>() ,this._dbContext);
            
            // Act
            int result = await repository.AddAsync(this._orderItemOne);
            
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
            Mock<DatabaseFacade> mockDatabase = new Mock<DatabaseFacade>(mockDbContext.Object);
            Mock<DbSet<OrderItem>> mockSet = new Mock<DbSet<OrderItem>>();

            mockDatabase.Setup(d => d.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(Mock.Of<IDbContextTransaction>());
            mockSet.Setup(m => m.AddAsync(It.IsAny<OrderItem>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Test exception"));
            
            mockDbContext.Setup(x => x.OrderItems).Returns(mockSet.Object);
            mockDbContext.Setup(x => x.Database).Returns(mockDatabase.Object);

            OrderItemAsyncRepository repository = new OrderItemAsyncRepository(Mock.Of<ILogger<OrderItemAsyncRepository>>(), mockDbContext.Object);

            // Act
            int result = await repository.AddAsync(this._orderItemThree);

            // Assert
            Assert.That(result, Is.EqualTo(-1));
        }

        #endregion

        #region UpdateAsync Tests

        [Test]
        public async Task UpdateAsync_WithExistingOrderItem_ReturnsTrue()
        {
            // Arrange
            OrderItemAsyncRepository repository = new OrderItemAsyncRepository(Mock.Of<ILogger<OrderItemAsyncRepository>>() ,this._dbContext);
            this._orderItemOne.LastModifiedBy = _userName;
            
            // Act
            bool result = await repository.UpdateAsync(this._orderItemOne);
            
            // Assert
            Assert.That(result, Is.True);
        }
        
        [Test]
        public async Task UpdateAsync_WhenOrderItemDoesNotExist_ReturnsFalse()
        {
            // Arrange
            OrderItemAsyncRepository repository = new OrderItemAsyncRepository(Mock.Of<ILogger<OrderItemAsyncRepository>>() ,this._dbContext);
            
            // Act
            bool result = await repository.UpdateAsync(this._orderItemThree);
            
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
            Mock<DatabaseFacade> mockDatabase = new Mock<DatabaseFacade>(mockDbContext.Object);
            Mock<DbSet<OrderItem>> mockSet = new Mock<DbSet<OrderItem>>();

            mockDatabase.Setup(d => d.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(Mock.Of<IDbContextTransaction>());
            
            mockDbContext.Setup(x => x.OrderItems).Returns(mockSet.Object);
            mockDbContext.Setup(x => x.Database).Returns(mockDatabase.Object);

            OrderItemAsyncRepository repository = new OrderItemAsyncRepository(Mock.Of<ILogger<OrderItemAsyncRepository>>(), mockDbContext.Object);

            // Act
            bool result = await repository.UpdateAsync(this._orderItemOne);

            // Assert
            Assert.That(result, Is.False);
        }

        #endregion

        #region DeleteAsync Tests

        [Test]
        public async Task DeleteAsync_WhenOrderItemExists_ReturnsTrue()
        {
            // Arrange
            OrderItemAsyncRepository repository = new OrderItemAsyncRepository(Mock.Of<ILogger<OrderItemAsyncRepository>>() ,this._dbContext);
            
            // Act
            bool result = await repository.DeleteAsync(this._orderItemOne);
            
            // Assert
            Assert.That(result, Is.True);
        }
        
        [Test]
        public async Task DeleteAsync_WhenOrderItemDoesNotExist_ReturnsFalse()
        {
            // Arrange
            OrderItemAsyncRepository repository = new OrderItemAsyncRepository(Mock.Of<ILogger<OrderItemAsyncRepository>>() ,this._dbContext);
            
            // Act
            bool result = await repository.DeleteAsync(this._orderItemThree);
            
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
            Mock<DatabaseFacade> mockDatabase = new Mock<DatabaseFacade>(mockDbContext.Object);
            Mock<DbSet<OrderItem>> mockSet = new Mock<DbSet<OrderItem>>();

            mockDatabase.Setup(d => d.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(Mock.Of<IDbContextTransaction>());
            mockSet.Setup(m => m.Remove(It.IsAny<OrderItem>()))
                .Throws(new Exception("Test exception"));
            
            mockDbContext.Setup(x => x.OrderItems).Returns(mockSet.Object);
            mockDbContext.Setup(x => x.Database).Returns(mockDatabase.Object);

            OrderItemAsyncRepository repository = new OrderItemAsyncRepository(Mock.Of<ILogger<OrderItemAsyncRepository>>(), mockDbContext.Object);

            // Act
            bool result = await repository.DeleteAsync(this._orderItemOne);

            // Assert
            Assert.That(result, Is.False);
        }

        #endregion

        #region ListAllAsync Tests

        [Test]
        public async Task ListAllAsync_WhenOrderItemsExist_ReturnsOrderItems()
        {
            // Arrange
            OrderItemAsyncRepository repository = new OrderItemAsyncRepository(Mock.Of<ILogger<OrderItemAsyncRepository>>() ,this._dbContext);
            
            // Act
            IEnumerable<OrderItem> result = await repository.ListAllAsync(this._orderItemOne.OrderId);
            
            // Assert
            Assert.That(result, Is.Not.Empty);
            Assert.That(result, Has.Exactly(1).Items);
        }
        
        [Test]
        public async Task ListAllAsync_WhenNoOrderItemsExist_ReturnsEmpty()
        {
            // Arrange
            OrderItemAsyncRepository repository = new OrderItemAsyncRepository(Mock.Of<ILogger<OrderItemAsyncRepository>>() ,this._dbContext);
            
            // Act
            IEnumerable<OrderItem> result = await repository.ListAllAsync(100);
            
            // Assert
            Assert.That(result, Is.Empty);
        }
        
        [Test]
        public async Task ListAllAsync_WhenExceptionThrown_ReturnsEmpty()
        {
            // Arrange
            DbContextOptions<EcommercePersistenceDbContext> options =
                new DbContextOptionsBuilder<EcommercePersistenceDbContext>()
                    .UseInMemoryDatabase(databaseName: "Ecommerce")
                    .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                    .Options;

            Mock<EcommercePersistenceDbContext> mockDbContext = new Mock<EcommercePersistenceDbContext>(options);
            Mock<DbSet<OrderItem>> mockSet = new Mock<DbSet<OrderItem>>();

            mockDbContext.Setup(x => x.OrderItems).Returns(mockSet.Object);

            OrderItemAsyncRepository repository = new OrderItemAsyncRepository(Mock.Of<ILogger<OrderItemAsyncRepository>>(), mockDbContext.Object);

            // Act
            IEnumerable<OrderItem> result = await repository.ListAllAsync(1);

            // Assert
            Assert.That(result, Is.Empty);
        }

        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Domain.Entities;
using Ecommerce.Persistence;
using Ecommerce.Persistence.Repositories;
using Ecommerce.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Moq;
// ReSharper disable PossibleMultipleEnumeration

namespace Ecommerce.UnitTests.PersistenceTests
{
    [TestFixture]
    public class OrderAsyncRepositoryTests
    {
        private static readonly Guid _userId = new Guid("095a987b-b4da-4eb6-a286-19aa3c75be53");
        private const string _userName = "Test User";

        private readonly Order _orderOne = new Order
        {
            Id = 1,
            UserId = _userId,
            Status = OrderStatus.Created,
            Total = 100.00,
            PayPalRequestId = Guid.Empty,
            OrderItems = Array.Empty<OrderItem>(),
            CreatedBy = _userName,
            CreatedDate = DateTime.MinValue
        };

        private readonly Order _orderTwo = new Order
        {
            Id = 2,
            UserId = _userId,
            Status = OrderStatus.Created,
            Total = 200.00,
            PayPalRequestId = Guid.Empty,
            OrderItems = Array.Empty<OrderItem>(),
            CreatedBy = _userName,
            CreatedDate = DateTime.MinValue
        };
        
        private readonly Order _orderThree = new Order
        {
            Id = 3,
            UserId = _userId,
            Status = OrderStatus.Created,
            Total = 300.00,
            PayPalRequestId = Guid.Empty,
            OrderItems = Array.Empty<OrderItem>(),
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
            this._dbContext.Orders.AddRange(
                this._orderOne,
                this._orderTwo
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
        public async Task GetByIdAsync_WhenOrderExists_ReturnsOrder()
        {
            // Arrange
            Order expectedOrder = this._orderOne;

            OrderAsyncRepository repository =
                new OrderAsyncRepository(Mock.Of<ILogger<OrderAsyncRepository>>(), this._dbContext);

            // Act
            Order? result = await repository.GetByIdAsync(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result!.Id, Is.EqualTo(expectedOrder.Id));
                Assert.That(result.UserId, Is.EqualTo(expectedOrder.UserId));
                Assert.That(result.Status, Is.EqualTo(expectedOrder.Status));
                Assert.That(result.Total, Is.EqualTo(expectedOrder.Total));
                Assert.That(result.PayPalRequestId, Is.EqualTo(expectedOrder.PayPalRequestId));
                Assert.That(result.OrderItems, Is.EqualTo(expectedOrder.OrderItems));
                Assert.That(result.CreatedBy, Is.EqualTo(expectedOrder.CreatedBy));
                Assert.That(result.CreatedDate, Is.EqualTo(expectedOrder.CreatedDate));
            });
        }

        [Test]
        public async Task GetByIdAsync_WhenOrderDoesNotExist_ReturnsNull()
        {
            // Arrange
            OrderAsyncRepository repository =
                new OrderAsyncRepository(Mock.Of<ILogger<OrderAsyncRepository>>(), this._dbContext);

            // Act
            Order? result = await repository.GetByIdAsync(100);

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
            Mock<DbSet<Order>> mockSet = new Mock<DbSet<Order>>();

            mockDbContext.Setup(x => x.Orders).Returns(mockSet.Object);

            OrderAsyncRepository repository =
                new OrderAsyncRepository(Mock.Of<ILogger<OrderAsyncRepository>>(), mockDbContext.Object);

            // Act
            Order? result = await repository.GetByIdAsync(1);

            // Assert
            Assert.That(result, Is.Null);
        }

        #endregion

        #region AddAsync Tests

        [Test]
        public async Task AddAsync_WithNewOrder_ReturnsId()
        {
            // Arrange
            OrderAsyncRepository repository = new OrderAsyncRepository(Mock.Of<ILogger<OrderAsyncRepository>>(), this._dbContext);

            // Act
            int result = await repository.AddAsync(this._orderThree);

            // Assert
            Assert.That(result, Is.EqualTo(3));
        }

        [Test]
        public async Task AddAsync_WithExistingOrder_ReturnsMinusOne()
        {
            // Arrange
            OrderAsyncRepository repository =
                new OrderAsyncRepository(Mock.Of<ILogger<OrderAsyncRepository>>(), this._dbContext);

            // Act
            int result = await repository.AddAsync(this._orderOne);

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
            Mock<DbSet<Order>> mockSet = new Mock<DbSet<Order>>();
            Mock<DatabaseFacade> mockDatabase = new Mock<DatabaseFacade>(mockDbContext.Object);

            mockDatabase.Setup(d => d.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(Mock.Of<IDbContextTransaction>());
            mockSet.Setup(m => m.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Test exception"));

            mockDbContext.Setup(x => x.Orders).Returns(mockSet.Object);
            mockDbContext.Setup(x => x.Database).Returns(mockDatabase.Object);

            OrderAsyncRepository repository = new OrderAsyncRepository(Mock.Of<ILogger<OrderAsyncRepository>>(), mockDbContext.Object);

            // Act
            int result = await repository.AddAsync(this._orderThree);

            // Assert
            Assert.That(result, Is.EqualTo(-1));
        }

        #endregion

        #region UpdateAsync

        [Test]
        public async Task UpdateAsync_WithExistingOrder_ReturnsTrue()
        {
            // Arrange
            OrderAsyncRepository repository = new OrderAsyncRepository(Mock.Of<ILogger<OrderAsyncRepository>>(), this._dbContext);
            this._orderOne.LastModifiedBy = _userName;

            // Act
            bool result = await repository.UpdateAsync(this._orderOne);

            // Assert
            Assert.That(result, Is.True);
        }
        
        [Test]
        public async Task UpdateAsync_WithOrderNonExistentOrder_ReturnsFalse()
        {
            // Arrange
            OrderAsyncRepository repository = new OrderAsyncRepository(Mock.Of<ILogger<OrderAsyncRepository>>(), this._dbContext);

            // Act
            bool result = await repository.UpdateAsync(this._orderThree);

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
            Mock<DbSet<Order>> mockSet = new Mock<DbSet<Order>>();
            Mock<DatabaseFacade> mockDatabase = new Mock<DatabaseFacade>(mockDbContext.Object);

            mockDatabase.Setup(d => d.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(Mock.Of<IDbContextTransaction>());

            mockDbContext.Setup(x => x.Orders).Returns(mockSet.Object);
            mockDbContext.Setup(x => x.Database).Returns(mockDatabase.Object);

            OrderAsyncRepository repository = new OrderAsyncRepository(Mock.Of<ILogger<OrderAsyncRepository>>(), mockDbContext.Object);

            // Act
            bool result = await repository.UpdateAsync(this._orderOne);

            // Assert
            Assert.That(result, Is.False);
        }

        #endregion

        #region DeleteAsync

        [Test]
        public async Task DeleteAsync_WhenOrderExists_ReturnsTrue()
        {
            // Arrange
            OrderAsyncRepository repository = new OrderAsyncRepository(Mock.Of<ILogger<OrderAsyncRepository>>(), this._dbContext);

            // Act
            bool result = await repository.DeleteAsync(this._orderOne);

            // Assert
            Assert.That(result, Is.True);
        }
        
        [Test]
        public async Task DeleteAsync_WithOrderDoesNotExist_ReturnsFalse()
        {
            // Arrange
            OrderAsyncRepository repository = new OrderAsyncRepository(Mock.Of<ILogger<OrderAsyncRepository>>(), this._dbContext);

            // Act
            bool result = await repository.DeleteAsync(this._orderThree);

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
            Mock<DbSet<Order>> mockSet = new Mock<DbSet<Order>>();
            Mock<DatabaseFacade> mockDatabase = new Mock<DatabaseFacade>(mockDbContext.Object);

            mockDatabase.Setup(d => d.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(Mock.Of<IDbContextTransaction>());
            mockSet.Setup(m => m.Remove(It.IsAny<Order>()))
                .Throws(new Exception("Test exception"));

            mockDbContext.Setup(x => x.Orders).Returns(mockSet.Object);
            mockDbContext.Setup(x => x.Database).Returns(mockDatabase.Object);

            OrderAsyncRepository repository = new OrderAsyncRepository(Mock.Of<ILogger<OrderAsyncRepository>>(), mockDbContext.Object);

            // Act
            bool result = await repository.DeleteAsync(this._orderOne);

            // Assert
            Assert.That(result, Is.False);
        }

        #endregion

        #region ListAllAsync Tests

        [Test]
        public async Task ListAllAsync_WhenOrdersExist_ReturnsOrders()
        {
            // Arrange
            OrderAsyncRepository repository = new OrderAsyncRepository(Mock.Of<ILogger<OrderAsyncRepository>>(), this._dbContext);

            // Act
            IEnumerable<Order>? result = await repository.ListAllAsync(_userId);

            // Assert
            Assert.That(result, Is.Not.Empty);
            Assert.That(result, Has.Exactly(2).Items);
        }
        
        [Test]
        public async Task ListAllAsync_WhenNoOrdersExist_ReturnsEmpty()
        {
            // Arrange
            OrderAsyncRepository repository = new OrderAsyncRepository(Mock.Of<ILogger<OrderAsyncRepository>>(), this._dbContext);
            
            Guid randomUserId = new Guid("f8cdc893-9cd9-40fa-a0a4-5da4003a9b5f");

            // Act
            IEnumerable<Order>? result = await repository.ListAllAsync(randomUserId);

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

            mockDbContext.Setup(x => x.Orders).Throws(new Exception());

            OrderAsyncRepository repository = new OrderAsyncRepository(Mock.Of<ILogger<OrderAsyncRepository>>(), mockDbContext.Object);

            // Act
            IEnumerable<Order>? result = await repository.ListAllAsync(_userId);

            // Assert
            Assert.That(result, Is.Null);
        }

        #endregion
    }
}
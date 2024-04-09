using System;
using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Domain.Infrastructure;
using Ecommerce.Persistence;
using Ecommerce.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Moq;

namespace Ecommerce.UnitTests.PersistenceTests
{
    public class OrderKeyRepositoryTests
    {
        private const string _orderOneToken = "OrderTokenOne";
        private readonly OrderKey _orderKeyOne = new OrderKey
        {
            Id = 1,
            OrderId = 1,
            OrderToken = _orderOneToken,
            CreatedAt = DateTime.MinValue
        };
        
        private readonly OrderKey _orderKeyTwo = new OrderKey
        {
            Id = 2,
            OrderId = 2,
            OrderToken = "OrderTokenTwo",
            CreatedAt = DateTime.MinValue
        };
        
        private readonly OrderKey _orderKeyThree = new OrderKey
        {
            OrderId = 3,
            OrderToken = "OrderTokenThree",
            CreatedAt = DateTime.MinValue
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
            this._dbContext.OrderKeys.AddRange(
                this._orderKeyOne,
                this._orderKeyTwo
            );
            this._dbContext.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
            this._dbContext.Database.EnsureDeleted();
            this._dbContext.Dispose();
        }

        #region AddAsync Tests

        [Test]
        public async Task AddAsync_ValidOrderKey_ReturnsId()
        {
            // Arrange
            OrderKeyRepository repository = new OrderKeyRepository(Mock.Of<ILogger<OrderKeyRepository>>(), this._dbContext);

            // Act
            int result = await repository.AddAsync(this._orderKeyThree);

            // Assert
            Assert.That(result, Is.EqualTo(3));
        }
        
        [Test]
        public async Task AddAsync_ExistingOrderKey_ReturnsMinusOne()
        {
            // Arrange
            OrderKeyRepository repository = new OrderKeyRepository(Mock.Of<ILogger<OrderKeyRepository>>(), this._dbContext);

            // Act
            int result = await repository.AddAsync(this._orderKeyOne);

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
            Mock<DbSet<OrderKey>> mockSet = new Mock<DbSet<OrderKey>>();

            mockDatabase.Setup(d => d.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(Mock.Of<IDbContextTransaction>());
            mockSet.Setup(m => m.AddAsync(It.IsAny<OrderKey>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Test exception"));
            
            mockDbContext.Setup(x => x.OrderKeys).Returns(mockSet.Object);
            mockDbContext.Setup(x => x.Database).Returns(mockDatabase.Object);

            OrderKeyRepository repository = new OrderKeyRepository(Mock.Of<ILogger<OrderKeyRepository>>(), mockDbContext.Object);

            // Act
            int result = await repository.AddAsync(this._orderKeyThree);

            // Assert
            Assert.That(result, Is.EqualTo(-1));
        }

        #endregion

        #region DeleteAsync Tests

        [Test]
        public async Task DeleteAsync_ExistingOrderKey_ReturnsTrue()
        {
            // Arrange
            OrderKeyRepository repository = new OrderKeyRepository(Mock.Of<ILogger<OrderKeyRepository>>(), this._dbContext);

            // Act
            bool result = await repository.DeleteAsync(this._orderKeyOne);

            // Assert
            Assert.That(result, Is.True);
        }
        
        [Test]
        public async Task DeleteAsync_NonExistingOrderKey_ReturnsFalse()
        {
            // Arrange
            OrderKeyRepository repository = new OrderKeyRepository(Mock.Of<ILogger<OrderKeyRepository>>(), this._dbContext);

            // Act
            bool result = await repository.DeleteAsync(this._orderKeyThree);

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
            Mock<DbSet<OrderKey>> mockSet = new Mock<DbSet<OrderKey>>();

            mockDatabase.Setup(d => d.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(Mock.Of<IDbContextTransaction>());
            mockSet.Setup(m => m.Remove(It.IsAny<OrderKey>()));
            mockDbContext.Setup(x => x.OrderKeys).Returns(mockSet.Object);
            mockDbContext.Setup(x => x.Database).Returns(mockDatabase.Object);

            OrderKeyRepository repository = new OrderKeyRepository(Mock.Of<ILogger<OrderKeyRepository>>(), mockDbContext.Object);

            // Act
            bool result = await repository.DeleteAsync(this._orderKeyOne);

            // Assert
            Assert.That(result, Is.False);
        }

        #endregion

        #region GetByOrderIdAsync Tests

        [Test]
        public async Task GetByOrderIdAsync_ExistingOrderKey_ReturnsOrderKey()
        {
            // Arrange
            OrderKeyRepository repository = new OrderKeyRepository(Mock.Of<ILogger<OrderKeyRepository>>(), this._dbContext);

            // Act
            OrderKey? result = await repository.GetByOrderIdAsync(1);

            // Assert
            Assert.That(result, Is.EqualTo(this._orderKeyOne));
        }
        
        [Test]
        public async Task GetByOrderIdAsync_NonExistingOrderKey_ReturnsNull()
        {
            // Arrange
            OrderKeyRepository repository = new OrderKeyRepository(Mock.Of<ILogger<OrderKeyRepository>>(), this._dbContext);

            // Act
            OrderKey? result = await repository.GetByOrderIdAsync(3);

            // Assert
            Assert.That(result, Is.Null);
        }
        
        [Test]
        public async Task GetByOrderIdAsync_WhenExceptionThrown_ReturnsNull()
        {
            // Arrange
            DbContextOptions<EcommercePersistenceDbContext> options =
                new DbContextOptionsBuilder<EcommercePersistenceDbContext>()
                    .UseInMemoryDatabase(databaseName: "Ecommerce")
                    .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                    .Options;

            Mock<EcommercePersistenceDbContext> mockDbContext = new Mock<EcommercePersistenceDbContext>(options);
            Mock<DbSet<OrderKey>> mockSet = new Mock<DbSet<OrderKey>>();
            
            mockDbContext.Setup(x => x.OrderKeys).Returns(mockSet.Object);

            OrderKeyRepository repository = new OrderKeyRepository(Mock.Of<ILogger<OrderKeyRepository>>(), mockDbContext.Object);

            // Act
            OrderKey? result = await repository.GetByOrderIdAsync(1);

            // Assert
            Assert.That(result, Is.Null);
        }

        #endregion

        #region GetByReturnKeyAsync Tests

        [Test]
        public async Task GetByReturnKeyAsync_ExistingOrderKey_ReturnsOrderKey()
        {
            // Arrange
            OrderKeyRepository repository = new OrderKeyRepository(Mock.Of<ILogger<OrderKeyRepository>>(), this._dbContext);

            // Act
            OrderKey? result = await repository.GetByReturnKeyAsync(_orderOneToken);

            // Assert
            Assert.That(result, Is.EqualTo(this._orderKeyOne));
        }
        
        [Test]
        public async Task GetByReturnKeyAsync_NonExistingOrderKey_ReturnsNull()
        {
            // Arrange
            OrderKeyRepository repository = new OrderKeyRepository(Mock.Of<ILogger<OrderKeyRepository>>(), this._dbContext);

            // Act
            OrderKey? result = await repository.GetByReturnKeyAsync("NonExistingToken");

            // Assert
            Assert.That(result, Is.Null);
        }
        
        [Test]
        public async Task GetByReturnKeyAsync_WhenExceptionThrown_ReturnsNull()
        {
            // Arrange
            DbContextOptions<EcommercePersistenceDbContext> options =
                new DbContextOptionsBuilder<EcommercePersistenceDbContext>()
                    .UseInMemoryDatabase(databaseName: "Ecommerce")
                    .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                    .Options;

            Mock<EcommercePersistenceDbContext> mockDbContext = new Mock<EcommercePersistenceDbContext>(options);
            Mock<DbSet<OrderKey>> mockSet = new Mock<DbSet<OrderKey>>();
            
            mockDbContext.Setup(x => x.OrderKeys).Returns(mockSet.Object);

            OrderKeyRepository repository = new OrderKeyRepository(Mock.Of<ILogger<OrderKeyRepository>>(), mockDbContext.Object);

            // Act
            OrderKey? result = await repository.GetByReturnKeyAsync(_orderOneToken);

            // Assert
            Assert.That(result, Is.Null);
        }

        #endregion
    }
}
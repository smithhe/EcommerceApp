using System;
using System.Collections.Generic;
using System.Linq;
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
    [TestFixture]
    public class CartItemRepositoryTests
    {
        private static readonly Guid _userId = new Guid("095a987b-b4da-4eb6-a286-19aa3c75be53");
        private const string _userName = "Test User";

        private readonly CartItem _cartItemOne = new CartItem
        {
            Id = 1,
            ProductId = 1,
            Quantity = 1,
            UserId = _userId,
            CreatedBy = _userName,
            CreatedDate = DateTime.MinValue
        };

        private readonly CartItem _cartItemTwo = new CartItem
        {
            Id = 2,
            ProductId = 2,
            Quantity = 2,
            UserId = _userId,
            CreatedBy = _userName,
            CreatedDate = DateTime.MinValue
        };
        
        private readonly CartItem _cartItemThree = new CartItem
        {
            Id = 3,
            ProductId = 3,
            Quantity = 3,
            UserId = _userId,
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
            this._dbContext.CartItems.AddRange(
                this._cartItemOne,
                this._cartItemTwo
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
        public async Task GetByIdAsync_WhenCartItemExists_ShouldReturnCartItem()
        {
            // Arrange
            CartItem expectedCartItem = this._cartItemOne;

            CartItemRepository repository = new CartItemRepository(Mock.Of<ILogger<CartItemRepository>>(), this._dbContext);

            // Act
            CartItem? result = await repository.GetByIdAsync(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result?.Id, Is.EqualTo(expectedCartItem.Id));
                Assert.That(result?.ProductId, Is.EqualTo(expectedCartItem.ProductId));
                Assert.That(result?.Quantity, Is.EqualTo(expectedCartItem.Quantity));
                Assert.That(result?.UserId, Is.EqualTo(expectedCartItem.UserId));
                Assert.That(result?.CreatedBy, Is.EqualTo(expectedCartItem.CreatedBy));
                Assert.That(result?.CreatedDate, Is.EqualTo(expectedCartItem.CreatedDate));
            });
        }

        [Test]
        public async Task GetByIdAsync_WhenCartItemDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            CartItemRepository repository = new CartItemRepository(Mock.Of<ILogger<CartItemRepository>>(), this._dbContext);

            // Act
            CartItem? result = await repository.GetByIdAsync(100);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetByIdAsync_WhenExceptionThrown_ShouldReturnNull()
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


            CartItemRepository repository = new CartItemRepository(Mock.Of<ILogger<CartItemRepository>>(), mockDbContext.Object);

            // Act
            CartItem? result = await repository.GetByIdAsync(1);

            // Assert
            Assert.That(result, Is.Null);
        }

        #endregion

        #region AddAsync Tests

        [Test]
        public async Task AddAsync_WithNewCartItem_ShouldReturnId()
        {
            // Arrange
            CartItem cartItem = new CartItem
            {
                ProductId = 3,
                Quantity = 3,
                UserId = _userId,
                CreatedBy = _userName,
                CreatedDate = DateTime.MinValue
            };

            CartItemRepository repository = new CartItemRepository(Mock.Of<ILogger<CartItemRepository>>(), this._dbContext);

            // Act
            int result = await repository.AddAsync(cartItem);

            // Assert
            Assert.That(result, Is.EqualTo(3));
        }

        [Test]
        public async Task AddAsync_WithExistingCartItem_ShouldReturnMinusOne()
        {
            // Arrange
            CartItemRepository repository = new CartItemRepository(Mock.Of<ILogger<CartItemRepository>>(), this._dbContext);

            // Act
            int result = await repository.AddAsync(this._cartItemOne);

            // Assert
            Assert.That(result, Is.EqualTo(-1));
        }

        [Test]
        public async Task AddAsync_WhenExceptionThrown_ShouldReturnMinusOne()
        {
            // Arrange
            DbContextOptions<EcommercePersistenceDbContext> options =
                new DbContextOptionsBuilder<EcommercePersistenceDbContext>()
                    .UseInMemoryDatabase(databaseName: "Ecommerce")
                    .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                    .Options;

            Mock<EcommercePersistenceDbContext> mockDbContext = new Mock<EcommercePersistenceDbContext>(options);
            Mock<DbSet<CartItem>> mockSet = new Mock<DbSet<CartItem>>();
            Mock<DatabaseFacade> mockDatabase = new Mock<DatabaseFacade>(mockDbContext.Object);
            
            mockDatabase.Setup(d => d.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(Mock.Of<IDbContextTransaction>());
            mockSet.Setup(m => m.AddAsync(It.IsAny<CartItem>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Test exception"));

            mockDbContext.Setup(x => x.CartItems).Returns(mockSet.Object);
            mockDbContext.Setup(x => x.Database).Returns(mockDatabase.Object);

            CartItemRepository repository = new CartItemRepository(Mock.Of<ILogger<CartItemRepository>>(), mockDbContext.Object);

            // Act
            int result = await repository.AddAsync(this._cartItemThree);

            // Assert
            Assert.That(result, Is.EqualTo(-1));
        }

        #endregion

        #region UpdateAsync Tests

        [Test]
        public async Task UpdateAsync_WithExistingCartItem_ShouldReturnTrue()
        {
            // Arrange
            CartItemRepository repository = new CartItemRepository(Mock.Of<ILogger<CartItemRepository>>(), this._dbContext);
            this._cartItemOne.LastModifiedBy = _userName;
            
            // Act
            bool result = await repository.UpdateAsync(this._cartItemOne);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task UpdateAsync_WhenCartItemDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            CartItemRepository repository = new CartItemRepository(Mock.Of<ILogger<CartItemRepository>>(), this._dbContext);

            // Act
            bool result = await repository.UpdateAsync(this._cartItemThree);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task UpdateAsync_WhenExceptionThrown_ShouldReturnFalse()
        {
            // Arrange
            DbContextOptions<EcommercePersistenceDbContext> options =
                new DbContextOptionsBuilder<EcommercePersistenceDbContext>()
                    .UseInMemoryDatabase(databaseName: "Ecommerce")
                    .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                    .Options;

            Mock<EcommercePersistenceDbContext> mockDbContext = new Mock<EcommercePersistenceDbContext>(options);
            Mock<DbSet<CartItem>> mockSet = new Mock<DbSet<CartItem>>();
            Mock<DatabaseFacade> mockDatabase = new Mock<DatabaseFacade>(mockDbContext.Object);
            
            mockDatabase.Setup(d => d.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(Mock.Of<IDbContextTransaction>());
            
            mockDbContext.Setup(x => x.CartItems).Returns(mockSet.Object);
            mockDbContext.Setup(x => x.Database).Returns(mockDatabase.Object);

            CartItemRepository repository = new CartItemRepository(Mock.Of<ILogger<CartItemRepository>>(), mockDbContext.Object);

            // Act
            bool result = await repository.UpdateAsync(new CartItem());

            // Assert
            Assert.That(result, Is.False);
        }

        #endregion

        #region DeleteAsync Tests

        [Test]
        public async Task DeleteAsync_WhenCartItemExists_ShouldReturnTrue()
        {
            // Arrange
            CartItemRepository repository = new CartItemRepository(Mock.Of<ILogger<CartItemRepository>>(), this._dbContext);

            // Act
            bool result = await repository.DeleteAsync(this._cartItemOne);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task DeleteAsync_WhenCartItemDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            CartItemRepository repository = new CartItemRepository(Mock.Of<ILogger<CartItemRepository>>(), this._dbContext);

            // Act
            bool result = await repository.DeleteAsync(this._cartItemThree);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task DeleteAsync_WhenExceptionThrown_ShouldReturnFalse()
        {
            // Arrange
            DbContextOptions<EcommercePersistenceDbContext> options =
                new DbContextOptionsBuilder<EcommercePersistenceDbContext>()
                    .UseInMemoryDatabase(databaseName: "Ecommerce")
                    .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                    .Options;

            Mock<EcommercePersistenceDbContext> mockDbContext = new Mock<EcommercePersistenceDbContext>(options);
            Mock<DbSet<CartItem>> mockSet = new Mock<DbSet<CartItem>>();
            Mock<DatabaseFacade> mockDatabase = new Mock<DatabaseFacade>(mockDbContext.Object);
            
            mockDatabase.Setup(d => d.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(Mock.Of<IDbContextTransaction>());
            mockSet.Setup(m => m.Remove(It.IsAny<CartItem>()))
                .Throws(new Exception("Test exception"));

            mockDbContext.Setup(x => x.CartItems).Returns(mockSet.Object);
            mockDbContext.Setup(x => x.Database).Returns(mockDatabase.Object);

            CartItemRepository repository = new CartItemRepository(Mock.Of<ILogger<CartItemRepository>>(), mockDbContext.Object);

            // Act
            bool result = await repository.DeleteAsync(new CartItem());

            // Assert
            Assert.That(result, Is.False);
        }

        #endregion

        #region ListAllAsync Tests

        [Test]
        public async Task ListAllAsync_WhenCartItemsExist_ShouldReturnCartItems()
        {
            // Arrange
            CartItemRepository repository = new CartItemRepository(Mock.Of<ILogger<CartItemRepository>>(), this._dbContext);

            // Act
            IEnumerable<CartItem>? result = await repository.ListAllAsync(_userId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task ListAllAsync_WhenNoCartItemsExist_ShouldReturnEmptyList()
        {
            // Arrange
            CartItemRepository repository = new CartItemRepository(Mock.Of<ILogger<CartItemRepository>>(), this._dbContext);

            Guid randomUserId = new Guid("f8cdc893-9cd9-40fa-a0a4-5da4003a9b5f");
            
            // Act
            IEnumerable<CartItem>? result = await repository.ListAllAsync(randomUserId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task ListAllAsync_WhenExceptionThrown_ShouldReturnNull()
        {
            // Arrange
            DbContextOptions<EcommercePersistenceDbContext> options =
                new DbContextOptionsBuilder<EcommercePersistenceDbContext>()
                    .UseInMemoryDatabase(databaseName: "Ecommerce")
                    .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                    .Options;

            Mock<EcommercePersistenceDbContext> mockDbContext = new Mock<EcommercePersistenceDbContext>(options);

            mockDbContext.Setup(x => x.CartItems).Throws(new Exception());


            CartItemRepository repository = new CartItemRepository(Mock.Of<ILogger<CartItemRepository>>(), mockDbContext.Object);

            // Act
            IEnumerable<CartItem>? result = await repository.ListAllAsync(_userId);

            // Assert
            Assert.That(result, Is.Null);
        }

        #endregion

        #region RemoveUserCartItems Tests

        [Test]
        public async Task RemoveUserCartItems_WhenCartItemsExist_ShouldReturnTrue()
        {
            // Arrange
            CartItemRepository repository = new CartItemRepository(Mock.Of<ILogger<CartItemRepository>>(), this._dbContext);

            // Act
            bool result = await repository.RemoveUserCartItems(_userId);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task RemoveUserCartItems_WhenNoCartItemsExist_ShouldReturnFalse()
        {
            // Arrange
            CartItemRepository repository = new CartItemRepository(Mock.Of<ILogger<CartItemRepository>>(), this._dbContext);

            // Act
            bool result = await repository.RemoveUserCartItems(Guid.NewGuid());

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task RemoveUserCartItems_WhenExceptionThrown_ShouldReturnFalse()
        {
            // Arrange
            DbContextOptions<EcommercePersistenceDbContext> options =
                new DbContextOptionsBuilder<EcommercePersistenceDbContext>()
                    .UseInMemoryDatabase(databaseName: "Ecommerce")
                    .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                    .Options;

            Mock<EcommercePersistenceDbContext> mockDbContext = new Mock<EcommercePersistenceDbContext>(options);
            Mock<DbSet<CartItem>> mockSet = new Mock<DbSet<CartItem>>();
            Mock<DatabaseFacade> mockDatabase = new Mock<DatabaseFacade>(mockDbContext.Object);
            
            mockDatabase.Setup(d => d.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(Mock.Of<IDbContextTransaction>());
            mockSet.Setup(m => m.RemoveRange(It.IsAny<CartItem>()))
                .Throws(new Exception("Test exception"));

            mockDbContext.Setup(x => x.CartItems).Returns(mockSet.Object);
            mockDbContext.Setup(x => x.Database).Returns(mockDatabase.Object);

            CartItemRepository repository = new CartItemRepository(Mock.Of<ILogger<CartItemRepository>>(), mockDbContext.Object);

            // Act
            bool result = await repository.RemoveUserCartItems(_userId);

            // Assert
            Assert.That(result, Is.False);
        }

        #endregion

        #region CartItemExistsForUser Tests

        [Test]
        public async Task CartItemExistsForUser_WhenCartItemExists_ShouldReturnTrue()
        {
            // Arrange
            CartItemRepository repository = new CartItemRepository(Mock.Of<ILogger<CartItemRepository>>(), this._dbContext);

            // Act
            bool result = await repository.CartItemExistsForUser(_userId, 1);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task CartItemExistsForUser_WhenNoCartItemExists_ShouldReturnFalse()
        {
            // Arrange
            CartItemRepository repository = new CartItemRepository(Mock.Of<ILogger<CartItemRepository>>(), this._dbContext);

            // Act
            bool result = await repository.CartItemExistsForUser(_userId, 100);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task CartItemExistsForUser_WhenExceptionThrown_ShouldReturnFalse()
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


            CartItemRepository repository = new CartItemRepository(Mock.Of<ILogger<CartItemRepository>>(), mockDbContext.Object);

            // Act
            bool result = await repository.CartItemExistsForUser(_userId, 1);

            // Assert
            Assert.That(result, Is.False);
        }

        #endregion
    }
}
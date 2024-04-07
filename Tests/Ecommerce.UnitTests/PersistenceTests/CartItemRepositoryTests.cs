using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Ecommerce.Domain.Entities;
using Ecommerce.Persistence;
using Ecommerce.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Moq;
// ReSharper disable PossibleMultipleEnumeration

namespace Ecommerce.UnitTests.PersistenceTests
{
    public class CartItemRepositoryTests
    {
        private readonly Guid _userId = new Guid("095a987b-b4da-4eb6-a286-19aa3c75be53");
        private const string _userName = "Test User";

        private CartItem _cartItemOne = null!;
        private CartItem _cartItemTwo = null!;

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

            this._cartItemOne = new CartItem
            {
                Id = 1,
                ProductId = 1,
                Quantity = 1,
                UserId = this._userId,
                CreatedBy = _userName,
                CreatedDate = DateTime.MinValue
            };

            this._cartItemTwo = new CartItem
            {
                Id = 2,
                ProductId = 2,
                Quantity = 2,
                UserId = this._userId,
                CreatedBy = _userName,
                CreatedDate = DateTime.MinValue
            };
            
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
            CartItem cartItem = new CartItem
            {
                Id = 1,
                ProductId = 1,
                Quantity = 1,
                CreatedBy = _userName,
                CreatedDate = DateTime.MinValue
            };

            Mock<ILogger<CartItemRepository>> logger = new Mock<ILogger<CartItemRepository>>();
            CartItemRepository repository = new CartItemRepository(logger.Object, this._dbContext);

            // Act
            CartItem? result = await repository.GetByIdAsync(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result?.Id, Is.EqualTo(cartItem.Id));
                Assert.That(result?.ProductId, Is.EqualTo(cartItem.ProductId));
                Assert.That(result?.Quantity, Is.EqualTo(cartItem.Quantity));
            });
        }

        [Test]
        public async Task GetByIdAsync_WhenCartItemDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            Mock<ILogger<CartItemRepository>> logger = new Mock<ILogger<CartItemRepository>>();
            CartItemRepository repository = new CartItemRepository(logger.Object, this._dbContext);

            // Act
            CartItem? result = await repository.GetByIdAsync(100);

            // Assert
            Assert.That(result, Is.Null);
        }

        #endregion
        
        #region AddAsync Tests
        
        [Test]
        public async Task AddAsync_WhenCartItemIsValid_ShouldReturnId()
        {
            // Arrange
            CartItem cartItem = new CartItem
            {
                ProductId = 3,
                Quantity = 3,
                UserId = this._userId,
                CreatedBy = _userName,
                CreatedDate = DateTime.MinValue
            };

            Mock<ILogger<CartItemRepository>> logger = new Mock<ILogger<CartItemRepository>>();
            CartItemRepository repository = new CartItemRepository(logger.Object, this._dbContext);

            // Act
            int result = await repository.AddAsync(cartItem);

            // Assert
            Assert.That(result, Is.EqualTo(3));
        }
        
        [Test]
        public async Task AddAsync_WhenExceptionThrown_ShouldReturnMinusOne()
        {
            // Arrange
            Mock<ILogger<CartItemRepository>> logger = new Mock<ILogger<CartItemRepository>>();
            
            Mock<DbSet<CartItem>> mockSet = new Mock<DbSet<CartItem>>();
            mockSet.Setup(x => x.AddAsync(It.IsAny<CartItem>(), default)).ThrowsAsync(new Exception());
            
            Mock<EcommercePersistenceDbContext> mockDbContext = new Mock<EcommercePersistenceDbContext>();
            mockDbContext.Setup(x => x.CartItems).Returns(mockSet.Object);
            
            CartItemRepository repository = new CartItemRepository(logger.Object, this._dbContext);
            
            // Act
            int result = await repository.AddAsync(new CartItem());

            // Assert
            Assert.That(result, Is.EqualTo(-1));
        }
        
        #endregion

        #region UpdateAsync Tests

        [Test]
        public async Task UpdateAsync_WhenCartItemIsValid_ShouldReturnTrue()
        {
            // Arrange
            CartItem cartItem = new CartItem
            {
                Id = 1,
                ProductId = 1,
                Quantity = 5,
                UserId = this._userId,
                CreatedBy = _userName,
                CreatedDate = DateTime.MinValue
            };

            Mock<ILogger<CartItemRepository>> logger = new Mock<ILogger<CartItemRepository>>();
            CartItemRepository repository = new CartItemRepository(logger.Object, this._dbContext);

            // Act
            bool result = await repository.UpdateAsync(cartItem);

            // Assert
            Assert.That(result, Is.True);
        }
        
        [Test]
        public async Task UpdateAsync_WhenCartItemDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            CartItem cartItem = new CartItem
            {
                Id = 100,
                ProductId = 1,
                Quantity = 5,
                UserId = this._userId,
                CreatedBy = _userName,
                CreatedDate = DateTime.MinValue
            };

            Mock<ILogger<CartItemRepository>> logger = new Mock<ILogger<CartItemRepository>>();
            CartItemRepository repository = new CartItemRepository(logger.Object, this._dbContext);

            // Act
            bool result = await repository.UpdateAsync(cartItem);

            // Assert
            Assert.That(result, Is.False);
        }
        
        [Test]
        public async Task UpdateAsync_WhenExceptionThrown_ShouldReturnFalse()
        {
            // Arrange
            Mock<ILogger<CartItemRepository>> logger = new Mock<ILogger<CartItemRepository>>();
            
            Mock<IQueryProvider> mockProvider = new Mock<IQueryProvider>();
            mockProvider
                .Setup(p => p.Execute(It.IsAny<Expression>()))
                .Throws(new Exception("Database error"));
            
            Mock<DbSet<CartItem>> mockSet = new Mock<DbSet<CartItem>>();
            
            Mock<EcommercePersistenceDbContext> mockDbContext = new Mock<EcommercePersistenceDbContext>();
            mockDbContext.Setup(x => x.CartItems).Returns(mockSet.Object);
            
            CartItemRepository repository = new CartItemRepository(logger.Object, this._dbContext);
            
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
            Mock<ILogger<CartItemRepository>> logger = new Mock<ILogger<CartItemRepository>>();
            CartItemRepository repository = new CartItemRepository(logger.Object, this._dbContext);

            // Act
            bool result = await repository.DeleteAsync(this._cartItemOne);

            // Assert
            Assert.That(result, Is.True);
        }
        
        [Test]
        public async Task DeleteAsync_WhenCartItemDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            CartItem cartItem = new CartItem
            {
                Id = 100,
                ProductId = 1,
                Quantity = 5,
                UserId = this._userId,
                CreatedBy = _userName,
                CreatedDate = DateTime.MinValue
            };

            Mock<ILogger<CartItemRepository>> logger = new Mock<ILogger<CartItemRepository>>();
            CartItemRepository repository = new CartItemRepository(logger.Object, this._dbContext);

            // Act
            bool result = await repository.DeleteAsync(cartItem);

            // Assert
            Assert.That(result, Is.False);
        }
        
        [Test]
        public async Task DeleteAsync_WhenExceptionThrown_ShouldReturnFalse()
        {
            // Arrange
            Mock<ILogger<CartItemRepository>> logger = new Mock<ILogger<CartItemRepository>>();
            
            Mock<IQueryProvider> mockProvider = new Mock<IQueryProvider>();
            mockProvider
                .Setup(p => p.Execute(It.IsAny<Expression>()))
                .Throws(new Exception("Database error"));
            
            Mock<DbSet<CartItem>> mockSet = new Mock<DbSet<CartItem>>();
            
            Mock<EcommercePersistenceDbContext> mockDbContext = new Mock<EcommercePersistenceDbContext>();
            mockDbContext.Setup(x => x.CartItems).Returns(mockSet.Object);
            
            CartItemRepository repository = new CartItemRepository(logger.Object, this._dbContext);
            
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
            Mock<ILogger<CartItemRepository>> logger = new Mock<ILogger<CartItemRepository>>();
            CartItemRepository repository = new CartItemRepository(logger.Object, this._dbContext);

            // Act
            IEnumerable<CartItem> result = await repository.ListAllAsync(this._userId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
        }
        
        [Test]
        public async Task ListAllAsync_WhenNoCartItemsExist_ShouldReturnEmptyList()
        {
            // Arrange
            Mock<ILogger<CartItemRepository>> logger = new Mock<ILogger<CartItemRepository>>();
            CartItemRepository repository = new CartItemRepository(logger.Object, this._dbContext);

            // Act
            IEnumerable<CartItem> result = await repository.ListAllAsync(Guid.NewGuid());

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(0));
        }
        
        [Test]
        public async Task ListAllAsync_WhenExceptionThrown_ShouldReturnEmptyList()
        {
            // Arrange
            Mock<ILogger<CartItemRepository>> logger = new Mock<ILogger<CartItemRepository>>();
            
            Mock<IQueryProvider> mockProvider = new Mock<IQueryProvider>();
            mockProvider
                .Setup(p => p.Execute(It.IsAny<Expression>()))
                .Throws(new Exception("Database error"));
            
            Mock<DbSet<CartItem>> mockSet = new Mock<DbSet<CartItem>>();
            
            Mock<EcommercePersistenceDbContext> mockDbContext = new Mock<EcommercePersistenceDbContext>();
            mockDbContext.Setup(x => x.CartItems).Returns(mockSet.Object);
            
            CartItemRepository repository = new CartItemRepository(logger.Object, this._dbContext);
            
            // Act
            IEnumerable<CartItem> result = await repository.ListAllAsync(Guid.NewGuid());

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(0));
        }

        #endregion
        
        #region RemoveUserCartItems Tests
        
        [Test]
        public async Task RemoveUserCartItems_WhenCartItemsExist_ShouldReturnTrue()
        {
            // Arrange
            Mock<ILogger<CartItemRepository>> logger = new Mock<ILogger<CartItemRepository>>();
            CartItemRepository repository = new CartItemRepository(logger.Object, this._dbContext);

            // Act
            bool result = await repository.RemoveUserCartItems(this._userId);

            // Assert
            Assert.That(result, Is.True);
        }
        
        [Test]
        public async Task RemoveUserCartItems_WhenNoCartItemsExist_ShouldReturnFalse()
        {
            // Arrange
            Mock<ILogger<CartItemRepository>> logger = new Mock<ILogger<CartItemRepository>>();
            CartItemRepository repository = new CartItemRepository(logger.Object, this._dbContext);

            // Act
            bool result = await repository.RemoveUserCartItems(Guid.NewGuid());

            // Assert
            Assert.That(result, Is.False);
        }
        
        [Test]
        public async Task RemoveUserCartItems_WhenExceptionThrown_ShouldReturnFalse()
        {
            // Arrange
            Mock<ILogger<CartItemRepository>> logger = new Mock<ILogger<CartItemRepository>>();
            
            Mock<IQueryProvider> mockProvider = new Mock<IQueryProvider>();
            mockProvider
                .Setup(p => p.Execute(It.IsAny<Expression>()))
                .Throws(new Exception("Database error"));
            
            Mock<DbSet<CartItem>> mockSet = new Mock<DbSet<CartItem>>();
            
            Mock<EcommercePersistenceDbContext> mockDbContext = new Mock<EcommercePersistenceDbContext>();
            mockDbContext.Setup(x => x.CartItems).Returns(mockSet.Object);
            
            CartItemRepository repository = new CartItemRepository(logger.Object, this._dbContext);
            
            // Act
            bool result = await repository.RemoveUserCartItems(Guid.NewGuid());

            // Assert
            Assert.That(result, Is.False);
        }
        
        #endregion
        
        #region CartItemExistsForUser Tests
        
        [Test]
        public async Task CartItemExistsForUser_WhenCartItemExists_ShouldReturnTrue()
        {
            // Arrange
            Mock<ILogger<CartItemRepository>> logger = new Mock<ILogger<CartItemRepository>>();
            CartItemRepository repository = new CartItemRepository(logger.Object, this._dbContext);

            // Act
            bool result = await repository.CartItemExistsForUser(this._userId, 1);

            // Assert
            Assert.That(result, Is.True);
        }
        
        [Test]
        public async Task CartItemExistsForUser_WhenCartItemDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            Mock<ILogger<CartItemRepository>> logger = new Mock<ILogger<CartItemRepository>>();
            CartItemRepository repository = new CartItemRepository(logger.Object, this._dbContext);

            // Act
            bool result = await repository.CartItemExistsForUser(this._userId, 100);

            // Assert
            Assert.That(result, Is.False);
        }
        
        [Test]
        public async Task CartItemExistsForUser_WhenExceptionThrown_ShouldReturnFalse()
        {
            // Arrange
            Mock<ILogger<CartItemRepository>> logger = new Mock<ILogger<CartItemRepository>>();
            
            Mock<IQueryProvider> mockProvider = new Mock<IQueryProvider>();
            mockProvider
                .Setup(p => p.Execute(It.IsAny<Expression>()))
                .Throws(new Exception("Database error"));
            
            Mock<DbSet<CartItem>> mockSet = new Mock<DbSet<CartItem>>();
            
            Mock<EcommercePersistenceDbContext> mockDbContext = new Mock<EcommercePersistenceDbContext>();
            mockDbContext.Setup(x => x.CartItems).Returns(mockSet.Object);
            
            CartItemRepository repository = new CartItemRepository(logger.Object, this._dbContext);
            
            // Act
            bool result = await repository.CartItemExistsForUser(Guid.NewGuid(), 1);

            // Assert
            Assert.That(result, Is.False);
        }
        
        #endregion
    }
}
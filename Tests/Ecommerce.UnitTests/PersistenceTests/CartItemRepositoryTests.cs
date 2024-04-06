using System;
using System.Threading.Tasks;
using Ecommerce.Domain.Entities;
using Ecommerce.Persistence;
using Ecommerce.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace Ecommerce.UnitTests.PersistenceTests
{
    public class CartItemRepositoryTests
    {
        private readonly Guid _userId = new Guid("095a987b-b4da-4eb6-a286-19aa3c75be53");
        private const string _userName = "Test User";
        
        
        private EcommercePersistenceDbContext _dbContext = null!;
        
        [SetUp]
        public void Setup()
        {
            DbContextOptions<EcommercePersistenceDbContext> options = new DbContextOptionsBuilder<EcommercePersistenceDbContext>()
                .UseInMemoryDatabase(databaseName: "Ecommerce")
                .Options;
            
            this._dbContext = new EcommercePersistenceDbContext(options);
            
            //Seed the database
            this._dbContext.CartItems.Add(new CartItem
            {
                Id = 1,
                ProductId = 1,
                Quantity = 1,
                UserId = this._userId,
                CreatedBy = _userName,
                CreatedDate = DateTime.MinValue
            });
            
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

        #endregion
    }
}
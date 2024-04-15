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
    [TestFixture]
    public class ReviewAsyncRepositoryTests
    {
        private const string _userName = "Test User";
        
        private readonly Review _reviewOne = new Review
        {
            Id = 1,
            ProductId = 1,
            UserName = _userName,
            Stars = 5,
            Comments = "Great product!",
            CreatedBy = _userName,
            CreatedDate = DateTime.MinValue
        };
        
        private readonly Review _reviewTwo = new Review
        {
            Id = 2,
            ProductId = 2,
            UserName = _userName,
            Stars = 4,
            Comments = "Good product!",
            CreatedBy = _userName,
            CreatedDate = DateTime.MinValue
        };
        
        private readonly Review _reviewThree = new Review
        {
            ProductId = 3,
            UserName = _userName,
            Stars = 3,
            Comments = "Average product!",
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
            this._dbContext.Reviews.AddRange(
                this._reviewOne,
                this._reviewTwo
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
        public async Task GetByIdAsync_WhenReviewExists_ReturnsReview()
        {
            // Arrange
            Review expectedReview = this._reviewOne;
            ReviewAsyncRepository repository = new ReviewAsyncRepository(Mock.Of<ILogger<ReviewAsyncRepository>>(), this._dbContext);

            // Act
            Review? result = await repository.GetByIdAsync(this._reviewOne.Id);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result!.Id, Is.EqualTo(expectedReview.Id));
                Assert.That(result.ProductId, Is.EqualTo(expectedReview.ProductId));
                Assert.That(result.UserName, Is.EqualTo(expectedReview.UserName));
                Assert.That(result.Stars, Is.EqualTo(expectedReview.Stars));
                Assert.That(result.Comments, Is.EqualTo(expectedReview.Comments));
                Assert.That(result.CreatedBy, Is.EqualTo(expectedReview.CreatedBy));
                Assert.That(result.CreatedDate, Is.EqualTo(expectedReview.CreatedDate));
            });
        }
        
        [Test]
        public async Task GetByIdAsync_WhenReviewDoesNotExist_ReturnsNull()
        {
            // Arrange
            ReviewAsyncRepository repository = new ReviewAsyncRepository(Mock.Of<ILogger<ReviewAsyncRepository>>(), this._dbContext);

            // Act
            Review? result = await repository.GetByIdAsync(100);

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
            Mock<DbSet<Review>> mockSet = new Mock<DbSet<Review>>();

            mockDbContext.Setup(x => x.Reviews).Returns(mockSet.Object);
            
            ReviewAsyncRepository repository = new ReviewAsyncRepository(Mock.Of<ILogger<ReviewAsyncRepository>>(), mockDbContext.Object);
            
            // Act
            Review? result = await repository.GetByIdAsync(1);

            // Assert
            Assert.That(result, Is.Null);
        }

        #endregion

        #region AddAsync Tests

        [Test]
        public async Task AddAsync_WithNewReview_ReturnsId()
        {
            // Arrange
            ReviewAsyncRepository repository = new ReviewAsyncRepository(Mock.Of<ILogger<ReviewAsyncRepository>>(), this._dbContext);

            // Act
            int result = await repository.AddAsync(this._reviewThree);

            // Assert
            Assert.That(result, Is.EqualTo(3));
        }
        
        [Test]
        public async Task AddAsync_WithExistingReview_ReturnsMinusOne()
        {
            // Arrange
            ReviewAsyncRepository repository = new ReviewAsyncRepository(Mock.Of<ILogger<ReviewAsyncRepository>>(), this._dbContext);

            // Act
            int result = await repository.AddAsync(this._reviewOne);

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
            Mock<DbSet<Review>> mockSet = new Mock<DbSet<Review>>();
            Mock<DatabaseFacade> mockDatabase = new Mock<DatabaseFacade>(mockDbContext.Object);
            
            mockDatabase.Setup(d => d.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(Mock.Of<IDbContextTransaction>());
            mockSet.Setup(m => m.AddAsync(It.IsAny<Review>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Test exception"));

            mockDbContext.Setup(x => x.Reviews).Returns(mockSet.Object);
            mockDbContext.Setup(x => x.Database).Returns(mockDatabase.Object);
            
            ReviewAsyncRepository repository = new ReviewAsyncRepository(Mock.Of<ILogger<ReviewAsyncRepository>>(), mockDbContext.Object);
            
            // Act
            int result = await repository.AddAsync(this._reviewThree);

            // Assert
            Assert.That(result, Is.EqualTo(-1));
        }

        #endregion

        #region UpdateAsync Tests

        [Test]
        public async Task UpdateAsync_WithExistingReview_ReturnsTrue()
        {
            // Arrange
            ReviewAsyncRepository repository = new ReviewAsyncRepository(Mock.Of<ILogger<ReviewAsyncRepository>>(), this._dbContext);
            this._reviewOne.LastModifiedBy = _userName;

            // Act
            bool result = await repository.UpdateAsync(this._reviewOne);

            // Assert
            Assert.That(result, Is.True);
        }
        
        [Test]
        public async Task UpdateAsync_WhenReviewDoesNotExist_ReturnsFalse()
        {
            // Arrange
            ReviewAsyncRepository repository = new ReviewAsyncRepository(Mock.Of<ILogger<ReviewAsyncRepository>>(), this._dbContext);

            // Act
            bool result = await repository.UpdateAsync(this._reviewThree);

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
            Mock<DbSet<Review>> mockSet = new Mock<DbSet<Review>>();
            Mock<DatabaseFacade> mockDatabase = new Mock<DatabaseFacade>(mockDbContext.Object);
            
            mockDatabase.Setup(d => d.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(Mock.Of<IDbContextTransaction>());

            mockDbContext.Setup(x => x.Reviews).Returns(mockSet.Object);
            mockDbContext.Setup(x => x.Database).Returns(mockDatabase.Object);
            
            ReviewAsyncRepository repository = new ReviewAsyncRepository(Mock.Of<ILogger<ReviewAsyncRepository>>(), mockDbContext.Object);
            
            // Act
            bool result = await repository.UpdateAsync(this._reviewOne);

            // Assert
            Assert.That(result, Is.False);
        }

        #endregion

        #region DeleteAsync Tests

        [Test]
        public async Task DeleteAsync_WhenReviewExists_ReturnsTrue()
        {
            // Arrange
            ReviewAsyncRepository repository = new ReviewAsyncRepository(Mock.Of<ILogger<ReviewAsyncRepository>>(), this._dbContext);

            // Act
            bool result = await repository.DeleteAsync(this._reviewOne);

            // Assert
            Assert.That(result, Is.True);
        }
        
        [Test]
        public async Task DeleteAsync_WhenReviewDoesNotExist_ReturnsFalse()
        {
            // Arrange
            ReviewAsyncRepository repository = new ReviewAsyncRepository(Mock.Of<ILogger<ReviewAsyncRepository>>(), this._dbContext);

            // Act
            bool result = await repository.DeleteAsync(this._reviewThree);

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
            Mock<DbSet<Review>> mockSet = new Mock<DbSet<Review>>();
            Mock<DatabaseFacade> mockDatabase = new Mock<DatabaseFacade>(mockDbContext.Object);
            
            mockDatabase.Setup(d => d.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(Mock.Of<IDbContextTransaction>());
            mockSet.Setup(m => m.Remove(It.IsAny<Review>()))
                .Throws(new Exception("Test exception"));

            mockDbContext.Setup(x => x.Reviews).Returns(mockSet.Object);
            mockDbContext.Setup(x => x.Database).Returns(mockDatabase.Object);
            
            ReviewAsyncRepository repository = new ReviewAsyncRepository(Mock.Of<ILogger<ReviewAsyncRepository>>(), mockDbContext.Object);
            
            // Act
            bool result = await repository.DeleteAsync(this._reviewOne);

            // Assert
            Assert.That(result, Is.False);
        }

        #endregion

        #region ListAllAsync Tests

        [Test]
        public async Task ListAllAsync_WhenReviewsExist_ReturnsReviews()
        {
            // Arrange
            ReviewAsyncRepository repository = new ReviewAsyncRepository(Mock.Of<ILogger<ReviewAsyncRepository>>(), this._dbContext);

            // Act
            IEnumerable<Review> result = await repository.ListAllAsync(1);

            // Assert
            Assert.That(result, Is.Not.Empty);
            Assert.That(result, Has.Exactly(1).Items);
        }
        
        [Test]
        public async Task ListAllAsync_WhenNoReviewsExist_ReturnsEmpty()
        {
            // Arrange
            ReviewAsyncRepository repository = new ReviewAsyncRepository(Mock.Of<ILogger<ReviewAsyncRepository>>(), this._dbContext);

            // Act
            IEnumerable<Review> result = await repository.ListAllAsync(3);

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
            Mock<DbSet<Review>> mockSet = new Mock<DbSet<Review>>();
            
            mockDbContext.Setup(x => x.Reviews).Returns(mockSet.Object);
            
            ReviewAsyncRepository repository = new ReviewAsyncRepository(Mock.Of<ILogger<ReviewAsyncRepository>>(), mockDbContext.Object);
            
            // Act
            IEnumerable<Review> result = await repository.ListAllAsync(1);

            // Assert
            Assert.That(result, Is.Empty);
        }

        #endregion

        #region GetUserReviewForProduct Tests

        [Test]
        public async Task GetUserReviewForProduct_WhenReviewExists_ReturnsReview()
        {
            // Arrange
            ReviewAsyncRepository repository = new ReviewAsyncRepository(Mock.Of<ILogger<ReviewAsyncRepository>>(), this._dbContext);

            // Act
            Review? result = await repository.GetUserReviewForProduct(_userName, 1);

            // Assert
            Assert.That(result, Is.Not.Null);
        }
        
        [Test]
        public async Task GetUserReviewForProduct_WhenReviewDoesNotExist_ReturnsNull()
        {
            // Arrange
            ReviewAsyncRepository repository = new ReviewAsyncRepository(Mock.Of<ILogger<ReviewAsyncRepository>>(), this._dbContext);

            // Act
            Review? result = await repository.GetUserReviewForProduct(_userName, 3);

            // Assert
            Assert.That(result, Is.Null);
        }
        
        [Test]
        public async Task GetUserReviewForProduct_WhenExceptionThrown_ReturnsNull()
        {
            // Arrange
            DbContextOptions<EcommercePersistenceDbContext> options =
                new DbContextOptionsBuilder<EcommercePersistenceDbContext>()
                    .UseInMemoryDatabase(databaseName: "Ecommerce")
                    .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                    .Options;
            
            Mock<EcommercePersistenceDbContext> mockDbContext = new Mock<EcommercePersistenceDbContext>(options);
            Mock<DbSet<Review>> mockSet = new Mock<DbSet<Review>>();
            
            mockDbContext.Setup(x => x.Reviews).Returns(mockSet.Object);
            
            ReviewAsyncRepository repository = new ReviewAsyncRepository(Mock.Of<ILogger<ReviewAsyncRepository>>(), mockDbContext.Object);
            
            // Act
            Review? result = await repository.GetUserReviewForProduct(_userName, 1);

            // Assert
            Assert.That(result, Is.Null);
        }

        #endregion

        #region GetAverageRatingForProduct Tests

        [Test]
        public async Task GetAverageRatingForProduct_WhenReviewsExist_ReturnsAverage()
        {
            // Arrange
            ReviewAsyncRepository repository = new ReviewAsyncRepository(Mock.Of<ILogger<ReviewAsyncRepository>>(), this._dbContext);

            // Act
            decimal result = await repository.GetAverageRatingForProduct(1);

            // Assert
            Assert.That(result, Is.EqualTo(5));
        }
        
        [Test]
        public async Task GetAverageRatingForProduct_WhenNoReviewsExist_ReturnsZero()
        {
            // Arrange
            ReviewAsyncRepository repository = new ReviewAsyncRepository(Mock.Of<ILogger<ReviewAsyncRepository>>(), this._dbContext);

            // Act
            decimal result = await repository.GetAverageRatingForProduct(3);

            // Assert
            Assert.That(result, Is.Zero);
        }
        
        [Test]
        public async Task GetAverageRatingForProduct_WhenExceptionThrown_ReturnsZero()
        {
            // Arrange
            DbContextOptions<EcommercePersistenceDbContext> options =
                new DbContextOptionsBuilder<EcommercePersistenceDbContext>()
                    .UseInMemoryDatabase(databaseName: "Ecommerce")
                    .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                    .Options;
            
            Mock<EcommercePersistenceDbContext> mockDbContext = new Mock<EcommercePersistenceDbContext>(options);
            Mock<DbSet<Review>> mockSet = new Mock<DbSet<Review>>();
            
            mockDbContext.Setup(x => x.Reviews).Returns(mockSet.Object);
            
            ReviewAsyncRepository repository = new ReviewAsyncRepository(Mock.Of<ILogger<ReviewAsyncRepository>>(), mockDbContext.Object);
            
            // Act
            decimal result = await repository.GetAverageRatingForProduct(1);

            // Assert
            Assert.That(result, Is.Zero);
        }

        #endregion
    }
}
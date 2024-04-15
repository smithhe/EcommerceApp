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
    public class CategoryAsyncRepositoryTests
    {
        private const string _userName = "Test User";

        private EcommercePersistenceDbContext _dbContext = null!;

        private readonly Category _categoryOne = new Category
        {
            Id = 1,
            Name = "Category 1",
            Summary = "Category 1 Summary",
            CreatedBy = _userName,
            CreatedDate = DateTime.MinValue
        };

        private readonly Category _categoryTwo = new Category
        {
            Id = 2,
            Name = "Category 2",
            Summary = "Category 2 Summary",
            CreatedBy = _userName,
            CreatedDate = DateTime.MinValue
        };
        
        private readonly Category _categoryThree = new Category
        {
            Id = 3,
            Name = "Category 3",
            Summary = "Category 3 Summary",
            CreatedBy = _userName,
            CreatedDate = DateTime.MinValue
        };

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
            this._dbContext.Categories.AddRange(
                this._categoryOne,
                this._categoryTwo
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
        public async Task GetByIdAsync_WhenCategoryExists_ReturnsCategory()
        {
            // Arrange
            Category expectedCategory = this._categoryOne;
            CategoryAsyncRepository repository = new CategoryAsyncRepository(Mock.Of<ILogger<CategoryAsyncRepository>>(), this._dbContext);

            // Act
            Category? result = await repository.GetByIdAsync(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result?.Id, Is.EqualTo(expectedCategory.Id));
                Assert.That(result?.Name, Is.EqualTo(expectedCategory.Name));
                Assert.That(result?.Summary, Is.EqualTo(expectedCategory.Summary));
                Assert.That(result?.CreatedBy, Is.EqualTo(expectedCategory.CreatedBy));
                Assert.That(result?.CreatedDate, Is.EqualTo(expectedCategory.CreatedDate));
            });
        }

        [Test]
        public async Task GetByIdAsync_WhenCategoryDoesNotExist_ReturnsNull()
        {
            // Arrange
            CategoryAsyncRepository repository = new CategoryAsyncRepository(Mock.Of<ILogger<CategoryAsyncRepository>>(), this._dbContext);

            // Act
            Category? result = await repository.GetByIdAsync(100);

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
            Mock<DbSet<Category>> mockSet = new Mock<DbSet<Category>>();

            mockDbContext.Setup(x => x.Categories).Returns(mockSet.Object);

            CategoryAsyncRepository repository = new CategoryAsyncRepository(Mock.Of<ILogger<CategoryAsyncRepository>>(), mockDbContext.Object);

            // Act
            Category? result = await repository.GetByIdAsync(1);

            // Assert
            Assert.That(result, Is.Null);
        }

        #endregion

        #region AddAsync Tests

        [Test]
        public async Task AddAsync_WithNewCategory_ReturnsId()
        {
            // Arrange
            CategoryAsyncRepository repository = new CategoryAsyncRepository(Mock.Of<ILogger<CategoryAsyncRepository>>(), this._dbContext);

            // Act
            int result = await repository.AddAsync(this._categoryThree);

            // Assert
            Assert.That(result, Is.EqualTo(3));
        }

        [Test]
        public async Task AddAsync_WithExistingCategory_ReturnsMinusOne()
        {
            // Arrange
            CategoryAsyncRepository repository = new CategoryAsyncRepository(Mock.Of<ILogger<CategoryAsyncRepository>>(), this._dbContext);

            // Act
            int result = await repository.AddAsync(this._categoryOne);

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
            Mock<DbSet<Category>> mockSet = new Mock<DbSet<Category>>();
            Mock<DatabaseFacade> mockDatabase = new Mock<DatabaseFacade>(mockDbContext.Object);

            mockDatabase.Setup(d => d.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(Mock.Of<IDbContextTransaction>());
            mockSet.Setup(m => m.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Test exception"));

            mockDbContext.Setup(x => x.Categories).Returns(mockSet.Object);
            mockDbContext.Setup(x => x.Database).Returns(mockDatabase.Object);

            CategoryAsyncRepository repository = new CategoryAsyncRepository(Mock.Of<ILogger<CategoryAsyncRepository>>(), mockDbContext.Object);

            // Act
            int result = await repository.AddAsync(this._categoryOne);

            // Assert
            Assert.That(result, Is.EqualTo(-1));
        }

        #endregion

        #region UpdateAsync Tests

        [Test]
        public async Task UpdateAsync_WithExistingCategory_ReturnsTrue()
        {
            // Arrange
            CategoryAsyncRepository repository = new CategoryAsyncRepository(Mock.Of<ILogger<CategoryAsyncRepository>>(), this._dbContext);
            this._categoryOne.LastModifiedBy = _userName;
            
            // Act
            bool result = await repository.UpdateAsync(this._categoryOne);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task UpdateAsync_WhenCategoryDoesNotExist_ReturnsFalse()
        {
            // Arrange
            CategoryAsyncRepository repository = new CategoryAsyncRepository(Mock.Of<ILogger<CategoryAsyncRepository>>(), this._dbContext);

            // Act
            bool result = await repository.UpdateAsync(this._categoryThree);

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
            Mock<DbSet<Category>> mockSet = new Mock<DbSet<Category>>();
            Mock<DatabaseFacade> mockDatabase = new Mock<DatabaseFacade>(mockDbContext.Object);

            mockDbContext.Setup(x => x.Categories).Returns(mockSet.Object);
            mockDbContext.Setup(x => x.Database).Returns(mockDatabase.Object);

            mockDatabase.Setup(d => d.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(Mock.Of<IDbContextTransaction>());

            CategoryAsyncRepository repository = new CategoryAsyncRepository(Mock.Of<ILogger<CategoryAsyncRepository>>(), mockDbContext.Object);

            // Act
            bool result = await repository.UpdateAsync(this._categoryOne);

            // Assert
            Assert.That(result, Is.False);
        }

        #endregion

        #region DeleteAsync Tests

        [Test]
        public async Task DeleteAsync_WhenCategoryExists_ReturnsTrue()
        {
            // Arrange
            CategoryAsyncRepository repository = new CategoryAsyncRepository(Mock.Of<ILogger<CategoryAsyncRepository>>(), this._dbContext);

            // Act
            bool result = await repository.DeleteAsync(this._categoryOne);

            // Assert
            Assert.That(result, Is.True);
        }
        
        [Test]
        public async Task DeleteAsync_WhenCategoryDoesNotExist_ReturnsFalse()
        {
            // Arrange
            CategoryAsyncRepository repository = new CategoryAsyncRepository(Mock.Of<ILogger<CategoryAsyncRepository>>(), this._dbContext);

            // Act
            bool result = await repository.DeleteAsync(this._categoryThree);

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
            Mock<DbSet<Category>> mockSet = new Mock<DbSet<Category>>();
            Mock<DatabaseFacade> mockDatabase = new Mock<DatabaseFacade>(mockDbContext.Object);

            mockDbContext.Setup(x => x.Categories).Returns(mockSet.Object);
            mockDbContext.Setup(x => x.Database).Returns(mockDatabase.Object);

            mockDatabase.Setup(d => d.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(Mock.Of<IDbContextTransaction>());
            mockSet.Setup(m => m.Remove(It.IsAny<Category>())).Throws(new Exception("Test exception"));

            CategoryAsyncRepository repository = new CategoryAsyncRepository(Mock.Of<ILogger<CategoryAsyncRepository>>(), mockDbContext.Object);

            // Act
            bool result = await repository.DeleteAsync(this._categoryOne);

            // Assert
            Assert.That(result, Is.False);
        }
        
        #endregion
        
        #region ListAllAsync Tests
        
        [Test]
        public async Task ListAllAsync_WhenCategoriesExist_ReturnsAllCategories()
        {
            // Arrange
            CategoryAsyncRepository repository = new CategoryAsyncRepository(Mock.Of<ILogger<CategoryAsyncRepository>>(), this._dbContext);

            // Act
            IEnumerable<Category> result = await repository.ListAllAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));
        }
        
        [Test]
        public async Task ListAllAsync_WhenNoCategoriesExist_ReturnsEmptyList()
        {
            // Arrange
            DbContextOptions<EcommercePersistenceDbContext> options =
                new DbContextOptionsBuilder<EcommercePersistenceDbContext>()
                    .UseInMemoryDatabase(databaseName: "Ecommerce")
                    .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                    .Options;
            
            Mock<EcommercePersistenceDbContext> mockDbContext = new Mock<EcommercePersistenceDbContext>(options);
            CategoryAsyncRepository repository = new CategoryAsyncRepository(Mock.Of<ILogger<CategoryAsyncRepository>>(), mockDbContext.Object);

            // Act
            IEnumerable<Category> result = await repository.ListAllAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(0));
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
            Mock<DbSet<Category>> mockSet = new Mock<DbSet<Category>>();
            
            mockDbContext.Setup(x => x.Categories).Returns(mockSet.Object);
            
            CategoryAsyncRepository repository = new CategoryAsyncRepository(Mock.Of<ILogger<CategoryAsyncRepository>>(), mockDbContext.Object);

            // Act
            IEnumerable<Category> result = await repository.ListAllAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(0));
        }
        
        #endregion

        #region IsNameUnique Tests

        [Test]
        public async Task IsNameUnique_WithUniqueName_ReturnsTrue()
        {
            // Arrange
            CategoryAsyncRepository repository = new CategoryAsyncRepository(Mock.Of<ILogger<CategoryAsyncRepository>>(), this._dbContext);

            // Act
            bool result = await repository.IsNameUnique("Category 3");

            // Assert
            Assert.That(result, Is.True);
        }
        
        [Test]
        public async Task IsNameUnique_WithExistingName_ReturnsFalse()
        {
            // Arrange
            CategoryAsyncRepository repository = new CategoryAsyncRepository(Mock.Of<ILogger<CategoryAsyncRepository>>(), this._dbContext);

            // Act
            bool result = await repository.IsNameUnique("Category 1");

            // Assert
            Assert.That(result, Is.False);
        }

        #endregion
    }
}
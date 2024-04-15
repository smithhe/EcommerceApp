using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Ecommerce.Application.Features.Category.Commands.CreateCategory;
using Ecommerce.Application.Features.Category.Commands.DeleteCategory;
using Ecommerce.Application.Features.Category.Commands.UpdateCategory;
using Ecommerce.Application.Features.Category.Queries.GetAllCategories;
using Ecommerce.Application.Features.Category.Queries.GetCategoryById;
using Ecommerce.Application.Profiles;
using Ecommerce.Domain.Constants;
using Ecommerce.Domain.Entities;
using Ecommerce.Persistence.Contracts;
using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Responses.Category;
using Microsoft.Extensions.Logging;
using Moq;

namespace Ecommerce.UnitTests.ApplicationTests
{
    public class CategoryTests
    {
        private const string _userName = "Test User";
        
        private CategoryDto _categoryDto = null!;

        private Category _category = null!;
        
        private Mock<ICategoryAsyncRepository> _categoryRepository = null!;
        private IMapper _mapper = null!;
        
        [SetUp]
        public void Setup()
        {
            this._categoryRepository = new Mock<ICategoryAsyncRepository>();
            
            this._categoryDto = new CategoryDto
            {
                Id = 1,
                Name = "Category 1",
                Summary = "Category 1 Summary"
            };
            
            this._category = new Category
            {
                Id = 1,
                Name = "Category 1",
                Summary = "Category 1 Summary"
            };
            
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            this._mapper = config.CreateMapper();
        }

        #region CreateCategoryCommandHandler Tests

        [Test]
        public async Task CreateCategoryCommandHandler_WithValidRequestAndWithoutErrors_ReturnsSuccessResponse() 
        {
            //Arrange
            CreateCategoryCommand command = new CreateCategoryCommand
            {
                CategoryToCreate = this._categoryDto,
                UserName = _userName
            };
            CreateCategoryCommandHandler handler = new CreateCategoryCommandHandler(Mock.Of<ILogger<CreateCategoryCommandHandler>>() , this._mapper, this._categoryRepository.Object);
            
            this._categoryRepository.Setup(c => c.AddAsync(It.IsAny<Category>())).ReturnsAsync(1);
            this._categoryRepository.Setup(c => c.IsNameUnique(It.IsAny<string>())).ReturnsAsync(true);
            this._categoryRepository.Setup(c => c.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(this._category);
            
            //Act
            CreateCategoryResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Success, Is.True);
                Assert.That(result.Message, Is.EqualTo(CategoryConstants._createSuccessMessage));
                Assert.That(result.ValidationErrors, Is.Empty);
                Assert.That(result.Category, Is.Not.Null);
            });
        }
        
        [Test]
        public async Task CreateCategoryCommandHandler_WithEmptyDto_ReturnsFailedResponse() 
        {
            //Arrange
            CreateCategoryCommand command = new CreateCategoryCommand
            {
                CategoryToCreate = null,
                UserName = _userName
            };
            CreateCategoryCommandHandler handler = new CreateCategoryCommandHandler(Mock.Of<ILogger<CreateCategoryCommandHandler>>() , this._mapper, this._categoryRepository.Object);
            
            //Act
            CreateCategoryResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(CategoryConstants._createErrorMessage));
                Assert.That(result.ValidationErrors, Is.Empty);
                Assert.That(result.Category, Is.Null);
            });
        }
        
        [Test]
        public async Task CreateCategoryCommandHandler_WithEmptyUserName_ReturnsFailedResponse() 
        {
            //Arrange
            CreateCategoryCommand command = new CreateCategoryCommand
            {
                CategoryToCreate = this._categoryDto,
                UserName = string.Empty
            };
            CreateCategoryCommandHandler handler = new CreateCategoryCommandHandler(Mock.Of<ILogger<CreateCategoryCommandHandler>>() , this._mapper, this._categoryRepository.Object);
            
            //Act
            CreateCategoryResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(CategoryConstants._createErrorMessage));
                Assert.That(result.ValidationErrors, Is.Empty);
                Assert.That(result.Category, Is.Null);
            });
        }
        
        [Test]
        public async Task CreateCategoryCommandHandler_WhenNameIsNotUnique_ReturnsFailedResponse() 
        {
            //Arrange
            CreateCategoryCommand command = new CreateCategoryCommand
            {
                CategoryToCreate = this._categoryDto,
                UserName = _userName
            };
            CreateCategoryCommandHandler handler = new CreateCategoryCommandHandler(Mock.Of<ILogger<CreateCategoryCommandHandler>>() , this._mapper, this._categoryRepository.Object);
            
            this._categoryRepository.Setup(c => c.IsNameUnique(It.IsAny<string>())).ReturnsAsync(false);
            
            //Act
            CreateCategoryResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(CategoryConstants._genericValidationErrorMessage));
                Assert.That(result.ValidationErrors, Is.Not.Empty);
                Assert.That(result.ValidationErrors, Has.Count.EqualTo(1));
                Assert.That(result.ValidationErrors[0], Is.EqualTo("Name must be unique"));
                Assert.That(result.Category, Is.Null);
            });
        }
        
        [Test]
        public async Task CreateCategoryCommandHandler_WhenNameIsEmpty_ReturnsFailedResponse() 
        {
            //Arrange
            this._categoryDto.Name = string.Empty;
            CreateCategoryCommand command = new CreateCategoryCommand
            {
                CategoryToCreate = this._categoryDto,
                UserName = _userName
            };
            CreateCategoryCommandHandler handler = new CreateCategoryCommandHandler(Mock.Of<ILogger<CreateCategoryCommandHandler>>() , this._mapper, this._categoryRepository.Object);
            
            this._categoryRepository.Setup(c => c.IsNameUnique(It.IsAny<string>())).ReturnsAsync(true);
            
            //Act
            CreateCategoryResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(CategoryConstants._genericValidationErrorMessage));
                Assert.That(result.ValidationErrors, Is.Not.Empty);
                Assert.That(result.ValidationErrors, Has.Count.EqualTo(1));
                Assert.That(result.ValidationErrors[0], Is.EqualTo("Name cannot not be empty"));
                Assert.That(result.Category, Is.Null);
            });
        }

        [Test]
        public async Task CreateCategoryCommandHandler_WhenNameIsNull_ReturnsFailedResponse() 
        {
            //Arrange
            this._categoryDto.Name = null!;
            CreateCategoryCommand command = new CreateCategoryCommand
            {
                CategoryToCreate = this._categoryDto,
                UserName = _userName
            };
            CreateCategoryCommandHandler handler = new CreateCategoryCommandHandler(Mock.Of<ILogger<CreateCategoryCommandHandler>>() , this._mapper, this._categoryRepository.Object);
            
            this._categoryRepository.Setup(c => c.IsNameUnique(It.IsAny<string>())).ReturnsAsync(true);
            
            //Act
            CreateCategoryResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(CategoryConstants._genericValidationErrorMessage));
                Assert.That(result.ValidationErrors, Is.Not.Empty);
                Assert.That(result.ValidationErrors, Has.Count.EqualTo(2));
                Assert.That(result.ValidationErrors[0], Is.EqualTo("Name cannot not be null"));
                Assert.That(result.ValidationErrors[1], Is.EqualTo("Name cannot not be empty"));
                Assert.That(result.Category, Is.Null);
            });
        }
        
        [Test]
        public async Task CreateCategoryCommandHandler_WhenNameExceedsMaxLength_ReturnsFailedResponse() 
        {
            //Arrange
            this._categoryDto.Name = "This is a very long name that exceeds the maximum length of 50 characters";
            CreateCategoryCommand command = new CreateCategoryCommand
            {
                CategoryToCreate = this._categoryDto,
                UserName = _userName
            };
            CreateCategoryCommandHandler handler = new CreateCategoryCommandHandler(Mock.Of<ILogger<CreateCategoryCommandHandler>>() , this._mapper, this._categoryRepository.Object);
            
            this._categoryRepository.Setup(c => c.IsNameUnique(It.IsAny<string>())).ReturnsAsync(true);
            
            //Act
            CreateCategoryResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(CategoryConstants._genericValidationErrorMessage));
                Assert.That(result.ValidationErrors, Is.Not.Empty);
                Assert.That(result.ValidationErrors, Has.Count.EqualTo(1));
                Assert.That(result.ValidationErrors[0], Is.EqualTo("Name cannot exceed 50 characters"));
                Assert.That(result.Category, Is.Null);
            });
        }
        
        [Test]
        public async Task CreateCategoryCommandHandler_WhenSummaryIsEmpty_ReturnsFailedResponse() 
        {
            //Arrange
            this._categoryDto.Summary = string.Empty;
            CreateCategoryCommand command = new CreateCategoryCommand
            {
                CategoryToCreate = this._categoryDto,
                UserName = _userName
            };
            CreateCategoryCommandHandler handler = new CreateCategoryCommandHandler(Mock.Of<ILogger<CreateCategoryCommandHandler>>() , this._mapper, this._categoryRepository.Object);
            
            this._categoryRepository.Setup(c => c.IsNameUnique(It.IsAny<string>())).ReturnsAsync(true);
            
            //Act
            CreateCategoryResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(CategoryConstants._genericValidationErrorMessage));
                Assert.That(result.ValidationErrors, Is.Not.Empty);
                Assert.That(result.ValidationErrors, Has.Count.EqualTo(1));
                Assert.That(result.ValidationErrors[0], Is.EqualTo("Summary cannot be empty"));
                Assert.That(result.Category, Is.Null);
            });
        }
        
        [Test]
        public async Task CreateCategoryCommandHandler_WhenSummaryIsNull_ReturnsFailedResponse() 
        {
            //Arrange
            this._categoryDto.Summary = null!;
            CreateCategoryCommand command = new CreateCategoryCommand
            {
                CategoryToCreate = this._categoryDto,
                UserName = _userName
            };
            CreateCategoryCommandHandler handler = new CreateCategoryCommandHandler(Mock.Of<ILogger<CreateCategoryCommandHandler>>() , this._mapper, this._categoryRepository.Object);
            
            this._categoryRepository.Setup(c => c.IsNameUnique(It.IsAny<string>())).ReturnsAsync(true);
            
            //Act
            CreateCategoryResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(CategoryConstants._genericValidationErrorMessage));
                Assert.That(result.ValidationErrors, Is.Not.Empty);
                Assert.That(result.ValidationErrors, Has.Count.EqualTo(2));
                Assert.That(result.ValidationErrors[0], Is.EqualTo("Summary cannot not be null"));
                Assert.That(result.ValidationErrors[1], Is.EqualTo("Summary cannot be empty"));
                Assert.That(result.Category, Is.Null);
            });
        }

        [Test]
        public async Task CreateCategoryCommandHandler_WhenSummaryExceedsMaxLength_ReturnsFailedResponse()
        {
            //Arrange
            this._categoryDto.Summary = "aaaa";
            for (int i = 0; i < 200; i++)
            {
                this._categoryDto.Summary += "a";
            }
            
            CreateCategoryCommand command = new CreateCategoryCommand
            {
                CategoryToCreate = this._categoryDto,
                UserName = _userName
            };
            CreateCategoryCommandHandler handler = new CreateCategoryCommandHandler(Mock.Of<ILogger<CreateCategoryCommandHandler>>() , this._mapper, this._categoryRepository.Object);
            
            this._categoryRepository.Setup(c => c.IsNameUnique(It.IsAny<string>())).ReturnsAsync(true);
            
            //Act
            CreateCategoryResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(CategoryConstants._genericValidationErrorMessage));
                Assert.That(result.ValidationErrors, Is.Not.Empty);
                Assert.That(result.ValidationErrors, Has.Count.EqualTo(1));
                Assert.That(result.ValidationErrors[0], Is.EqualTo("Summary cannot exceed 200 characters"));
                Assert.That(result.Category, Is.Null);
            });
        }
        
        [Test]
        public async Task CreateCategoryCommandHandler_WhenSqlFailsToCreateCategory_ReturnsFailedResponse() 
        {
            //Arrange
            CreateCategoryCommand command = new CreateCategoryCommand
            {
                CategoryToCreate = this._categoryDto,
                UserName = _userName
            };
            CreateCategoryCommandHandler handler = new CreateCategoryCommandHandler(Mock.Of<ILogger<CreateCategoryCommandHandler>>() , this._mapper, this._categoryRepository.Object);
            
            this._categoryRepository.Setup(c => c.AddAsync(It.IsAny<Category>())).ReturnsAsync(-1);
            this._categoryRepository.Setup(c => c.IsNameUnique(It.IsAny<string>())).ReturnsAsync(true);
            
            //Act
            CreateCategoryResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(CategoryConstants._createErrorMessage));
                Assert.That(result.ValidationErrors, Is.Empty);
                Assert.That(result.Category, Is.Null);
            });
        }

        #endregion

        #region DeleteCategoryCommandHandler Tests

        [Test]
        public async Task DeleteCategoryCommandHandler_WithValidRequestAndWithoutErrors_ReturnsSuccessResponse() 
        {
            //Arrange
            DeleteCategoryCommand command = new DeleteCategoryCommand
            {
                CategoryToDelete = this._categoryDto
            };
            DeleteCategoryCommandHandler handler = new DeleteCategoryCommandHandler(Mock.Of<ILogger<DeleteCategoryCommandHandler>>(), this._mapper, this._categoryRepository.Object);
            
            this._categoryRepository.Setup(c => c.DeleteAsync(It.IsAny<Category>())).ReturnsAsync(true);
            
            //Act
            DeleteCategoryResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Success, Is.True);
                Assert.That(result.Message, Is.EqualTo(CategoryConstants._deleteSuccessMessage));
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }
        
        [Test]
        public async Task DeleteCategoryCommandHandler_WithEmptyDto_ReturnsFailedResponse() 
        {
            //Arrange
            DeleteCategoryCommand command = new DeleteCategoryCommand
            {
                CategoryToDelete = null
            };
            DeleteCategoryCommandHandler handler = new DeleteCategoryCommandHandler(Mock.Of<ILogger<DeleteCategoryCommandHandler>>(), this._mapper, this._categoryRepository.Object);
            
            //Act
            DeleteCategoryResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(CategoryConstants._deleteErrorMessage));
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }
        
        [Test]
        public async Task DeleteCategoryCommandHandler_WhenSqlFailsToDeleteCategory_ReturnsFailedResponse() 
        {
            //Arrange
            DeleteCategoryCommand command = new DeleteCategoryCommand
            {
                CategoryToDelete = this._categoryDto
            };
            DeleteCategoryCommandHandler handler = new DeleteCategoryCommandHandler(Mock.Of<ILogger<DeleteCategoryCommandHandler>>(), this._mapper, this._categoryRepository.Object);
            
            this._categoryRepository.Setup(c => c.DeleteAsync(It.IsAny<Category>())).ReturnsAsync(false);
            
            //Act
            DeleteCategoryResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(CategoryConstants._deleteErrorMessage));
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }

        #endregion

        #region UpdateCategoryCommandHandler Tests

        [Test]
        public async Task UpdateCategoryCommandHandler_WithValidRequestAndWithoutErrors_ReturnsSuccessResponse() 
        {
            //Arrange
            UpdateCategoryCommand command = new UpdateCategoryCommand
            {
                CategoryToUpdate = this._categoryDto,
                UserName = _userName
            };
            UpdateCategoryCommandHandler handler = new UpdateCategoryCommandHandler(Mock.Of<ILogger<UpdateCategoryCommandHandler>>(), this._mapper, this._categoryRepository.Object);
            
            this._categoryRepository.Setup(c => c.UpdateAsync(It.IsAny<Category>())).ReturnsAsync(true);
            this._categoryRepository.Setup(c => c.IsNameUnique(It.IsAny<string>())).ReturnsAsync(true);
            
            //Act
            UpdateCategoryResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Success, Is.True);
                Assert.That(result.Message, Is.EqualTo(CategoryConstants._updateSuccessMessage));
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }

        [Test]
        public async Task UpdateCategoryCommandHandler_WithEmptyDto_ReturnsFailedResponse() 
        {
            //Arrange
            UpdateCategoryCommand command = new UpdateCategoryCommand
            {
                CategoryToUpdate = null,
                UserName = _userName
            };
            UpdateCategoryCommandHandler handler = new UpdateCategoryCommandHandler(Mock.Of<ILogger<UpdateCategoryCommandHandler>>(), this._mapper, this._categoryRepository.Object);
            
            //Act
            UpdateCategoryResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(CategoryConstants._updateErrorMessage));
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }
        
        [Test]
        public async Task UpdateCategoryCommandHandler_WithEmptyUserName_ReturnsFailedResponse() 
        {
            //Arrange
            UpdateCategoryCommand command = new UpdateCategoryCommand
            {
                CategoryToUpdate = this._categoryDto,
                UserName = string.Empty
            };
            UpdateCategoryCommandHandler handler = new UpdateCategoryCommandHandler(Mock.Of<ILogger<UpdateCategoryCommandHandler>>(), this._mapper, this._categoryRepository.Object);
            
            //Act
            UpdateCategoryResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(CategoryConstants._updateErrorMessage));
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }
        
        [Test]
        public async Task UpdateCategoryCommandHandler_WhenNameIsNotUnique_ReturnsFailedResponse() 
        {
            //Arrange
            UpdateCategoryCommand command = new UpdateCategoryCommand
            {
                CategoryToUpdate = this._categoryDto,
                UserName = _userName
            };
            UpdateCategoryCommandHandler handler = new UpdateCategoryCommandHandler(Mock.Of<ILogger<UpdateCategoryCommandHandler>>(), this._mapper, this._categoryRepository.Object);
            
            this._categoryRepository.Setup(c => c.IsNameUnique(It.IsAny<string>())).ReturnsAsync(false);
            
            //Act
            UpdateCategoryResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(CategoryConstants._genericValidationErrorMessage));
                Assert.That(result.ValidationErrors, Is.Not.Empty);
                Assert.That(result.ValidationErrors, Has.Count.EqualTo(1));
                Assert.That(result.ValidationErrors[0], Is.EqualTo("Name must be unique"));
            });
        }
        
        [Test]
        public async Task UpdateCategoryCommandHandler_WhenNameIsEmpty_ReturnsFailedResponse() 
        {
            //Arrange
            this._categoryDto.Name = string.Empty;
            UpdateCategoryCommand command = new UpdateCategoryCommand
            {
                CategoryToUpdate = this._categoryDto,
                UserName = _userName
            };
            UpdateCategoryCommandHandler handler = new UpdateCategoryCommandHandler(Mock.Of<ILogger<UpdateCategoryCommandHandler>>(), this._mapper, this._categoryRepository.Object);
            
            this._categoryRepository.Setup(c => c.IsNameUnique(It.IsAny<string>())).ReturnsAsync(true);
            
            //Act
            UpdateCategoryResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(CategoryConstants._genericValidationErrorMessage));
                Assert.That(result.ValidationErrors, Is.Not.Empty);
                Assert.That(result.ValidationErrors, Has.Count.EqualTo(1));
                Assert.That(result.ValidationErrors[0], Is.EqualTo("Name cannot not be empty"));
            });
        }
        
        [Test]
        public async Task UpdateCategoryCommandHandler_WhenNameIsNull_ReturnsFailedResponse() 
        {
            //Arrange
            this._categoryDto.Name = null!;
            UpdateCategoryCommand command = new UpdateCategoryCommand
            {
                CategoryToUpdate = this._categoryDto,
                UserName = _userName
            };
            UpdateCategoryCommandHandler handler = new UpdateCategoryCommandHandler(Mock.Of<ILogger<UpdateCategoryCommandHandler>>(), this._mapper, this._categoryRepository.Object);
            
            this._categoryRepository.Setup(c => c.IsNameUnique(It.IsAny<string>())).ReturnsAsync(true);
            
            //Act
            UpdateCategoryResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(CategoryConstants._genericValidationErrorMessage));
                Assert.That(result.ValidationErrors, Is.Not.Empty);
                Assert.That(result.ValidationErrors, Has.Count.EqualTo(2));
                Assert.That(result.ValidationErrors[0], Is.EqualTo("Name cannot not be null"));
                Assert.That(result.ValidationErrors[1], Is.EqualTo("Name cannot not be empty"));
            });
        }
        
        [Test]
        public async Task UpdateCategoryCommandHandler_WhenNameExceedsMaxLength_ReturnsFailedResponse() 
        {
            //Arrange
            this._categoryDto.Name = "This is a very long name that exceeds the maximum length of 50 characters";
            UpdateCategoryCommand command = new UpdateCategoryCommand
            {
                CategoryToUpdate = this._categoryDto,
                UserName = _userName
            };
            UpdateCategoryCommandHandler handler = new UpdateCategoryCommandHandler(Mock.Of<ILogger<UpdateCategoryCommandHandler>>(), this._mapper, this._categoryRepository.Object);
            
            this._categoryRepository.Setup(c => c.IsNameUnique(It.IsAny<string>())).ReturnsAsync(true);
            
            //Act
            UpdateCategoryResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(CategoryConstants._genericValidationErrorMessage));
                Assert.That(result.ValidationErrors, Is.Not.Empty);
                Assert.That(result.ValidationErrors, Has.Count.EqualTo(1));
                Assert.That(result.ValidationErrors[0], Is.EqualTo("Name cannot exceed 50 characters"));
            });
        }
        
        [Test]
        public async Task UpdateCategoryCommandHandler_WhenSummaryIsEmpty_ReturnsFailedResponse() 
        {
            //Arrange
            this._categoryDto.Summary = string.Empty;
            UpdateCategoryCommand command = new UpdateCategoryCommand
            {
                CategoryToUpdate = this._categoryDto,
                UserName = _userName
            };
            UpdateCategoryCommandHandler handler = new UpdateCategoryCommandHandler(Mock.Of<ILogger<UpdateCategoryCommandHandler>>(), this._mapper, this._categoryRepository.Object);
            
            this._categoryRepository.Setup(c => c.IsNameUnique(It.IsAny<string>())).ReturnsAsync(true);
            
            //Act
            UpdateCategoryResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(CategoryConstants._genericValidationErrorMessage));
                Assert.That(result.ValidationErrors, Is.Not.Empty);
                Assert.That(result.ValidationErrors, Has.Count.EqualTo(1));
                Assert.That(result.ValidationErrors[0], Is.EqualTo("Summary cannot be empty"));
            });
        }
        
        [Test]
        public async Task UpdateCategoryCommandHandler_WhenSummaryIsNull_ReturnsFailedResponse() 
        {
            //Arrange
            this._categoryDto.Summary = null!;
            UpdateCategoryCommand command = new UpdateCategoryCommand
            {
                CategoryToUpdate = this._categoryDto,
                UserName = _userName
            };
            UpdateCategoryCommandHandler handler = new UpdateCategoryCommandHandler(Mock.Of<ILogger<UpdateCategoryCommandHandler>>(), this._mapper, this._categoryRepository.Object);
            
            this._categoryRepository.Setup(c => c.IsNameUnique(It.IsAny<string>())).ReturnsAsync(true);
            
            //Act
            UpdateCategoryResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(CategoryConstants._genericValidationErrorMessage));
                Assert.That(result.ValidationErrors, Is.Not.Empty);
                Assert.That(result.ValidationErrors, Has.Count.EqualTo(2));
                Assert.That(result.ValidationErrors[0], Is.EqualTo("Summary cannot not be null"));
                Assert.That(result.ValidationErrors[1], Is.EqualTo("Summary cannot be empty"));
            });
        }
        
        [Test]
        public async Task UpdateCategoryCommandHandler_WhenSummaryExceedsMaxLength_ReturnsFailedResponse() 
        {
            //Arrange
            this._categoryDto.Summary = "aaaa";
            for (int i = 0; i < 200; i++)
            {
                this._categoryDto.Summary += "a";
            }
            
            UpdateCategoryCommand command = new UpdateCategoryCommand
            {
                CategoryToUpdate = this._categoryDto,
                UserName = _userName
            };
            UpdateCategoryCommandHandler handler = new UpdateCategoryCommandHandler(Mock.Of<ILogger<UpdateCategoryCommandHandler>>(), this._mapper, this._categoryRepository.Object);
            
            this._categoryRepository.Setup(c => c.IsNameUnique(It.IsAny<string>())).ReturnsAsync(true);
            
            //Act
            UpdateCategoryResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(CategoryConstants._genericValidationErrorMessage));
                Assert.That(result.ValidationErrors, Is.Not.Empty);
                Assert.That(result.ValidationErrors, Has.Count.EqualTo(1));
                Assert.That(result.ValidationErrors[0], Is.EqualTo("Summary cannot exceed 200 characters"));
            });
        }
        
        [Test]
        public async Task UpdateCategoryCommandHandler_WhenSqlFailsToUpdateCategory_ReturnsFailedResponse() 
        {
            //Arrange
            UpdateCategoryCommand command = new UpdateCategoryCommand
            {
                CategoryToUpdate = this._categoryDto,
                UserName = _userName
            };
            UpdateCategoryCommandHandler handler = new UpdateCategoryCommandHandler(Mock.Of<ILogger<UpdateCategoryCommandHandler>>(), this._mapper, this._categoryRepository.Object);
            
            this._categoryRepository.Setup(c => c.UpdateAsync(It.IsAny<Category>())).ReturnsAsync(false);
            this._categoryRepository.Setup(c => c.IsNameUnique(It.IsAny<string>())).ReturnsAsync(true);
            
            //Act
            UpdateCategoryResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(CategoryConstants._updateErrorMessage));
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }
        
        #endregion

        #region GetAllCategoriesQueryHandler Tests

        [Test]
        public async Task GetAllCategoriesQueryHandler_WithValidRequestAndWithoutErrors_ReturnsSuccessResponse() 
        {
            //Arrange
            GetAllCategoriesQuery query = new GetAllCategoriesQuery();
            GetAllCategoriesQueryHandler handler = new GetAllCategoriesQueryHandler(Mock.Of<ILogger<GetAllCategoriesQueryHandler>>(), this._mapper, this._categoryRepository.Object);
            
            this._categoryRepository.Setup(c => c.ListAllAsync()).ReturnsAsync(new List<Category> { this._category });
            
            //Act
            GetAllCategoriesResponse result = await handler.Handle(query, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Success, Is.True);
                Assert.That(result.Message, Is.EqualTo(CategoryConstants._getAllSuccessMessage));
                Assert.That(result.Categories, Is.Not.Null);
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }
        
        [Test]
        public async Task GetAllCategoriesQueryHandler_WhenNoCategoriesFound_ReturnsFailedResponse() 
        {
            //Arrange
            GetAllCategoriesQuery query = new GetAllCategoriesQuery();
            GetAllCategoriesQueryHandler handler = new GetAllCategoriesQueryHandler(Mock.Of<ILogger<GetAllCategoriesQueryHandler>>(), this._mapper, this._categoryRepository.Object);
            
            this._categoryRepository.Setup(c => c.ListAllAsync()).ReturnsAsync(new List<Category>());
            
            //Act
            GetAllCategoriesResponse result = await handler.Handle(query, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(CategoryConstants._getAllErrorMessage));
                Assert.That(result.Categories, Is.Empty);
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }

        #endregion

        #region GetCategoryByIdQueryHandler Tests

        [Test]
        public async Task GetCategoryByIdQueryHandler_WithValidRequestAndWithoutErrors_ReturnsSuccessResponse() 
        {
            //Arrange
            GetCategoryByIdQuery query = new GetCategoryByIdQuery { Id = 1 };
            GetCategoryByIdQueryHandler handler = new GetCategoryByIdQueryHandler(Mock.Of<ILogger<GetCategoryByIdQueryHandler>>(), this._mapper, this._categoryRepository.Object);
            
            this._categoryRepository.Setup(c => c.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(this._category);
            
            //Act
            GetCategoryByIdResponse result = await handler.Handle(query, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Success, Is.True);
                Assert.That(result.Message, Is.EqualTo(CategoryConstants._getByIdSuccessMessage));
                Assert.That(result.Category, Is.Not.Null);
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }
        
        [Test]
        public async Task GetCategoryByIdQueryHandler_WhenCategoryNotFound_ReturnsFailedResponse() 
        {
            //Arrange
            GetCategoryByIdQuery query = new GetCategoryByIdQuery { Id = 1 };
            GetCategoryByIdQueryHandler handler = new GetCategoryByIdQueryHandler(Mock.Of<ILogger<GetCategoryByIdQueryHandler>>(), this._mapper, this._categoryRepository.Object);
            
            this._categoryRepository.Setup(c => c.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Category?)null);
            
            //Act
            GetCategoryByIdResponse result = await handler.Handle(query, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(CategoryConstants._getByIdErrorMessage));
                Assert.That(result.Category, Is.Null);
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }

        #endregion
    }
}
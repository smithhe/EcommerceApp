using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Application.Features.Category.Commands.CreateCategory;
using Ecommerce.Application.Features.Category.Commands.DeleteCategory;
using Ecommerce.Application.Features.Category.Commands.UpdateCategory;
using Ecommerce.Application.Features.Category.Queries.GetAllCategories;
using Ecommerce.Domain.Constants.Entities;
using Ecommerce.FastEndpoints.Contracts;
using Ecommerce.FastEndpoints.Endpoints.Category;
using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Requests.Category;
using Ecommerce.Shared.Responses.Category;
using FastEndpoints;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace Ecommerce.UnitTests.FastEndpointTests
{
    [TestFixture]
    public class CategoryEndpointTests
    {
        private const string _userName = "testuser";
        
        private CategoryDto _categoryDto = null!;
        
        private Mock<ITokenService> _tokenService = null!;
        private Mock<IMediator> _mediator = null!;
        
        [SetUp]
        public void Setup()
        {
            this._tokenService = new Mock<ITokenService>();
            this._mediator = new Mock<IMediator>();
            
            this._categoryDto = new CategoryDto
            {
                Name = "Test Category",
                Summary = "Test Summary"
            };
        }

        #region CreateCategoryEndpoint Tests

        [Test]
        public async Task CreateCategoryEndpoint_WithValidRequestAndWithoutErrors_ReturnsSuccess() 
        {
            //Arrange
            CreateCategoryApiRequest request = new CreateCategoryApiRequest
            {
                CategoryToCreate = this._categoryDto
            };
            
            CreateCategoryResponse expectedResponse = new CreateCategoryResponse
            {
                Success = true,
                Message = CategoryConstants._createSuccessMessage
            };
            
            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>())).ReturnsAsync(true);
            this._tokenService.Setup(t => t.GetUserNameFromToken(It.IsAny<string>())).Returns(_userName);
            
            this._mediator.Setup(m => m.Send(It.IsAny<CreateCategoryCommand>(), default(CancellationToken)))
                .ReturnsAsync(expectedResponse);
            
            CreateCategoryEndpoint endpoint = Factory.Create<CreateCategoryEndpoint>(Mock.Of<ILogger<CreateCategoryEndpoint>>(), this._mediator.Object, this._tokenService.Object);

            //Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            CreateCategoryResponse result = endpoint.Response;

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.EqualTo(expectedResponse.Success));
                Assert.That(result.Message, Is.EqualTo(expectedResponse.Message));
                Assert.That(result.Category, Is.EqualTo(expectedResponse.Category));
            });
        }
        
        [Test]
        public async Task CreateCategoryEndpoint_WithInvalidToken_ReturnsUnauthorized() 
        {
            //Arrange
            CreateCategoryApiRequest request = new CreateCategoryApiRequest
            {
                CategoryToCreate = this._categoryDto
            };
            
            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>())).ReturnsAsync(false);
            
            CreateCategoryEndpoint endpoint = Factory.Create<CreateCategoryEndpoint>(Mock.Of<ILogger<CreateCategoryEndpoint>>(), this._mediator.Object, this._tokenService.Object);

            //Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            CreateCategoryResponse result = endpoint.Response;

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.Null);
                Assert.That(result.Category, Is.Null);
            });
        }
        
        [Test]
        public async Task CreateCategoryEndpoint_WithException_ReturnsInternalServerError() 
        {
            //Arrange
            CreateCategoryApiRequest request = new CreateCategoryApiRequest
            {
                CategoryToCreate = this._categoryDto
            };
            
            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>())).ReturnsAsync(true);
            this._tokenService.Setup(t => t.GetUserNameFromToken(It.IsAny<string>())).Returns(_userName);
            
            this._mediator.Setup(m => m.Send(It.IsAny<CreateCategoryCommand>(), default(CancellationToken)))
                .ThrowsAsync(new Exception("Test Exception"));
            
            CreateCategoryEndpoint endpoint = Factory.Create<CreateCategoryEndpoint>(Mock.Of<ILogger<CreateCategoryEndpoint>>(), this._mediator.Object, this._tokenService.Object);

            //Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            CreateCategoryResponse result = endpoint.Response;

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo("Unexpected Error Occurred"));
                Assert.That(result.Category, Is.Null);
            });
        }

        #endregion

        #region DeleteCategoryEndpoint Tests

        [Test]
        public async Task DeleteCategoryEndpoint_WithValidRequestAndWithoutErrors_ReturnsSuccess() 
        {
            //Arrange
            DeleteCategoryApiRequest request = new DeleteCategoryApiRequest
            {
                CategoryToDelete = this._categoryDto
            };
            
            DeleteCategoryResponse expectedResponse = new DeleteCategoryResponse
            {
                Success = true,
                Message = CategoryConstants._deleteSuccessMessage
            };
            
            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>())).ReturnsAsync(true);
            this._tokenService.Setup(t => t.GetUserNameFromToken(It.IsAny<string>())).Returns(_userName);
            
            this._mediator.Setup(m => m.Send(It.IsAny<DeleteCategoryCommand>(), default(CancellationToken)))
                .ReturnsAsync(expectedResponse);
            
            DeleteCategoryEndpoint endpoint = Factory.Create<DeleteCategoryEndpoint>(Mock.Of<ILogger<DeleteCategoryEndpoint>>(), this._mediator.Object, this._tokenService.Object);

            //Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            DeleteCategoryResponse result = endpoint.Response;

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.EqualTo(expectedResponse.Success));
                Assert.That(result.Message, Is.EqualTo(expectedResponse.Message));
            });
        }
        
        [Test]
        public async Task DeleteCategoryEndpoint_WithInvalidToken_ReturnsUnauthorized() 
        {
            //Arrange
            DeleteCategoryApiRequest request = new DeleteCategoryApiRequest
            {
                CategoryToDelete = this._categoryDto
            };
            
            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>())).ReturnsAsync(false);
            
            DeleteCategoryEndpoint endpoint = Factory.Create<DeleteCategoryEndpoint>(Mock.Of<ILogger<DeleteCategoryEndpoint>>(), this._mediator.Object, this._tokenService.Object);

            //Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            DeleteCategoryResponse result = endpoint.Response;

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.Null);
            });
        }
        
        [Test]
        public async Task DeleteCategoryEndpoint_WithException_ReturnsInternalServerError() 
        {
            //Arrange
            DeleteCategoryApiRequest request = new DeleteCategoryApiRequest
            {
                CategoryToDelete = this._categoryDto
            };
            
            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>())).ReturnsAsync(true);
            this._tokenService.Setup(t => t.GetUserNameFromToken(It.IsAny<string>())).Returns(_userName);
            
            this._mediator.Setup(m => m.Send(It.IsAny<DeleteCategoryCommand>(), default(CancellationToken)))
                .ThrowsAsync(new Exception("Test Exception"));
            
            DeleteCategoryEndpoint endpoint = Factory.Create<DeleteCategoryEndpoint>(Mock.Of<ILogger<DeleteCategoryEndpoint>>(), this._mediator.Object, this._tokenService.Object);

            //Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            DeleteCategoryResponse result = endpoint.Response;

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo("Unexpected Error Occurred"));
            });
        }

        #endregion

        #region GetAllCategoriesEndpoint Tests

        [Test]
        public async Task GetAllCategoriesEndpoint_WithValidRequestAndWithoutErrors_ReturnsSuccess() 
        {
            //Arrange
            GetAllCategoriesApiRequest request = new GetAllCategoriesApiRequest();
            
            GetAllCategoriesResponse expectedResponse = new GetAllCategoriesResponse
            {
                Success = true,
                Message = CategoryConstants._getAllSuccessMessage,
                Categories = new List<CategoryDto>
                {
                    this._categoryDto
                }
            };
            
            this._mediator.Setup(m => m.Send(It.IsAny<GetAllCategoriesQuery>(), default(CancellationToken)))
                .ReturnsAsync(expectedResponse);
            
            GetAllCategoriesEndpoint endpoint = Factory.Create<GetAllCategoriesEndpoint>(Mock.Of<ILogger<GetAllCategoriesEndpoint>>(), this._mediator.Object);

            //Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            GetAllCategoriesResponse result = endpoint.Response;

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.EqualTo(expectedResponse.Success));
                Assert.That(result.Message, Is.EqualTo(expectedResponse.Message));
                Assert.That(result.Categories, Is.EqualTo(expectedResponse.Categories));
            });
        }
        
        [Test]
        public async Task GetAllCategoriesEndpoint_WithException_ReturnsInternalServerError() 
        {
            //Arrange
            GetAllCategoriesApiRequest request = new GetAllCategoriesApiRequest();
            
            this._mediator.Setup(m => m.Send(It.IsAny<GetAllCategoriesQuery>(), default(CancellationToken)))
                .ThrowsAsync(new Exception("Test Exception"));
            
            GetAllCategoriesEndpoint endpoint = Factory.Create<GetAllCategoriesEndpoint>(Mock.Of<ILogger<GetAllCategoriesEndpoint>>(), this._mediator.Object);

            //Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            GetAllCategoriesResponse result = endpoint.Response;

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo("Unexpected Error Occurred"));
                Assert.That(result.Categories, Is.Empty);
            });
        }

        #endregion

        #region UpdateCategoryEndpoint Tests

        [Test]
        public async Task UpdateCategoryEndpoint_WithValidRequestAndWithoutErrors_ReturnsSuccess() 
        {
            //Arrange
            UpdateCategoryApiRequest request = new UpdateCategoryApiRequest
            {
                CategoryToUpdate = this._categoryDto
            };
            
            UpdateCategoryResponse expectedResponse = new UpdateCategoryResponse
            {
                Success = true,
                Message = CategoryConstants._updateSuccessMessage
            };
            
            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>())).ReturnsAsync(true);
            this._tokenService.Setup(t => t.GetUserNameFromToken(It.IsAny<string>())).Returns(_userName);
            
            this._mediator.Setup(m => m.Send(It.IsAny<UpdateCategoryCommand>(), default(CancellationToken)))
                .ReturnsAsync(expectedResponse);
            
            UpdateCategoryEndpoint endpoint = Factory.Create<UpdateCategoryEndpoint>(Mock.Of<ILogger<UpdateCategoryEndpoint>>(), this._mediator.Object, this._tokenService.Object);

            //Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            UpdateCategoryResponse result = endpoint.Response;

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.EqualTo(expectedResponse.Success));
                Assert.That(result.Message, Is.EqualTo(expectedResponse.Message));
            });
        }
        
        [Test]
        public async Task UpdateCategoryEndpoint_WithInvalidToken_ReturnsUnauthorized() 
        {
            //Arrange
            UpdateCategoryApiRequest request = new UpdateCategoryApiRequest
            {
                CategoryToUpdate = this._categoryDto
            };
            
            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>())).ReturnsAsync(false);
            
            UpdateCategoryEndpoint endpoint = Factory.Create<UpdateCategoryEndpoint>(Mock.Of<ILogger<UpdateCategoryEndpoint>>(), this._mediator.Object, this._tokenService.Object);

            //Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            UpdateCategoryResponse result = endpoint.Response;

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.Null);
            });
        }
        
        [Test]
        public async Task UpdateCategoryEndpoint_WithException_ReturnsInternalServerError() 
        {
            //Arrange
            UpdateCategoryApiRequest request = new UpdateCategoryApiRequest
            {
                CategoryToUpdate = this._categoryDto
            };
            
            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>())).ReturnsAsync(true);
            this._tokenService.Setup(t => t.GetUserNameFromToken(It.IsAny<string>())).Returns(_userName);
            
            this._mediator.Setup(m => m.Send(It.IsAny<UpdateCategoryCommand>(), default(CancellationToken)))
                .ThrowsAsync(new Exception("Test Exception"));
            
            UpdateCategoryEndpoint endpoint = Factory.Create<UpdateCategoryEndpoint>(Mock.Of<ILogger<UpdateCategoryEndpoint>>(), this._mediator.Object, this._tokenService.Object);

            //Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            UpdateCategoryResponse result = endpoint.Response;

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo("Unexpected Error Occurred"));
            });
        }

        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Ecommerce.Application.Features.CartItem.Commands.CreateCartItem;
using Ecommerce.Application.Features.CartItem.Commands.DeleteCartItem;
using Ecommerce.Application.Features.CartItem.Commands.DeleteUserCartItems;
using Ecommerce.Application.Features.CartItem.Commands.UpdateCartItem;
using Ecommerce.Application.Features.CartItem.Queries.GetUserCartItems;
using Ecommerce.Application.Features.Product.Queries.GetProductById;
using Ecommerce.Application.Profiles;
using Ecommerce.Domain.Constants.Entities;
using Ecommerce.Domain.Entities;
using Ecommerce.Persistence.Contracts;
using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Responses.CartItem;
using Ecommerce.Shared.Responses.Product;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace Ecommerce.UnitTests.ApplicationTests
{
    [TestFixture]
    public class CartItemTests
    {
        private static readonly Guid _userId = new Guid("095a987b-b4da-4eb6-a286-19aa3c75be53");
        private const string _userName = "Test User";

        private CartItemDto _cartItemDto = null!;

        private CartItem _cartItem = null!;
        
        private Mock<ICartItemRepository> _cartItemRepository = null!;
        private Mock<IMediator> _mediator = null!;
        private IMapper _mapper = null!;
        
        [SetUp]
        public void Setup()
        {
            this._cartItemRepository = new Mock<ICartItemRepository>();
            this._mediator = new Mock<IMediator>();
            
            this._cartItemDto = new CartItemDto
            {
                Id = 1,
                ProductId = 1,
                Quantity = 1,
                UserId = _userId
            };
            
            this._cartItem = new CartItem
            {
                Id = 1,
                ProductId = 1,
                Quantity = 1,
                UserId = _userId
            };
            
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            this._mapper = config.CreateMapper();
        }

        #region CreateCartItemCommandHandler Tests

        [Test]
        public async Task CreateCartItemCommandHandler_WithValidRequestAndWithoutErrors_ReturnsSuccessResponse()
        {
            //Arrange
            CreateCartItemCommand command = new CreateCartItemCommand
            {
                CartItemToCreate = this._cartItemDto,
                UserName = _userName
            };
            
            CreateCartItemCommandHandler handler = new CreateCartItemCommandHandler(
                Mock.Of<ILogger<CreateCartItemCommandHandler>>(),
                this._mapper,
                this._cartItemRepository.Object,
                this._mediator.Object
            );

            this._cartItemRepository.Setup(c => c.AddAsync(It.IsAny<CartItem>())).ReturnsAsync(1);
            this._cartItemRepository.Setup(c => c.GetByIdAsync(1)).ReturnsAsync(this._cartItem);
            this._mediator.Setup(m => m.Send(It.IsAny<GetProductByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetProductByIdResponse
                {
                    Success = true,
                    Product = new ProductDto
                    {
                        Id = 1,
                        Name = "Test Product",
                        Price = 10.00
                    }
                });

            //Act
            CreateCartItemResponse response = await handler.Handle(command, CancellationToken.None);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(response, Is.Not.Null);
                Assert.That(response.Success, Is.True);
                Assert.That(response.Message, Is.EqualTo(CartItemConstants._createSuccessMessage));
                Assert.That(response.CartItem, Is.Not.Null);
            });
        }
        
        [Test]
        public async Task CreateCartItemCommandHandler_WithNullCartItemDto_ReturnsFailedResponse()
        {
            //Arrange
            CreateCartItemCommand command = new CreateCartItemCommand
            {
                CartItemToCreate = null,
                UserName = _userName
            };
            
            CreateCartItemCommandHandler handler = new CreateCartItemCommandHandler(
                Mock.Of<ILogger<CreateCartItemCommandHandler>>(),
                this._mapper,
                this._cartItemRepository.Object,
                this._mediator.Object
            );

            //Act
            CreateCartItemResponse response = await handler.Handle(command, CancellationToken.None);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(response, Is.Not.Null);
                Assert.That(response.Success, Is.False);
                Assert.That(response.Message, Is.EqualTo(CartItemConstants._createErrorMessage));
                Assert.That(response.CartItem, Is.Null);
            });
        }
        
        [Test]
        public async Task CreateCartItemCommandHandler_WithNullOrEmptyUserName_ReturnsFailedResponse()
        {
            //Arrange
            CreateCartItemCommand command = new CreateCartItemCommand
            {
                CartItemToCreate = this._cartItemDto,
                UserName = string.Empty
            };
            
            CreateCartItemCommandHandler handler = new CreateCartItemCommandHandler(
                Mock.Of<ILogger<CreateCartItemCommandHandler>>(),
                this._mapper,
                this._cartItemRepository.Object,
                this._mediator.Object
            );

            //Act
            CreateCartItemResponse response = await handler.Handle(command, CancellationToken.None);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(response, Is.Not.Null);
                Assert.That(response.Success, Is.False);
                Assert.That(response.Message, Is.EqualTo(CartItemConstants._createErrorMessage));
                Assert.That(response.CartItem, Is.Null);
            });
        }
        
        [Test]
        public async Task CreateCartItemCommandHandler_WithCartItemAlreadyExists_ReturnsFailedResponse()
        {
            //Arrange
            CreateCartItemCommand command = new CreateCartItemCommand
            {
                CartItemToCreate = this._cartItemDto,
                UserName = _userName
            };
            
            CreateCartItemCommandHandler handler = new CreateCartItemCommandHandler(
                Mock.Of<ILogger<CreateCartItemCommandHandler>>(),
                this._mapper,
                this._cartItemRepository.Object,
                this._mediator.Object
            );
            
            this._cartItemRepository.Setup(c => c.CartItemExistsForUser(It.IsAny<Guid>(), It.IsAny<int>())).ReturnsAsync(true);

            this._mediator.Setup(m => m.Send(It.IsAny<GetProductByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetProductByIdResponse
                {
                    Success = true,
                    Product = new ProductDto
                    {
                        Id = 1,
                        Name = "Test Product",
                        Price = 10.00
                    }
                });
            
            //Act
            CreateCartItemResponse response = await handler.Handle(command, CancellationToken.None);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(response, Is.Not.Null);
                Assert.That(response.Success, Is.False);
                Assert.That(response.Message, Is.EqualTo(CartItemConstants._genericValidationErrorMessage));
                Assert.That(response.CartItem, Is.Null);
                Assert.That(response.ValidationErrors, Is.Not.Empty);
                Assert.That(response.ValidationErrors[0], Is.EqualTo("Cart Item Already Exists"));
            });
        }
        
        [Test]
        public async Task CreateCartItemCommandHandler_WithProductNotFound_ReturnsFailedResponse()
        {
            //Arrange
            CreateCartItemCommand command = new CreateCartItemCommand
            {
                CartItemToCreate = this._cartItemDto,
                UserName = _userName
            };
            
            CreateCartItemCommandHandler handler = new CreateCartItemCommandHandler(
                Mock.Of<ILogger<CreateCartItemCommandHandler>>(),
                this._mapper,
                this._cartItemRepository.Object,
                this._mediator.Object
            );

            this._mediator.Setup(m => m.Send(It.IsAny<GetProductByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetProductByIdResponse
                {
                    Success = false,
                    Message = ProductConstants._getProductByIdErrorMessage,
                    Product = null
                });

            //Act
            CreateCartItemResponse response = await handler.Handle(command, CancellationToken.None);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(response, Is.Not.Null);
                Assert.That(response.Success, Is.False);
                Assert.That(response.Message, Is.EqualTo(CartItemConstants._genericValidationErrorMessage));
                Assert.That(response.CartItem, Is.Null);
                Assert.That(response.ValidationErrors, Is.Not.Empty);
                Assert.That(response.ValidationErrors[0], Is.EqualTo("Product must exist"));
            });
        }
        
        [Test]
        public async Task CreateCartItemCommandHandler_WithQuantityLessThanOne_ReturnsFailedResponse()
        {
            //Arrange
            CreateCartItemCommand command = new CreateCartItemCommand
            {
                CartItemToCreate = new CartItemDto
                {
                    Id = 1,
                    ProductId = 1,
                    Quantity = 0,
                    UserId = _userId
                },
                UserName = _userName
            };
            
            CreateCartItemCommandHandler handler = new CreateCartItemCommandHandler(
                Mock.Of<ILogger<CreateCartItemCommandHandler>>(),
                this._mapper,
                this._cartItemRepository.Object,
                this._mediator.Object
            );

            this._cartItemRepository.Setup(c => c.CartItemExistsForUser(It.IsAny<Guid>(), It.IsAny<int>())).ReturnsAsync(false);
            
            this._mediator.Setup(m => m.Send(It.IsAny<GetProductByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetProductByIdResponse
                {
                    Success = true,
                    Product = new ProductDto
                    {
                        Id = 1,
                        Name = "Test Product",
                        Price = 10.00
                    }
                });

            //Act
            CreateCartItemResponse response = await handler.Handle(command, CancellationToken.None);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(response, Is.Not.Null);
                Assert.That(response.Success, Is.False);
                Assert.That(response.Message, Is.EqualTo(CartItemConstants._genericValidationErrorMessage));
                Assert.That(response.CartItem, Is.Null);
                Assert.That(response.ValidationErrors, Is.Not.Empty);
                Assert.That(response.ValidationErrors[0], Is.EqualTo("Quantity Must Be Greater Than 0"));
            });
        }

        [Test]
        public async Task CreateCartItemCommandHandler_WhenSqlFailsToCreate_ReturnsFailedResponse()
        {
            //Arrange
            CreateCartItemCommand command = new CreateCartItemCommand
            {
                CartItemToCreate = this._cartItemDto,
                UserName = _userName
            };
            
            CreateCartItemCommandHandler handler = new CreateCartItemCommandHandler(
                Mock.Of<ILogger<CreateCartItemCommandHandler>>(),
                this._mapper,
                this._cartItemRepository.Object,
                this._mediator.Object
            );

            this._cartItemRepository.Setup(c => c.AddAsync(It.IsAny<CartItem>())).ReturnsAsync(-1);
            this._mediator.Setup(m => m.Send(It.IsAny<GetProductByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetProductByIdResponse
                {
                    Success = true,
                    Product = new ProductDto
                    {
                        Id = 1,
                        Name = "Test Product",
                        Price = 10.00
                    }
                });
            
            //Act
            CreateCartItemResponse response = await handler.Handle(command, CancellationToken.None);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(response, Is.Not.Null);
                Assert.That(response.Success, Is.False);
                Assert.That(response.Message, Is.EqualTo(CartItemConstants._createErrorMessage));
                Assert.That(response.CartItem, Is.Null);
            });
        }
        
        #endregion

        #region DeleteCartItemCommandHandler Tests

        [Test]
        public async Task DeleteCartItemCommandHandler_WithValidRequestAndWithoutErrors_ReturnsSuccessResponse()
        {
            //Arrange
            DeleteCartItemCommand command = new DeleteCartItemCommand
            {
                CartItemToDelete = this._cartItemDto
            };
            
            DeleteCartItemCommandHandler handler = new DeleteCartItemCommandHandler(
                Mock.Of<ILogger<DeleteCartItemCommandHandler>>(),
                this._mapper,
                this._cartItemRepository.Object
            );

            this._cartItemRepository.Setup(c => c.DeleteAsync(It.IsAny<CartItem>())).ReturnsAsync(true);

            //Act
            DeleteCartItemResponse response = await handler.Handle(command, CancellationToken.None);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(response, Is.Not.Null);
                Assert.That(response.Success, Is.True);
                Assert.That(response.Message, Is.EqualTo(CartItemConstants._deleteSingleItemSuccessMessage));
            });
        }
        
        [Test]
        public async Task DeleteCartItemCommandHandler_WithNullCartItemDto_ReturnsFailedResponse()
        {
            //Arrange
            DeleteCartItemCommand command = new DeleteCartItemCommand
            {
                CartItemToDelete = null
            };
            
            DeleteCartItemCommandHandler handler = new DeleteCartItemCommandHandler(
                Mock.Of<ILogger<DeleteCartItemCommandHandler>>(),
                this._mapper,
                this._cartItemRepository.Object
            );

            //Act
            DeleteCartItemResponse response = await handler.Handle(command, CancellationToken.None);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(response, Is.Not.Null);
                Assert.That(response.Success, Is.False);
                Assert.That(response.Message, Is.EqualTo(CartItemConstants._deleteSingleItemErrorMessage));
            });
        }
        
        [Test]
        public async Task DeleteCartItemCommandHandler_WhenSqlFailsToDelete_ReturnsFailedResponse()
        {
            //Arrange
            DeleteCartItemCommand command = new DeleteCartItemCommand
            {
                CartItemToDelete = this._cartItemDto
            };
            
            DeleteCartItemCommandHandler handler = new DeleteCartItemCommandHandler(
                Mock.Of<ILogger<DeleteCartItemCommandHandler>>(),
                this._mapper,
                this._cartItemRepository.Object
            );

            this._cartItemRepository.Setup(c => c.DeleteAsync(It.IsAny<CartItem>())).ReturnsAsync(false);
            
            //Act
            DeleteCartItemResponse response = await handler.Handle(command, CancellationToken.None);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(response, Is.Not.Null);
                Assert.That(response.Success, Is.False);
                Assert.That(response.Message, Is.EqualTo(CartItemConstants._deleteSingleItemErrorMessage));
            });
        }

        #endregion

        #region DeleteUserCartItemsCommandHandler Tests

        [Test]
        public async Task DeleteUserCartItemsCommandHandler_WithValidRequestAndWithoutErrors_ReturnsSuccessResponse()
        {
            //Arrange
            DeleteUserCartItemsCommand command = new DeleteUserCartItemsCommand
            {
                UserId = _userId
            };
            
            DeleteUserCartItemsCommandHandler handler = new DeleteUserCartItemsCommandHandler(
                Mock.Of<ILogger<DeleteUserCartItemsCommandHandler>>(),
                this._cartItemRepository.Object
            );

            this._cartItemRepository.Setup(c => c.RemoveUserCartItems(It.IsAny<Guid>())).ReturnsAsync(true);

            //Act
            DeleteUserCartItemsResponse response = await handler.Handle(command, CancellationToken.None);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(response, Is.Not.Null);
                Assert.That(response.Success, Is.True);
                Assert.That(response.Message, Is.EqualTo(CartItemConstants._deleteAllItemsSuccessMessage));
            });
        }

        [Test]
        public async Task DeleteUserCartItemsCommandHandler_WithEmptyUserId_ReturnsFailedResponse()
        {
            //Arrange
            DeleteUserCartItemsCommand command = new DeleteUserCartItemsCommand
            {
                UserId = Guid.Empty
            };
            
            DeleteUserCartItemsCommandHandler handler = new DeleteUserCartItemsCommandHandler(
                Mock.Of<ILogger<DeleteUserCartItemsCommandHandler>>(),
                this._cartItemRepository.Object
            );

            //Act
            DeleteUserCartItemsResponse response = await handler.Handle(command, CancellationToken.None);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(response, Is.Not.Null);
                Assert.That(response.Success, Is.False);
                Assert.That(response.Message, Is.EqualTo(CartItemConstants._deleteAllItemsErrorMessage));
            });
        }
        
        [Test]
        public async Task DeleteUserCartItemsCommandHandler_WhenSqlFailsToDelete_ReturnsFailedResponse()
        {
            //Arrange
            DeleteUserCartItemsCommand command = new DeleteUserCartItemsCommand
            {
                UserId = _userId
            };
            
            DeleteUserCartItemsCommandHandler handler = new DeleteUserCartItemsCommandHandler(
                Mock.Of<ILogger<DeleteUserCartItemsCommandHandler>>(),
                this._cartItemRepository.Object
            );

            this._cartItemRepository.Setup(c => c.RemoveUserCartItems(It.IsAny<Guid>())).ReturnsAsync(false);
            
            //Act
            DeleteUserCartItemsResponse response = await handler.Handle(command, CancellationToken.None);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(response, Is.Not.Null);
                Assert.That(response.Success, Is.False);
                Assert.That(response.Message, Is.EqualTo(CartItemConstants._deleteAllItemsErrorMessage));
            });
        }
        
        #endregion

        #region UpdateCartItemCommandHandler Tests

        [Test]
        public async Task UpdateCartItemCommandHandler_WithValidRequestAndWithoutErrors_ReturnsSuccessResponse()
        {
            //Arrange
            UpdateCartItemCommand command = new UpdateCartItemCommand
            {
                CartItemToUpdate = this._cartItemDto,
                UserName = _userName
            };
            
            UpdateCartItemCommandHandler handler = new UpdateCartItemCommandHandler(
                Mock.Of<ILogger<UpdateCartItemCommandHandler>>(),
                this._mapper,
                this._cartItemRepository.Object,
                this._mediator.Object
            );

            this._cartItemRepository.Setup(c => c.UpdateAsync(It.IsAny<CartItem>())).ReturnsAsync(true);
            this._cartItemRepository.Setup(c => c.CartItemExistsForUser(_userId, 1)).ReturnsAsync(true);
            this._mediator.Setup(m => m.Send(It.IsAny<GetProductByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetProductByIdResponse
                {
                    Success = true,
                    Product = new ProductDto
                    {
                        Id = 1,
                        Name = "Test Product",
                        Price = 10.00
                    }
                });

            //Act
            UpdateCartItemResponse response = await handler.Handle(command, CancellationToken.None);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(response, Is.Not.Null);
                Assert.That(response.Success, Is.True);
                Assert.That(response.Message, Is.EqualTo(CartItemConstants._updateSuccessMessage));
            });
        }

        [Test]
        public async Task UpdateCartItemCommandHandler_WithNullCartItemDto_ReturnsFailedResponse()
        {
            //Arrange
            UpdateCartItemCommand command = new UpdateCartItemCommand
            {
                CartItemToUpdate = null,
                UserName = _userName
            };
            
            UpdateCartItemCommandHandler handler = new UpdateCartItemCommandHandler(
                Mock.Of<ILogger<UpdateCartItemCommandHandler>>(),
                this._mapper,
                this._cartItemRepository.Object,
                this._mediator.Object
            );

            //Act
            UpdateCartItemResponse response = await handler.Handle(command, CancellationToken.None);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(response, Is.Not.Null);
                Assert.That(response.Success, Is.False);
                Assert.That(response.Message, Is.EqualTo(CartItemConstants._updateErrorMessage));
            });
        }
        
        [Test]
        public async Task UpdateCartItemCommandHandler_WithNullOrEmptyUserName_ReturnsFailedResponse()
        {
            //Arrange
            UpdateCartItemCommand command = new UpdateCartItemCommand
            {
                CartItemToUpdate = this._cartItemDto,
                UserName = string.Empty
            };
            
            UpdateCartItemCommandHandler handler = new UpdateCartItemCommandHandler(
                Mock.Of<ILogger<UpdateCartItemCommandHandler>>(),
                this._mapper,
                this._cartItemRepository.Object,
                this._mediator.Object
            );

            //Act
            UpdateCartItemResponse response = await handler.Handle(command, CancellationToken.None);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(response, Is.Not.Null);
                Assert.That(response.Success, Is.False);
                Assert.That(response.Message, Is.EqualTo(CartItemConstants._updateErrorMessage));
            });
        }
        
        [Test]
        public async Task UpdateCartItemCommandHandler_WithCartItemNotFound_ReturnsFailedResponse()
        {
            //Arrange
            UpdateCartItemCommand command = new UpdateCartItemCommand
            {
                CartItemToUpdate = this._cartItemDto,
                UserName = _userName
            };
            
            UpdateCartItemCommandHandler handler = new UpdateCartItemCommandHandler(
                Mock.Of<ILogger<UpdateCartItemCommandHandler>>(),
                this._mapper,
                this._cartItemRepository.Object,
                this._mediator.Object
            );
            
            this._cartItemRepository.Setup(c => c.CartItemExistsForUser(_userId, 1)).ReturnsAsync(false);

            this._mediator.Setup(m => m.Send(It.IsAny<GetProductByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetProductByIdResponse
                {
                    Success = true,
                    Product = new ProductDto
                    {
                        Id = 1,
                        Name = "Test Product",
                        Price = 10.00
                    }
                });
            
            //Act
            UpdateCartItemResponse response = await handler.Handle(command, CancellationToken.None);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(response, Is.Not.Null);
                Assert.That(response.Success, Is.False);
                Assert.That(response.Message, Is.EqualTo(CartItemConstants._genericValidationErrorMessage));
                Assert.That(response.ValidationErrors, Is.Not.Empty);
                Assert.That(response.ValidationErrors[0], Is.EqualTo("Cart Item must exist"));
            });
        }
        
        [Test]
        public async Task UpdateCartItemCommandHandler_WithProductNotFound_ReturnsFailedResponse()
        {
            //Arrange
            UpdateCartItemCommand command = new UpdateCartItemCommand
            {
                CartItemToUpdate = this._cartItemDto,
                UserName = _userName
            };
            
            UpdateCartItemCommandHandler handler = new UpdateCartItemCommandHandler(
                Mock.Of<ILogger<UpdateCartItemCommandHandler>>(),
                this._mapper,
                this._cartItemRepository.Object,
                this._mediator.Object
            );

            this._cartItemRepository.Setup(c => c.CartItemExistsForUser(_userId, 1)).ReturnsAsync(true);

            this._mediator.Setup(m => m.Send(It.IsAny<GetProductByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetProductByIdResponse
                {
                    Success = false,
                    Message = ProductConstants._getProductByIdErrorMessage,
                    Product = null
                });

            //Act
            UpdateCartItemResponse response = await handler.Handle(command, CancellationToken.None);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(response, Is.Not.Null);
                Assert.That(response.Success, Is.False);
                Assert.That(response.Message, Is.EqualTo(CartItemConstants._genericValidationErrorMessage));
                Assert.That(response.ValidationErrors, Is.Not.Empty);
                Assert.That(response.ValidationErrors[0], Is.EqualTo("Product must exist"));
            });
        }
        
        [Test]
        public async Task UpdateCartItemCommandHandler_WithQuantityLessThanOne_ReturnsFailedResponse()
        {
            //Arrange
            UpdateCartItemCommand command = new UpdateCartItemCommand
            {
                CartItemToUpdate = new CartItemDto
                {
                    Id = 1,
                    ProductId = 1,
                    Quantity = 0,
                    UserId = _userId
                },
                UserName = _userName
            };
            
            UpdateCartItemCommandHandler handler = new UpdateCartItemCommandHandler(
                Mock.Of<ILogger<UpdateCartItemCommandHandler>>(),
                this._mapper,
                this._cartItemRepository.Object,
                this._mediator.Object
            );

            this._cartItemRepository.Setup(c => c.CartItemExistsForUser(_userId, 1)).ReturnsAsync(true);
            
            this._mediator.Setup(m => m.Send(It.IsAny<GetProductByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetProductByIdResponse
                {
                    Success = true,
                    Product = new ProductDto
                    {
                        Id = 1,
                        Name = "Test Product",
                        Price = 10.00
                    }
                });

            //Act
            UpdateCartItemResponse response = await handler.Handle(command, CancellationToken.None);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(response, Is.Not.Null);
                Assert.That(response.Success, Is.False);
                Assert.That(response.Message, Is.EqualTo(CartItemConstants._genericValidationErrorMessage));
                Assert.That(response.ValidationErrors, Is.Not.Empty);
                Assert.That(response.ValidationErrors[0], Is.EqualTo("Quantity Must Be Greater Than 0"));
            });
        }
        
        [Test]
        public async Task UpdateCartItemCommandHandler_WhenSqlFailsToUpdate_ReturnsFailedResponse()
        {
            //Arrange
            UpdateCartItemCommand command = new UpdateCartItemCommand
            {
                CartItemToUpdate = this._cartItemDto,
                UserName = _userName
            };
            
            UpdateCartItemCommandHandler handler = new UpdateCartItemCommandHandler(
                Mock.Of<ILogger<UpdateCartItemCommandHandler>>(),
                this._mapper,
                this._cartItemRepository.Object,
                this._mediator.Object
            );

            this._cartItemRepository.Setup(c => c.UpdateAsync(It.IsAny<CartItem>())).ReturnsAsync(false);
            this._cartItemRepository.Setup(c => c.CartItemExistsForUser(_userId, 1)).ReturnsAsync(true);
            this._mediator.Setup(m => m.Send(It.IsAny<GetProductByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetProductByIdResponse
                {
                    Success = true,
                    Product = new ProductDto
                    {
                        Id = 1,
                        Name = "Test Product",
                        Price = 10.00
                    }
                });
            
            //Act
            UpdateCartItemResponse response = await handler.Handle(command, CancellationToken.None);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(response, Is.Not.Null);
                Assert.That(response.Success, Is.False);
                Assert.That(response.Message, Is.EqualTo(CartItemConstants._updateErrorMessage));
            });
        }
        
        #endregion

        #region GetUserCartItemsQueryHandler Tests

        [Test]
        public async Task GetUserCartItemsQueryHandler_WithValidRequestAndWithoutErrors_ReturnsSuccessResponse()
        {
            //Arrange
            GetUserCartItemsQuery query = new GetUserCartItemsQuery
            {
                UserId = _userId
            };
            
            GetUserCartItemsQueryHandler handler = new GetUserCartItemsQueryHandler(
                Mock.Of<ILogger<GetUserCartItemsQueryHandler>>(),
                this._mapper,
                this._cartItemRepository.Object
            );

            this._cartItemRepository.Setup(c => c.ListAllAsync(It.IsAny<Guid>())).ReturnsAsync(new List<CartItem>
            {
                this._cartItem
            });

            //Act
            GetUserCartItemsResponse response = await handler.Handle(query, CancellationToken.None);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(response, Is.Not.Null);
                Assert.That(response.Success, Is.True);
                Assert.That(response.Message, Is.EqualTo(CartItemConstants._getAllItemsSuccessMessage));
                Assert.That(response.CartItems, Is.Not.Empty);
            });
        }

        [Test]
        public async Task GetUserCartItemsQueryHandler_WithEmptyUserId_ReturnsFailedResponse()
        {
            //Arrange
            GetUserCartItemsQuery query = new GetUserCartItemsQuery
            {
                UserId = Guid.Empty
            };
            
            GetUserCartItemsQueryHandler handler = new GetUserCartItemsQueryHandler(
                Mock.Of<ILogger<GetUserCartItemsQueryHandler>>(),
                this._mapper,
                this._cartItemRepository.Object
            );

            //Act
            GetUserCartItemsResponse response = await handler.Handle(query, CancellationToken.None);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(response, Is.Not.Null);
                Assert.That(response.Success, Is.False);
                Assert.That(response.Message, Is.EqualTo(CartItemConstants._getAllItemsErrorMessage));
                Assert.That(response.CartItems, Is.Empty);
            });
        }
        
        [Test]
        public async Task GetUserCartItemsQueryHandler_WhenSqlReturnsAnEmptyList_ReturnsFailedResponse()
        {
            //Arrange
            GetUserCartItemsQuery query = new GetUserCartItemsQuery
            {
                UserId = _userId
            };
            
            GetUserCartItemsQueryHandler handler = new GetUserCartItemsQueryHandler(
                Mock.Of<ILogger<GetUserCartItemsQueryHandler>>(),
                this._mapper,
                this._cartItemRepository.Object
            );

            this._cartItemRepository.Setup(c => c.ListAllAsync(It.IsAny<Guid>())).ReturnsAsync(new List<CartItem>());

            //Act
            GetUserCartItemsResponse response = await handler.Handle(query, CancellationToken.None);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(response, Is.Not.Null);
                Assert.That(response.Success, Is.False);
                Assert.That(response.Message, Is.EqualTo(CartItemConstants._getAllItemsErrorMessage));
                Assert.That(response.CartItems, Is.Empty);
            });
        }
        
        #endregion
    }
}
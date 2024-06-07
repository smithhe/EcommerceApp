using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Ecommerce.Application.Features.Order.Commands.AddPayPalRequestId;
using Ecommerce.Application.Features.Order.Commands.CreateOrder;
using Ecommerce.Application.Features.Order.Commands.DeleteOrder;
using Ecommerce.Application.Features.Order.Commands.UpdateOrder;
using Ecommerce.Application.Features.Order.Queries.GetAllOrdersByUserId;
using Ecommerce.Application.Features.Order.Queries.GetOrderAfterSuccessfulCheckout;
using Ecommerce.Application.Features.Order.Queries.GetOrderById;
using Ecommerce.Application.Features.OrderItem.Commands.CreateOrderItem;
using Ecommerce.Application.Features.OrderItem.Queries.GetAllOrderItemsByOrderId;
using Ecommerce.Application.Features.Product.Queries.GetProductById;
using Ecommerce.Application.Profiles;
using Ecommerce.Domain.Constants.Entities;
using Ecommerce.Domain.Entities;
using Ecommerce.Persistence.Contracts;
using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Enums;
using Ecommerce.Shared.Responses.Order;
using Ecommerce.Shared.Responses.OrderItem;
using Ecommerce.Shared.Responses.Product;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace Ecommerce.UnitTests.ApplicationTests
{
    [TestFixture]
    public class OrderTests
    {
        private static readonly Guid _userId = new Guid("095a987b-b4da-4eb6-a286-19aa3c75be53");
        private const string _userName = "Test User";

        private Order _order = null!;
        private OrderDto _orderDto = null!;
        
        private Mock<IOrderAsyncRepository> _orderRepository = null!; 
        private Mock<IMediator> _mediator = null!;
        private IMapper _mapper = null!;
        
        [SetUp]
        public void Setup()
        {
            this._orderRepository = new Mock<IOrderAsyncRepository>();
            this._mediator = new Mock<IMediator>();

            this._order = new Order
            {
                UserId = _userId,
                Status = OrderStatus.Created,
                Total = 100,
                PayPalRequestId = Guid.Empty
            };
            
            this._orderDto = new OrderDto
            {
                UserId = _userId,
                Status = OrderStatus.Created,
                Total = 100,
                PayPalRequestId = Guid.Empty
            };
         
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            this._mapper = config.CreateMapper();
        }

        #region AddPayPalRequestIdCommandHandler Tests

        [Test]
        public async Task AddPayPalRequestIdCommandHandler_ValidRequestAndWithoutErrors_ReturnsTrue()
        {
            //Arrange
            this._mediator.Setup(m => m.Send(It.IsAny<GetOrderByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetOrderByIdResponse
                {
                    Success = true,
                    Order = this._orderDto
                });
            
            this._mediator.Setup(m => m.Send(It.IsAny<UpdateOrderCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new UpdateOrderResponse()
                {
                    Success = true,
                    Message = OrderConstants._updateSuccessMessage
                });
            
            AddPayPalRequestIdCommand command = new AddPayPalRequestIdCommand
            {
                OrderId = 1,
                PayPalRequestId = Guid.NewGuid()
            };
            AddPayPalRequestIdCommandHandler handler = new AddPayPalRequestIdCommandHandler(Mock.Of<ILogger<AddPayPalRequestIdCommandHandler>>(), this._mediator.Object);
            
            //Act
            bool result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.That(result, Is.True);
        }
        
        [Test]
        public async Task AddPayPalRequestIdCommandHandler_InvalidOrderId_ReturnsFalse()
        {
            //Arrange
            this._mediator.Setup(m => m.Send(It.IsAny<GetOrderByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetOrderByIdResponse
                {
                    Success = false,
                    Order = null
                });
            
            AddPayPalRequestIdCommand command = new AddPayPalRequestIdCommand
            {
                OrderId = 1,
                PayPalRequestId = Guid.NewGuid()
            };
            AddPayPalRequestIdCommandHandler handler = new AddPayPalRequestIdCommandHandler(Mock.Of<ILogger<AddPayPalRequestIdCommandHandler>>(), this._mediator.Object);
            
            //Act
            bool result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.That(result, Is.False);
        }
        
        [Test]
        public async Task AddPayPalRequestIdCommandHandler_UpdateFails_ReturnsFalse()
        {
            //Arrange
            this._mediator.Setup(m => m.Send(It.IsAny<GetOrderByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetOrderByIdResponse
                {
                    Success = true,
                    Order = this._orderDto
                });
            
            this._mediator.Setup(m => m.Send(It.IsAny<UpdateOrderCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new UpdateOrderResponse()
                {
                    Success = false,
                    Message = OrderConstants._updateErrorMessage
                });
            
            AddPayPalRequestIdCommand command = new AddPayPalRequestIdCommand
            {
                OrderId = 1,
                PayPalRequestId = Guid.NewGuid()
            };
            AddPayPalRequestIdCommandHandler handler = new AddPayPalRequestIdCommandHandler(Mock.Of<ILogger<AddPayPalRequestIdCommandHandler>>(), this._mediator.Object);
            
            //Act
            bool result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.That(result, Is.False);
        }

        #endregion

        #region CreateOrderCommandHandler Tests

        [Test]
        public async Task CreateOrderCommandHandler_ValidRequestAndWithoutErrors_ReturnsSuccess()
        {
            //Arrange
            this._orderRepository.Setup(o => o.AddAsync(It.IsAny<Order>())).ReturnsAsync(1);
            this._orderRepository.Setup(o => o.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(this._order);
            
            this._mediator.Setup(m => m.Send(It.IsAny<CreateOrderItemCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CreateOrderItemResponse()
                {
                    Success = true,
                    Message = string.Empty,
                    OrderItem = new OrderItemDto()
                });
            
            this._mediator.Setup(m => m.Send(It.IsAny<GetProductByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetProductByIdResponse()
                {
                    Success = true,
                    Message = string.Empty,
                    Product = new ProductDto
                    {
                        Id = 1,
                        Name = "Test Product",
                        Price = 100,
                        Description = "Test Description"
                    }
                });
            
            CreateOrderCommand command = new CreateOrderCommand
            {
                CartItems = new List<CartItemDto>
                {
                    new CartItemDto
                    {
                        ProductId = 1,
                        Quantity = 1
                    }
                },
                UserId = _userId,
                UserName = _userName
            };
            CreateOrderCommandHandler handler = new CreateOrderCommandHandler(Mock.Of<ILogger<CreateOrderCommandHandler>>(), this._mapper, this._mediator.Object, this._orderRepository.Object);
            
            //Act
            CreateOrderResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Message, Is.EqualTo(OrderConstants._createSuccessMessage));
                Assert.That(result.Order, Is.Not.Null);
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }
        
        [Test]
        public async Task CreateOrderCommandHandler_WithNoCartItems_ReturnsFailedResponse()
        {
            //Arrange
            CreateOrderCommand command = new CreateOrderCommand
            {
                CartItems = new List<CartItemDto>(),
                UserId = _userId,
                UserName = _userName
            };
            CreateOrderCommandHandler handler = new CreateOrderCommandHandler(Mock.Of<ILogger<CreateOrderCommandHandler>>(), this._mapper, this._mediator.Object, this._orderRepository.Object);
            
            //Act
            CreateOrderResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(OrderConstants._createErrorMessage));
                Assert.That(result.Order, Is.Null);
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }
        
        [Test]
        public async Task CreateOrderCommandHandler_WithNoUserName_ReturnsFailedResponse()
        {
            //Arrange
            CreateOrderCommand command = new CreateOrderCommand
            {
                CartItems = new List<CartItemDto>
                {
                    new CartItemDto
                    {
                        ProductId = 1,
                        Quantity = 1
                    }
                },
                UserId = _userId,
                UserName = string.Empty
            };
            CreateOrderCommandHandler handler = new CreateOrderCommandHandler(Mock.Of<ILogger<CreateOrderCommandHandler>>(), this._mapper, this._mediator.Object, this._orderRepository.Object);
            
            //Act
            CreateOrderResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(OrderConstants._createErrorMessage));
                Assert.That(result.Order, Is.Null);
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }

        [Test]
        public async Task CreateOrderCommandHandler_WithNoUserId_ReturnsFailedResponse()
        {
            //Arrange
            CreateOrderCommand command = new CreateOrderCommand
            {
                CartItems = new List<CartItemDto>
                {
                    new CartItemDto
                    {
                        ProductId = 1,
                        Quantity = 1
                    }
                },
                UserId = Guid.Empty,
                UserName = _userName
            };
            CreateOrderCommandHandler handler = new CreateOrderCommandHandler(Mock.Of<ILogger<CreateOrderCommandHandler>>(), this._mapper, this._mediator.Object, this._orderRepository.Object);
            
            //Act
            CreateOrderResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(OrderConstants._createErrorMessage));
                Assert.That(result.Order, Is.Null);
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }
        
        [Test]
        public async Task CreateOrderCommandHandler_WhenProductIsNotFound_ReturnsFailedResponse()
        {
            //Arrange
            this._mediator.Setup(m => m.Send(It.IsAny<GetProductByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetProductByIdResponse()
                {
                    Success = false,
                    Message = ProductConstants._getProductByIdErrorMessage,
                    Product = null
                });
            
            CreateOrderCommand command = new CreateOrderCommand
            {
                CartItems = new List<CartItemDto>
                {
                    new CartItemDto
                    {
                        ProductId = 1,
                        Quantity = 1
                    }
                },
                UserId = _userId,
                UserName = _userName
            };
            CreateOrderCommandHandler handler = new CreateOrderCommandHandler(Mock.Of<ILogger<CreateOrderCommandHandler>>(), this._mapper, this._mediator.Object, this._orderRepository.Object);
            
            //Act
            CreateOrderResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(OrderConstants._createErrorMessage));
                Assert.That(result.Order, Is.Null);
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }
        
        [Test]
        public async Task CreateOrderCommandHandler_WhenOrderTotalIsNotGreaterThanZero_ReturnsFailedResponse()
        {
            //Arrange
            this._mediator.Setup(m => m.Send(It.IsAny<GetProductByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetProductByIdResponse()
                {
                    Success = true,
                    Message = string.Empty,
                    Product = new ProductDto
                    {
                        Id = 1,
                        Name = "Test Product",
                        Price = 0,
                        Description = "Test Description"
                    }
                });
            
            CreateOrderCommand command = new CreateOrderCommand
            {
                CartItems = new List<CartItemDto>
                {
                    new CartItemDto
                    {
                        ProductId = 1,
                        Quantity = 1
                    }
                },
                UserId = _userId,
                UserName = _userName
            };
            CreateOrderCommandHandler handler = new CreateOrderCommandHandler(Mock.Of<ILogger<CreateOrderCommandHandler>>(), this._mapper, this._mediator.Object, this._orderRepository.Object);
            
            //Act
            CreateOrderResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(OrderConstants._genericValidationErrorMessage));
                Assert.That(result.Order, Is.Null);
                Assert.That(result.ValidationErrors, Is.Not.Empty);
                Assert.That(result.ValidationErrors.Count, Is.EqualTo(1));
                Assert.That(result.ValidationErrors[0], Is.EqualTo("Total must be greater than 0"));
            });
        }
        
        [Test]
        public async Task CreateOrderCommandHandler_WhenOrderCreationFails_ReturnsFailedResponse()
        {
            //Arrange
            this._orderRepository.Setup(o => o.AddAsync(It.IsAny<Order>())).ReturnsAsync(-1);
            
            this._mediator.Setup(m => m.Send(It.IsAny<GetProductByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetProductByIdResponse()
                {
                    Success = true,
                    Message = string.Empty,
                    Product = new ProductDto
                    {
                        Id = 1,
                        Name = "Test Product",
                        Price = 100,
                        Description = "Test Description"
                    }
                });
            
            CreateOrderCommand command = new CreateOrderCommand
            {
                CartItems = new List<CartItemDto>
                {
                    new CartItemDto
                    {
                        ProductId = 1,
                        Quantity = 1
                    }
                },
                UserId = _userId,
                UserName = _userName
            };
            CreateOrderCommandHandler handler = new CreateOrderCommandHandler(Mock.Of<ILogger<CreateOrderCommandHandler>>(), this._mapper, this._mediator.Object, this._orderRepository.Object);
            
            //Act
            CreateOrderResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(OrderConstants._createErrorMessage));
                Assert.That(result.Order, Is.Null);
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }
        
        #endregion

        #region DeleteOrderCommandHandler Tests

        [Test]
        public async Task DeleteOrderCommandHandler_WithValidRequestAndWithoutErrors_ReturnsSuccess()
        {
            //Arrange
            this._orderRepository.Setup(o => o.DeleteAsync(It.IsAny<Order>())).ReturnsAsync(true);
            
            DeleteOrderCommand command = new DeleteOrderCommand
            {
                Order = this._orderDto
            };
            DeleteOrderCommandHandler handler = new DeleteOrderCommandHandler(Mock.Of<ILogger<DeleteOrderCommandHandler>>(), this._mapper, this._orderRepository.Object);
            
            //Act
            DeleteOrderResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Message, Is.EqualTo(OrderConstants._deleteSuccessMessage));
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }
        
        [Test]
        public async Task DeleteOrderCommandHandler_WithNullOrder_ReturnsFailedResponse()
        {
            //Arrange
            DeleteOrderCommand command = new DeleteOrderCommand
            {
                Order = null
            };
            DeleteOrderCommandHandler handler = new DeleteOrderCommandHandler(Mock.Of<ILogger<DeleteOrderCommandHandler>>(), this._mapper, this._orderRepository.Object);
            
            //Act
            DeleteOrderResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(OrderConstants._deleteErrorMessage));
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }
        
        [Test]
        public async Task DeleteOrderCommandHandler_WhenDeleteFails_ReturnsFailedResponse()
        {
            //Arrange
            this._orderRepository.Setup(o => o.DeleteAsync(It.IsAny<Order>())).ReturnsAsync(false);
            
            DeleteOrderCommand command = new DeleteOrderCommand
            {
                Order = this._orderDto
            };
            DeleteOrderCommandHandler handler = new DeleteOrderCommandHandler(Mock.Of<ILogger<DeleteOrderCommandHandler>>(), this._mapper, this._orderRepository.Object);
            
            //Act
            DeleteOrderResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(OrderConstants._deleteErrorMessage));
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }

        #endregion

        #region UpdateOrderCommandHandler Tests

        [Test]
        public async Task UpdateOrderCommandHandler_WithValidRequestAndWithoutErrors_ReturnsSuccess()
        {
            //Arrange
            this._orderRepository.Setup(o => o.UpdateAsync(It.IsAny<Order>())).ReturnsAsync(true);
            
            UpdateOrderCommand command = new UpdateOrderCommand
            {
                OrderToUpdate = this._orderDto,
                UserName = _userName
            };
            UpdateOrderCommandHandler handler = new UpdateOrderCommandHandler(Mock.Of<ILogger<UpdateOrderCommandHandler>>(), this._mapper, this._orderRepository.Object);
            
            //Act
            UpdateOrderResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Message, Is.EqualTo(OrderConstants._updateSuccessMessage));
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }
        
        [Test]
        public async Task UpdateOrderCommandHandler_WithNullOrder_ReturnsFailedResponse()
        {
            //Arrange
            UpdateOrderCommand command = new UpdateOrderCommand
            {
                OrderToUpdate = null,
                UserName = _userName
            };
            UpdateOrderCommandHandler handler = new UpdateOrderCommandHandler(Mock.Of<ILogger<UpdateOrderCommandHandler>>(), this._mapper, this._orderRepository.Object);
            
            //Act
            UpdateOrderResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(OrderConstants._updateErrorMessage));
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }
        
        [Test]
        public async Task UpdateOrderCommandHandler_WithEmptyUserName_ReturnsFailedResponse()
        {
            //Arrange
            UpdateOrderCommand command = new UpdateOrderCommand
            {
                OrderToUpdate = this._orderDto,
                UserName = string.Empty
            };
            UpdateOrderCommandHandler handler = new UpdateOrderCommandHandler(Mock.Of<ILogger<UpdateOrderCommandHandler>>(), this._mapper, this._orderRepository.Object);
            
            //Act
            UpdateOrderResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(OrderConstants._updateErrorMessage));
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }
        
        [Test]
        public async Task UpdateOrderCommandHandler_WhenOrderTotalIsNotGeaterThanZero_ReturnsFailedResponse()
        {
            //Arrange
            this._orderDto.Total = 0;
            
            UpdateOrderCommand command = new UpdateOrderCommand
            {
                OrderToUpdate = this._orderDto,
                UserName = _userName
            };
            UpdateOrderCommandHandler handler = new UpdateOrderCommandHandler(Mock.Of<ILogger<UpdateOrderCommandHandler>>(), this._mapper, this._orderRepository.Object);
            
            //Act
            UpdateOrderResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(OrderConstants._genericValidationErrorMessage));
                Assert.That(result.ValidationErrors, Is.Not.Empty);
                Assert.That(result.ValidationErrors.Count, Is.EqualTo(1));
                Assert.That(result.ValidationErrors[0], Is.EqualTo("Total must be greater than 0"));
            });
        }
        
        [Test]
        public async Task UpdateOrderCommandHandler_WhenOrderStatusIsNotInEnum_ReturnsFailedResponse()
        {
            //Arrange
            this._orderDto.Status = (OrderStatus) 100;
            
            UpdateOrderCommand command = new UpdateOrderCommand
            {
                OrderToUpdate = this._orderDto,
                UserName = _userName
            };
            UpdateOrderCommandHandler handler = new UpdateOrderCommandHandler(Mock.Of<ILogger<UpdateOrderCommandHandler>>(), this._mapper, this._orderRepository.Object);
            
            //Act
            UpdateOrderResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(OrderConstants._genericValidationErrorMessage));
                Assert.That(result.ValidationErrors, Is.Not.Empty);
                Assert.That(result.ValidationErrors.Count, Is.EqualTo(1));
                Assert.That(result.ValidationErrors[0], Is.EqualTo("Must use a predefined status"));
            });
        }
        
        [Test]
        public async Task UpdateOrderCommandHandler_WhenUpdateFails_ReturnsFailedResponse()
        {
            //Arrange
            this._orderRepository.Setup(o => o.UpdateAsync(It.IsAny<Order>())).ReturnsAsync(false);
            
            UpdateOrderCommand command = new UpdateOrderCommand
            {
                OrderToUpdate = this._orderDto,
                UserName = _userName
            };
            UpdateOrderCommandHandler handler = new UpdateOrderCommandHandler(Mock.Of<ILogger<UpdateOrderCommandHandler>>(), this._mapper, this._orderRepository.Object);
            
            //Act
            UpdateOrderResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(OrderConstants._updateErrorMessage));
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }

        #endregion

        #region GetAllOrdersByUserIdQueryHandler Tests

        [Test]
        public async Task GetAllOrdersByUserIdQueryHandler_WithValidRequestAndWithoutErrors_ReturnsSuccess()
        {
            //Arrange
            this._order.Status = OrderStatus.Processing;
            this._orderRepository.Setup(o => o.ListAllAsync(It.IsAny<Guid>())).ReturnsAsync(new List<Order>
            {
                this._order
            });
            
            GetAllOrdersByUserIdQuery query = new GetAllOrdersByUserIdQuery
            {
                UserId = _userId
            };
            GetAllOrdersByUserIdQueryHandler handler = new GetAllOrdersByUserIdQueryHandler(Mock.Of<ILogger<GetAllOrdersByUserIdQueryHandler>>(), this._mapper, this._orderRepository.Object);
            
            //Act
            GetAllOrdersByUserIdResponse result = await handler.Handle(query, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Message, Is.EqualTo(OrderConstants._getAllOrdersSuccessMessage));
                Assert.That(result.Orders, Is.Not.Empty);
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }
        
        [Test]
        public async Task GetAllOrdersByUserIdQueryHandler_WithInvalidUserId_ReturnsFailedResponse()
        {
            //Arrange
            GetAllOrdersByUserIdQuery query = new GetAllOrdersByUserIdQuery
            {
                UserId = Guid.Empty
            };
            GetAllOrdersByUserIdQueryHandler handler = new GetAllOrdersByUserIdQueryHandler(Mock.Of<ILogger<GetAllOrdersByUserIdQueryHandler>>(), this._mapper, this._orderRepository.Object);
            
            //Act
            GetAllOrdersByUserIdResponse result = await handler.Handle(query, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(OrderConstants._getAllOrdersErrorMessage));
                Assert.That(result.Orders, Is.Empty);
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }
        
        [Test]
        public async Task GetAllOrdersByUserIdQueryHandler_WithNoOrdersFound_ReturnsSuccessWithEmptyListResponse()
        {
            //Arrange
            this._orderRepository.Setup(o => o.ListAllAsync(It.IsAny<Guid>())).ReturnsAsync(new List<Order>());
            
            GetAllOrdersByUserIdQuery query = new GetAllOrdersByUserIdQuery
            {
                UserId = _userId
            };
            GetAllOrdersByUserIdQueryHandler handler = new GetAllOrdersByUserIdQueryHandler(Mock.Of<ILogger<GetAllOrdersByUserIdQueryHandler>>(), this._mapper, this._orderRepository.Object);
            
            //Act
            GetAllOrdersByUserIdResponse result = await handler.Handle(query, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Message, Is.EqualTo(OrderConstants._getAllOrdersSuccessMessage));
                Assert.That(result.Orders, Is.Empty);
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }
        
        [Test]
        public async Task GetAllOrdersByUserIdQueryHandler_WithOrdersInCreatedOrPendingStatus_ReturnsSuccessWithEmptyListResponse()
        {
            //Arrange
            this._orderRepository.Setup(o => o.ListAllAsync(It.IsAny<Guid>())).ReturnsAsync(new List<Order>
            {
                this._order
            });
            
            GetAllOrdersByUserIdQuery query = new GetAllOrdersByUserIdQuery
            {
                UserId = _userId
            };
            GetAllOrdersByUserIdQueryHandler handler = new GetAllOrdersByUserIdQueryHandler(Mock.Of<ILogger<GetAllOrdersByUserIdQueryHandler>>(), this._mapper, this._orderRepository.Object);
            
            //Act
            GetAllOrdersByUserIdResponse result = await handler.Handle(query, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Message, Is.EqualTo(OrderConstants._getAllOrdersSuccessMessage));
                Assert.That(result.Orders, Is.Empty);
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }
        
        [Test]
        public async Task GetAllOrdersByUserIdQueryHandler_WhenErrorOccurs_ReturnsFailedResponse()
        {
            //Arrange
            this._orderRepository.Setup(o => o.ListAllAsync(It.IsAny<Guid>())).ReturnsAsync((IEnumerable<Order>?) null);
            
            GetAllOrdersByUserIdQuery query = new GetAllOrdersByUserIdQuery
            {
                UserId = _userId
            };
            GetAllOrdersByUserIdQueryHandler handler = new GetAllOrdersByUserIdQueryHandler(Mock.Of<ILogger<GetAllOrdersByUserIdQueryHandler>>(), this._mapper, this._orderRepository.Object);
            
            //Act
            GetAllOrdersByUserIdResponse result = await handler.Handle(query, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(OrderConstants._getAllOrdersErrorMessage));
                Assert.That(result.Orders, Is.Empty);
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }

        #endregion

        #region GetOrderAfterSuccessfulCheckoutQueryHandler Tests

        [Test]
        public async Task GetOrderAfterSuccessfulCheckoutQueryHandler_WithValidRequestAndWithoutErrors_ReturnsSuccess()
        {
            //Arrange
            this._orderDto.Status = OrderStatus.Pending;
            this._mediator.Setup(m => m.Send(It.IsAny<GetOrderByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetOrderByIdResponse
                {
                    Success = true,
                    Order = this._orderDto
                });
            
            this._mediator.Setup(m => m.Send(It.IsAny<UpdateOrderCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new UpdateOrderResponse
                {
                    Success = true,
                    Message = OrderConstants._updateSuccessMessage
                });
            
            GetOrderAfterSuccessfulCheckoutQuery query = new GetOrderAfterSuccessfulCheckoutQuery
            {
                Id = 1
            };
            GetOrderAfterSuccessfulCheckoutQueryHandler handler = new GetOrderAfterSuccessfulCheckoutQueryHandler(Mock.Of<ILogger<GetOrderAfterSuccessfulCheckoutQueryHandler>>(), this._mediator.Object);
            
            //Act
            GetOrderAfterSuccessfulCheckoutResponse result = await handler.Handle(query, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Message, Is.EqualTo(OrderConstants._getOrderAfterSuccessfulCheckoutSuccessMessage));
            });
        }
        
        [Test]
        public async Task GetOrderAfterSuccessfulCheckoutQueryHandler_WithInvalidOrderId_ReturnsFailedResponse()
        {
            //Arrange
            this._mediator.Setup(m => m.Send(It.IsAny<GetOrderByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetOrderByIdResponse
                {
                    Success = false,
                    Order = null
                });
            
            GetOrderAfterSuccessfulCheckoutQuery query = new GetOrderAfterSuccessfulCheckoutQuery
            {
                Id = 1
            };
            GetOrderAfterSuccessfulCheckoutQueryHandler handler = new GetOrderAfterSuccessfulCheckoutQueryHandler(Mock.Of<ILogger<GetOrderAfterSuccessfulCheckoutQueryHandler>>(), this._mediator.Object);
            
            //Act
            GetOrderAfterSuccessfulCheckoutResponse result = await handler.Handle(query, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(OrderConstants._getOrderAfterSuccessfulCheckoutErrorMessage));
            });
        }
        
        [Test]
        public async Task GetOrderAfterSuccessfulCheckoutQueryHandler_WithOrderAlreadyProcessed_ReturnsFailedResponse()
        {
            //Arrange
            this._orderDto.Status = OrderStatus.Processing;
            this._mediator.Setup(m => m.Send(It.IsAny<GetOrderByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetOrderByIdResponse
                {
                    Success = true,
                    Order = this._orderDto
                });
            
            GetOrderAfterSuccessfulCheckoutQuery query = new GetOrderAfterSuccessfulCheckoutQuery
            {
                Id = 1
            };
            GetOrderAfterSuccessfulCheckoutQueryHandler handler = new GetOrderAfterSuccessfulCheckoutQueryHandler(Mock.Of<ILogger<GetOrderAfterSuccessfulCheckoutQueryHandler>>(), this._mediator.Object);
            
            //Act
            GetOrderAfterSuccessfulCheckoutResponse result = await handler.Handle(query, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(OrderConstants._getOrderAfterSuccessfulCheckoutErrorMessage));
            });
        }
        
        [Test]
        public async Task GetOrderAfterSuccessfulCheckoutQueryHandler_WhenUpdateFails_ReturnsFailedResponse()
        {
            //Arrange
            this._orderDto.Status = OrderStatus.Pending;
            this._mediator.Setup(m => m.Send(It.IsAny<GetOrderByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetOrderByIdResponse
                {
                    Success = true,
                    Order = this._orderDto
                });
            
            this._mediator.Setup(m => m.Send(It.IsAny<UpdateOrderCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new UpdateOrderResponse
                {
                    Success = false,
                    Message = OrderConstants._updateErrorMessage
                });
            
            GetOrderAfterSuccessfulCheckoutQuery query = new GetOrderAfterSuccessfulCheckoutQuery
            {
                Id = 1
            };
            GetOrderAfterSuccessfulCheckoutQueryHandler handler = new GetOrderAfterSuccessfulCheckoutQueryHandler(Mock.Of<ILogger<GetOrderAfterSuccessfulCheckoutQueryHandler>>(), this._mediator.Object);
            
            //Act
            GetOrderAfterSuccessfulCheckoutResponse result = await handler.Handle(query, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(OrderConstants._getOrderAfterSuccessfulCheckoutErrorMessage));
            });
        }

        #endregion

        #region GetOrderByIdQueryHandler Tests

        [Test]
        public async Task GetOrderByIdQueryHandler_WithValidRequestAndWithoutErrors_ReturnsSuccess()
        {
            //Arrange
            this._orderRepository.Setup(o => o.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(this._order);

            this._mediator.Setup(m => m.Send(It.IsAny<GetAllOrderItemsByOrderIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetAllOrderItemsByOrderIdResponse
                {
                    Success = true,
                    Message = string.Empty,
                    OrderItems = new List<OrderItemDto>()
                });
            
            GetOrderByIdQuery query = new GetOrderByIdQuery
            {
                Id = 1
            };
            GetOrderByIdQueryHandler handler = new GetOrderByIdQueryHandler(Mock.Of<ILogger<GetOrderByIdQueryHandler>>(), this._mapper, this._mediator.Object, this._orderRepository.Object);
            
            //Act
            GetOrderByIdResponse result = await handler.Handle(query, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Message, Is.EqualTo(OrderConstants._getOrderByIdSuccessMessage));
                Assert.That(result.Order, Is.Not.Null);
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }
        
        [Test]
        public async Task GetOrderByIdQueryHandler_WithInvalidOrderId_ReturnsFailedResponse()
        {
            //Arrange
            this._orderRepository.Setup(o => o.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Order?)null);
            
            GetOrderByIdQuery query = new GetOrderByIdQuery
            {
                Id = 1
            };
            GetOrderByIdQueryHandler handler = new GetOrderByIdQueryHandler(Mock.Of<ILogger<GetOrderByIdQueryHandler>>(), this._mapper, this._mediator.Object, this._orderRepository.Object);
            
            //Act
            GetOrderByIdResponse result = await handler.Handle(query, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(OrderConstants._getOrderByIdErrorMessage));
                Assert.That(result.Order, Is.Null);
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }
        
        [Test]
        public async Task GetOrderByIdQueryHandler_WhenOrderItemsAreNotFound_ReturnsFailedResponse()
        {
            //Arrange
            this._orderRepository.Setup(o => o.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(this._order);
            
            this._mediator.Setup(m => m.Send(It.IsAny<GetAllOrderItemsByOrderIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetAllOrderItemsByOrderIdResponse
                {
                    Success = false,
                    Message = OrderItemConstants._getAllOrderItemsByOrderIdErrorMessage,
                    OrderItems = null!
                });
            
            GetOrderByIdQuery query = new GetOrderByIdQuery
            {
                Id = 1
            };
            GetOrderByIdQueryHandler handler = new GetOrderByIdQueryHandler(Mock.Of<ILogger<GetOrderByIdQueryHandler>>(), this._mapper, this._mediator.Object, this._orderRepository.Object);
            
            //Act
            GetOrderByIdResponse result = await handler.Handle(query, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(OrderConstants._getOrderByIdErrorMessage));
                Assert.That(result.Order, Is.Null);
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }
        
        #endregion
    }
}
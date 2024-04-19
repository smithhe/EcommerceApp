using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Ecommerce.Application.Features.Order.Queries.GetOrderById;
using Ecommerce.Application.Features.OrderItem.Commands.CreateOrderItem;
using Ecommerce.Application.Features.OrderItem.Queries.GetAllOrderItemsByOrderId;
using Ecommerce.Application.Profiles;
using Ecommerce.Domain.Constants.Entities;
using Ecommerce.Domain.Entities;
using Ecommerce.Persistence.Contracts;
using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Responses.Order;
using Ecommerce.Shared.Responses.OrderItem;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace Ecommerce.UnitTests.ApplicationTests
{
    [TestFixture]
    public class OrderItemTests
    {
        private const string _userName = "Test User";

        private OrderItemDto _orderItemDto = null!;
        private OrderItem _orderItem = null!;
        
        private Mock<IOrderItemAsyncRepository> _orderItemAsyncRepository = null!;
        private Mock<IMediator> _mediator = null!;
        private IMapper _mapper = null!;
        
        [SetUp]
        public void Setup()
        {
            this._orderItemAsyncRepository = new Mock<IOrderItemAsyncRepository>();
            this._mediator = new Mock<IMediator>();
            
            this._orderItemDto = new OrderItemDto
            {
                OrderId = 1,
                Price = 10.00,
                Quantity = 1,
                ProductName = "Test Product",
                ProductDescription = "Test Description",
                ProductSku = "1234"
            };
            
            this._orderItem = new OrderItem
            {
                OrderId = 1,
                Price = 10.00,
                Quantity = 1,
                ProductName = "Test Product",
                ProductDescription = "Test Description",
                ProductSku = "1234"
            };
            
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            this._mapper = config.CreateMapper();
        }

        #region CreateOrderItemCommandHandler Tests

        [Test]
        public async Task CreateOrderItemCommandHandler_WithValidRequestAndWithoutErrors_ReturnsSuccess()
        {
            // Arrange
            CreateOrderItemCommand command = new CreateOrderItemCommand
            {
                OrderItemToCreate = this._orderItemDto,
                UserName = _userName
            };
            
            this._orderItemAsyncRepository.Setup(o => o.AddAsync(It.IsAny<OrderItem>())).ReturnsAsync(1);
            this._orderItemAsyncRepository.Setup(o => o.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(this._orderItem);
            this._mediator.Setup(m => m.Send(It.IsAny<GetOrderByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetOrderByIdResponse
                {
                    Success = true,
                    Order = new OrderDto
                    {
                        Id = 1
                    }
                });
            
            CreateOrderItemCommandHandler handler = new CreateOrderItemCommandHandler(
                Mock.Of<ILogger<CreateOrderItemCommandHandler>>(),
                this._mapper,
                this._orderItemAsyncRepository.Object,
                this._mediator.Object);
            
            // Act
            CreateOrderItemResponse result = await handler.Handle(command, CancellationToken.None);
            
            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Message, Is.Empty);
                Assert.That(result.OrderItem, Is.Not.Null);
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }
        
        [Test]
        public async Task CreateOrderItemCommandHandler_WhenUsernameIsNull_ReturnsFailedResponse()
        {
            // Arrange
            CreateOrderItemCommand command = new CreateOrderItemCommand
            {
                OrderItemToCreate = this._orderItemDto,
                UserName = string.Empty
            };
            
            CreateOrderItemCommandHandler handler = new CreateOrderItemCommandHandler(
                Mock.Of<ILogger<CreateOrderItemCommandHandler>>(),
                this._mapper,
                this._orderItemAsyncRepository.Object,
                this._mediator.Object);
            
            // Act
            CreateOrderItemResponse result = await handler.Handle(command, CancellationToken.None);
            
            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(OrderItemConstants._createUserNameErrorMessage));
                Assert.That(result.OrderItem, Is.Null);
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }
        
        [Test]
        public async Task CreateOrderItemCommandHandler_WhenQuantityIsLessThanOne_ReturnsFailedResponse()
        {
            // Arrange
            this._orderItemDto.Quantity = 0;
            CreateOrderItemCommand command = new CreateOrderItemCommand
            {
                OrderItemToCreate = this._orderItemDto,
                UserName = _userName
            };
            
            this._mediator.Setup(m => m.Send(It.IsAny<GetOrderByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetOrderByIdResponse
                {
                    Success = true,
                    Order = new OrderDto
                    {
                        Id = 1
                    }
                });
            
            CreateOrderItemCommandHandler handler = new CreateOrderItemCommandHandler(
                Mock.Of<ILogger<CreateOrderItemCommandHandler>>(),
                this._mapper,
                this._orderItemAsyncRepository.Object,
                this._mediator.Object);
            
            // Act
            CreateOrderItemResponse result = await handler.Handle(command, CancellationToken.None);
            
            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(OrderItemConstants._genericValidationErrorMessage));
                Assert.That(result.OrderItem, Is.Null);
                Assert.That(result.ValidationErrors, Has.Count.EqualTo(1));
                Assert.That(result.ValidationErrors[0], Is.EqualTo("Quantity must be greater than 0"));
            });
        }
        
        [Test]
        public async Task CreateOrderItemCommandHandler_WhenPriceIsLessThanOne_ReturnsFailedResponse()
        {
            // Arrange
            this._orderItemDto.Price = 0;
            CreateOrderItemCommand command = new CreateOrderItemCommand
            {
                OrderItemToCreate = this._orderItemDto,
                UserName = _userName
            };
            
            this._mediator.Setup(m => m.Send(It.IsAny<GetOrderByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetOrderByIdResponse
                {
                    Success = true,
                    Order = new OrderDto
                    {
                        Id = 1
                    }
                });
            
            CreateOrderItemCommandHandler handler = new CreateOrderItemCommandHandler(
                Mock.Of<ILogger<CreateOrderItemCommandHandler>>(),
                this._mapper,
                this._orderItemAsyncRepository.Object,
                this._mediator.Object);
            
            // Act
            CreateOrderItemResponse result = await handler.Handle(command, CancellationToken.None);
            
            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(OrderItemConstants._genericValidationErrorMessage));
                Assert.That(result.OrderItem, Is.Null);
                Assert.That(result.ValidationErrors, Has.Count.EqualTo(1));
                Assert.That(result.ValidationErrors[0], Is.EqualTo("Price must be greater than 0"));
            });
        }

        [Test]
        public async Task CreateOrderItemCommandHandler_WhenOrderDoesNotExist_ReturnsFailedResponse()
        {
            // Arrange
            CreateOrderItemCommand command = new CreateOrderItemCommand
            {
                OrderItemToCreate = this._orderItemDto,
                UserName = _userName
            };
            
            this._mediator.Setup(m => m.Send(It.IsAny<GetOrderByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetOrderByIdResponse
                {
                    Success = false,
                    Message = "Order not found"
                });
            
            CreateOrderItemCommandHandler handler = new CreateOrderItemCommandHandler(
                Mock.Of<ILogger<CreateOrderItemCommandHandler>>(),
                this._mapper,
                this._orderItemAsyncRepository.Object,
                this._mediator.Object);
            
            // Act
            CreateOrderItemResponse result = await handler.Handle(command, CancellationToken.None);
            
            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(OrderItemConstants._genericValidationErrorMessage));
                Assert.That(result.OrderItem, Is.Null);
                Assert.That(result.ValidationErrors, Is.Not.Empty);
                Assert.That(result.ValidationErrors[0], Is.EqualTo("Order must exist"));
            });
        }
        
        [Test]
        public async Task CreateOrderItemCommandHandler_WhenSqlOperationFails_ReturnsFailedResponse()
        {
            // Arrange
            CreateOrderItemCommand command = new CreateOrderItemCommand
            {
                OrderItemToCreate = this._orderItemDto,
                UserName = _userName
            };
            
            this._orderItemAsyncRepository.Setup(o => o.AddAsync(It.IsAny<OrderItem>())).ReturnsAsync(-1);
            this._mediator.Setup(m => m.Send(It.IsAny<GetOrderByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetOrderByIdResponse
                {
                    Success = true,
                    Order = new OrderDto
                    {
                        Id = 1
                    }
                });
            
            CreateOrderItemCommandHandler handler = new CreateOrderItemCommandHandler(
                Mock.Of<ILogger<CreateOrderItemCommandHandler>>(),
                this._mapper,
                this._orderItemAsyncRepository.Object,
                this._mediator.Object);
            
            // Act
            CreateOrderItemResponse result = await handler.Handle(command, CancellationToken.None);
            
            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(OrderItemConstants._createSqlErrorMessage));
                Assert.That(result.OrderItem, Is.Null);
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }
        
        #endregion

        #region GetAllOrderItemsByOrderIdQueryHandler Tests

        [Test]
        public async Task GetAllOrderItemsByOrderIdQueryHandler_WithValidRequestAndWithoutErrors_ReturnsSuccess()
        {
            // Arrange
            GetAllOrderItemsByOrderIdQuery query = new GetAllOrderItemsByOrderIdQuery
            {
                OrderId = 1
            };
            
            this._orderItemAsyncRepository.Setup(o => o.ListAllAsync(It.IsAny<int>()))
                .ReturnsAsync(new List<OrderItem>
                {
                    this._orderItem
                });
            
            GetAllOrderItemsByOrderIdQueryHandler handler = new GetAllOrderItemsByOrderIdQueryHandler(
                Mock.Of<ILogger<GetAllOrderItemsByOrderIdQueryHandler>>(),
                this._mapper,
                this._orderItemAsyncRepository.Object);
            
            // Act
            GetAllOrderItemsByOrderIdResponse result = await handler.Handle(query, CancellationToken.None);
            
            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Message, Is.Empty);
                Assert.That(result.OrderItems, Is.Not.Empty);
            });
        }

        [Test]
        public async Task GetAllOrderItemsByOrderIdQueryHandler_WhenNoOrderItemsFound_ReturnsFailedResponse()
        {
            // Arrange
            GetAllOrderItemsByOrderIdQuery query = new GetAllOrderItemsByOrderIdQuery
            {
                OrderId = 1
            };
            
            this._orderItemAsyncRepository.Setup(o => o.ListAllAsync(It.IsAny<int>()))
                .ReturnsAsync(new List<OrderItem>());
            
            GetAllOrderItemsByOrderIdQueryHandler handler = new GetAllOrderItemsByOrderIdQueryHandler(
                Mock.Of<ILogger<GetAllOrderItemsByOrderIdQueryHandler>>(),
                this._mapper,
                this._orderItemAsyncRepository.Object);
            
            // Act
            GetAllOrderItemsByOrderIdResponse result = await handler.Handle(query, CancellationToken.None);
            
            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(OrderItemConstants._getAllOrderItemsByOrderIdErrorMessage));
                Assert.That(result.OrderItems, Is.Empty);
            });
        }
        
        #endregion
    }
}
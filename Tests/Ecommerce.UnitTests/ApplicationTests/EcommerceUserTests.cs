using System;
using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Application.Features.EcommerceUser.Commands.ConfirmEmail;
using Ecommerce.Application.Features.EcommerceUser.Commands.RegisterEcommerceUser;
using Ecommerce.Application.Features.EcommerceUser.Commands.UpdateEcommerceUser;
using Ecommerce.Application.Features.EcommerceUser.Commands.UpdatePassword;
using Ecommerce.Domain.Constants.Entities;
using Ecommerce.Identity.Contracts;
using Ecommerce.Shared.Security.Requests;
using Ecommerce.Shared.Security.Responses;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace Ecommerce.UnitTests.ApplicationTests
{
    [TestFixture]
    public class EcommerceUserTests
    {
        private static readonly Guid _userId = new Guid("095a987b-b4da-4eb6-a286-19aa3c75be53");
        private const string _userName = "Test User";
        private const string _token = "token";
        
        private Mock<IAuthenticationService> _authenticationServiceMock = null!;
        private Mock<IConfiguration> _configurationMock = null!;
        private Mock<IBus> _busMock = null!;
        
        [SetUp]
        public void Setup()
        {
            this._authenticationServiceMock = new Mock<IAuthenticationService>();
            this._configurationMock = new Mock<IConfiguration>();
            this._busMock = new Mock<IBus>();
        }

        #region ConfirmEmailCommandHandler Tests

        [Test]
        public async Task ConfirmEmailCommandHandler_WhenServiceReturnsResponse_ShouldReturnSameResponse()
        {
            //Arrange
            ConfirmEmailCommand command = new ConfirmEmailCommand
            {
                Token = _token,
                UserId = _userId.ToString()
            };
            
            ConfirmEmailResponse expectedResponse = new ConfirmEmailResponse
            {
                Success = true,
                Message = "Email confirmed successfully"
            };
            
            this._authenticationServiceMock.Setup(x => x.ConfirmEmailAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(expectedResponse);

            ConfirmEmailCommandHandler handler = new ConfirmEmailCommandHandler(Mock.Of<ILogger<ConfirmEmailCommandHandler>>(), this._authenticationServiceMock.Object);

            
            //Act
            ConfirmEmailResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.EqualTo(expectedResponse.Success));
                Assert.That(result.Message, Is.EqualTo(expectedResponse.Message));
            });
        }

        #endregion

        #region RegisterEcommerceUserCommandHandler Tests

        [Test]
        public async Task RegisterEcommerceUserCommandHandler_WhenServiceReturnsResponse_ShouldReturnSameResponse()
        {
            //Arrange
            RegisterEcommerceUserCommand command = new RegisterEcommerceUserCommand
            {
                CreateUserRequest = new CreateUserRequest
                {
                    UserName = _userName,
                    Password = "password",
                    EmailAddress = "email@email.com",
                    FirstName = "First",
                    LastName = "Last"
                },
                LinkUrl = "link"
            };
            
            CreateUserResponse serviceResponse = new CreateUserResponse
            {
                Success = true,
                ConfirmationLink = "token",
                Errors = Array.Empty<string>()
            };
            
            CreateUserResponse expectedResponse = new CreateUserResponse
            {
                Success = true,
                ConfirmationLink = $"{command.LinkUrl}?userId={_userId}&emailToken={serviceResponse.ConfirmationLink}",
                Errors = Array.Empty<string>()
            };
            
            this._authenticationServiceMock.Setup(x => x.CreateUserAsync(It.IsAny<CreateUserRequest>())).ReturnsAsync(serviceResponse);
            this._authenticationServiceMock.Setup(x => x.GetUserIdByName(It.IsAny<string>())).ReturnsAsync(_userId);

            RegisterEcommerceUserCommandHandler handler = new RegisterEcommerceUserCommandHandler(Mock.Of<ILogger<RegisterEcommerceUserCommandHandler>>(), this._authenticationServiceMock.Object, this._configurationMock.Object, this._busMock.Object);
            
            //Act
            CreateUserResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.EqualTo(expectedResponse.Success));
                Assert.That(result.ConfirmationLink, Is.EqualTo(expectedResponse.ConfirmationLink));
                Assert.That(result.Errors, Is.EqualTo(expectedResponse.Errors));
            });
        }

        [Test]
        public async Task RegisterEcommerceUserCommandHandler_WhenServiceReturnsFailedResponse_ShouldReturnSameResponse()
        {
            //Arrange
            RegisterEcommerceUserCommand command = new RegisterEcommerceUserCommand
            {
                CreateUserRequest = new CreateUserRequest
                {
                    UserName = _userName,
                    Password = "password",
                    EmailAddress = "email@email.com",
                    FirstName = "First",
                    LastName = "Last"
                },
                LinkUrl = "link"
            };
            
            CreateUserResponse serviceResponse = new CreateUserResponse
            {
                Success = false,
                ConfirmationLink = string.Empty,
                Errors = new[] {"Error"}
            };
            
            this._authenticationServiceMock.Setup(x => x.CreateUserAsync(It.IsAny<CreateUserRequest>())).ReturnsAsync(serviceResponse);
            this._authenticationServiceMock.Setup(x => x.GetUserIdByName(It.IsAny<string>())).ReturnsAsync(_userId);

            RegisterEcommerceUserCommandHandler handler = new RegisterEcommerceUserCommandHandler(Mock.Of<ILogger<RegisterEcommerceUserCommandHandler>>(), this._authenticationServiceMock.Object, this._configurationMock.Object, this._busMock.Object);
            
            //Act
            CreateUserResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.ConfirmationLink, Is.EqualTo(string.Empty));
                Assert.That(result.Errors, Is.Not.Empty);
            });
        }
        
        [Test]
        public async Task RegisterEcommerceUserCommandHandler_WhenServiceReturnsNoToken_ShouldReturnSameResponse()
        {
            //Arrange
            RegisterEcommerceUserCommand command = new RegisterEcommerceUserCommand
            {
                CreateUserRequest = new CreateUserRequest
                {
                    UserName = _userName,
                    Password = "password",
                    EmailAddress = "email@email.com",
                    FirstName = "First",
                    LastName = "Last"
                },
                LinkUrl = "link"
            };
            
            CreateUserResponse serviceResponse = new CreateUserResponse
            {
                Success = false,
                ConfirmationLink = string.Empty,
                Errors = Array.Empty<string>()
            };
            
            this._authenticationServiceMock.Setup(x => x.CreateUserAsync(It.IsAny<CreateUserRequest>())).ReturnsAsync(serviceResponse);
            this._authenticationServiceMock.Setup(x => x.GetUserIdByName(It.IsAny<string>())).ReturnsAsync(_userId);

            RegisterEcommerceUserCommandHandler handler = new RegisterEcommerceUserCommandHandler(Mock.Of<ILogger<RegisterEcommerceUserCommandHandler>>(), this._authenticationServiceMock.Object, this._configurationMock.Object, this._busMock.Object);
            
            //Act
            CreateUserResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.ConfirmationLink, Is.EqualTo(string.Empty));
                Assert.That(result.Errors, Is.Empty);
            });
        }
        
        [Test]
        public async Task RegisterEcommerceUserCommandHandler_WhenUserIdNotFound_ShouldReturnFailedResponse()
        {
            //Arrange
            RegisterEcommerceUserCommand command = new RegisterEcommerceUserCommand
            {
                CreateUserRequest = new CreateUserRequest
                {
                    UserName = _userName,
                    Password = "password",
                    EmailAddress = "email@email.com",
                    FirstName = "First",
                    LastName = "Last"
                },
                LinkUrl = "link"
            };
            
            CreateUserResponse serviceResponse = new CreateUserResponse
            {
                Success = true,
                ConfirmationLink = "token",
                Errors = Array.Empty<string>()
            };
            
            this._authenticationServiceMock.Setup(x => x.CreateUserAsync(It.IsAny<CreateUserRequest>())).ReturnsAsync(serviceResponse);
            this._authenticationServiceMock.Setup(x => x.GetUserIdByName(It.IsAny<string>())).ReturnsAsync((Guid?)null);

            RegisterEcommerceUserCommandHandler handler = new RegisterEcommerceUserCommandHandler(Mock.Of<ILogger<RegisterEcommerceUserCommandHandler>>(), this._authenticationServiceMock.Object, this._configurationMock.Object, this._busMock.Object);
            
            //Act
            CreateUserResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.ConfirmationLink, Is.EqualTo(string.Empty));
                Assert.That(result.Errors, Is.Empty);
            });
        }

        #endregion

        #region UpdateEcommerceUserCommandHandler Tests

        [Test]
        public async Task UpdateEcommerceUserCommandHandler_WithValidRequestAndWithoutErrors_ReturnsSuccess()
        {
            //Arrange
            UpdateEcommerceUserCommand command = new UpdateEcommerceUserCommand
            {
                UserId = _userId,
                UserName = _userName,
                Email = "email",
                FirstName = "First",
                LastName = "Last"
            };
            
            UpdateEcommerceUserResponse expectedResponse = new UpdateEcommerceUserResponse
            {
                Success = true,
                Message = EcommerceUserConstants._updateUserSuccessMessage
            };
            
            this._authenticationServiceMock.Setup(x => x.GetUserById(It.IsAny<Guid>())).ReturnsAsync(new Domain.Entities.EcommerceUser());
            this._authenticationServiceMock.Setup(x => x.UpdateUser(It.IsAny<Domain.Entities.EcommerceUser>(), It.IsAny<string>())).ReturnsAsync(new UpdateEcommerceUserResponse
            {
                Success = true,
                Message = EcommerceUserConstants._updateUserSuccessMessage,
                UpdatedAccessToken = "token"
            });

            UpdateEcommerceUserCommandHandler handler = new UpdateEcommerceUserCommandHandler(Mock.Of<ILogger<UpdateEcommerceUserCommandHandler>>(), this._authenticationServiceMock.Object);
            
            //Act
            UpdateEcommerceUserResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.EqualTo(expectedResponse.Success));
                Assert.That(result.Message, Is.EqualTo(expectedResponse.Message));
            });
        }
        
        [Test]
        public async Task UpdateEcommerceUserCommandHandler_WithoutUserName_ReturnsFailedResponse()
        {
            //Arrange
            UpdateEcommerceUserCommand command = new UpdateEcommerceUserCommand
            {
                UserId = _userId,
                UserName = string.Empty,
                Email = "email",
                FirstName = "First",
                LastName = "Last"
            };
            
            UpdateEcommerceUserResponse expectedResponse = new UpdateEcommerceUserResponse
            {
                Success = false,
                Message = EcommerceUserConstants._updateUserErrorMessage
            };
            
            UpdateEcommerceUserCommandHandler handler = new UpdateEcommerceUserCommandHandler(Mock.Of<ILogger<UpdateEcommerceUserCommandHandler>>(), this._authenticationServiceMock.Object);
            
            //Act
            UpdateEcommerceUserResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.EqualTo(expectedResponse.Success));
                Assert.That(result.Message, Is.EqualTo(expectedResponse.Message));
            });
        }
        
        [Test]
        public async Task UpdateEcommerceUserCommandHandler_WithoutEmail_ReturnsFailedResponse()
        {
            //Arrange
            UpdateEcommerceUserCommand command = new UpdateEcommerceUserCommand
            {
                UserId = _userId,
                UserName = _userName,
                Email = string.Empty,
                FirstName = "First",
                LastName = "Last"
            };
            
            UpdateEcommerceUserResponse expectedResponse = new UpdateEcommerceUserResponse
            {
                Success = false,
                Message = EcommerceUserConstants._updateUserErrorMessage
            };
            
            UpdateEcommerceUserCommandHandler handler = new UpdateEcommerceUserCommandHandler(Mock.Of<ILogger<UpdateEcommerceUserCommandHandler>>(), this._authenticationServiceMock.Object);
            
            //Act
            UpdateEcommerceUserResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.EqualTo(expectedResponse.Success));
                Assert.That(result.Message, Is.EqualTo(expectedResponse.Message));
            });
        }
        
        [Test]
        public async Task UpdateEcommerceUserCommandHandler_WithoutFirstName_ReturnsFailedResponse()
        {
            //Arrange
            UpdateEcommerceUserCommand command = new UpdateEcommerceUserCommand
            {
                UserId = _userId,
                UserName = _userName,
                Email = "email",
                FirstName = string.Empty,
                LastName = "Last"
            };
            
            UpdateEcommerceUserResponse expectedResponse = new UpdateEcommerceUserResponse
            {
                Success = false,
                Message = EcommerceUserConstants._updateUserErrorMessage
            };
            
            UpdateEcommerceUserCommandHandler handler = new UpdateEcommerceUserCommandHandler(Mock.Of<ILogger<UpdateEcommerceUserCommandHandler>>(), this._authenticationServiceMock.Object);
            
            //Act
            UpdateEcommerceUserResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.EqualTo(expectedResponse.Success));
                Assert.That(result.Message, Is.EqualTo(expectedResponse.Message));
            });
        }
        
        [Test]
        public async Task UpdateEcommerceUserCommandHandler_WithoutLastName_ReturnsFailedResponse()
        {
            //Arrange
            UpdateEcommerceUserCommand command = new UpdateEcommerceUserCommand
            {
                UserId = _userId,
                UserName = _userName,
                Email = "email",
                FirstName = "First",
                LastName = string.Empty
            };
            
            UpdateEcommerceUserResponse expectedResponse = new UpdateEcommerceUserResponse
            {
                Success = false,
                Message = EcommerceUserConstants._updateUserErrorMessage
            };
            
            UpdateEcommerceUserCommandHandler handler = new UpdateEcommerceUserCommandHandler(Mock.Of<ILogger<UpdateEcommerceUserCommandHandler>>(), this._authenticationServiceMock.Object);
            
            //Act
            UpdateEcommerceUserResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.EqualTo(expectedResponse.Success));
                Assert.That(result.Message, Is.EqualTo(expectedResponse.Message));
            });
        }

        [Test]
        public async Task UpdateEcommerceUserCommandHandler_WhenUserNotFound_ReturnsFailedResponse()
        {
            //Arrange
            UpdateEcommerceUserCommand command = new UpdateEcommerceUserCommand
            {
                UserId = _userId,
                UserName = _userName,
                Email = "email",
                FirstName = "First",
                LastName = "Last"
            };
            
            UpdateEcommerceUserResponse expectedResponse = new UpdateEcommerceUserResponse
            {
                Success = false,
                Message = EcommerceUserConstants._updateUserErrorMessage
            };
            
            this._authenticationServiceMock.Setup(x => x.GetUserById(It.IsAny<Guid>())).ReturnsAsync((Domain.Entities.EcommerceUser?)null);

            UpdateEcommerceUserCommandHandler handler = new UpdateEcommerceUserCommandHandler(Mock.Of<ILogger<UpdateEcommerceUserCommandHandler>>(), this._authenticationServiceMock.Object);
            
            //Act
            UpdateEcommerceUserResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.EqualTo(expectedResponse.Success));
                Assert.That(result.Message, Is.EqualTo(expectedResponse.Message));
            });
        }
        
        [Test]
        public async Task UpdateEcommerceUserCommandHandler_WhenUpdateFails_ReturnsFailedResponse()
        {
            //Arrange
            UpdateEcommerceUserCommand command = new UpdateEcommerceUserCommand
            {
                UserId = _userId,
                UserName = _userName,
                Email = "email",
                FirstName = "First",
                LastName = "Last"
            };
            
            UpdateEcommerceUserResponse expectedResponse = new UpdateEcommerceUserResponse
            {
                Success = false,
                Message = EcommerceUserConstants._updateUserErrorMessage
            };
            
            this._authenticationServiceMock.Setup(x => x.GetUserById(It.IsAny<Guid>())).ReturnsAsync(new Domain.Entities.EcommerceUser());
            this._authenticationServiceMock.Setup(x => x.UpdateUser(It.IsAny<Domain.Entities.EcommerceUser>(), It.IsAny<string>())).ReturnsAsync(new UpdateEcommerceUserResponse
            {
                Success = false,
                Message = EcommerceUserConstants._updateUserErrorMessage
            });

            UpdateEcommerceUserCommandHandler handler = new UpdateEcommerceUserCommandHandler(Mock.Of<ILogger<UpdateEcommerceUserCommandHandler>>(), this._authenticationServiceMock.Object);
            
            //Act
            UpdateEcommerceUserResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.EqualTo(expectedResponse.Success));
                Assert.That(result.Message, Is.EqualTo(expectedResponse.Message));
            });
        }
        
        #endregion

        #region UpdatePasswordCommandHandler Tests

        [Test]
        public async Task UpdatePasswordCommandHandler_WithValidRequestAndWithoutErrors_ReturnsSuccess()
        {
            //Arrange
            UpdatePasswordCommand command = new UpdatePasswordCommand
            {
                UserName = _userName,
                CurrentPassword = "password",
                NewPassword = "newPassword"
            };
            
            this._authenticationServiceMock.Setup(x => x.GetUserIdByName(It.IsAny<string>())).ReturnsAsync(_userId);
            this._authenticationServiceMock.Setup(x => x.GetUserById(It.IsAny<Guid>())).ReturnsAsync(new Domain.Entities.EcommerceUser());
            this._authenticationServiceMock.Setup(x => x.UpdatePassword(It.IsAny<Domain.Entities.EcommerceUser>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new UpdatePasswordResponse
            {
                Success = true,
                Message = EcommerceUserConstants._updatePasswordSuccessMessage
            });

            UpdatePasswordCommandHandler handler = new UpdatePasswordCommandHandler(Mock.Of<ILogger<UpdatePasswordCommandHandler>>(), this._authenticationServiceMock.Object);
            
            //Act
            UpdatePasswordResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Message, Is.EqualTo(EcommerceUserConstants._updatePasswordSuccessMessage));
            });
        }
        
        [Test]
        public async Task UpdatePasswordCommandHandler_WithoutUserName_ReturnsFailedResponse()
        {
            //Arrange
            UpdatePasswordCommand command = new UpdatePasswordCommand
            {
                UserName = string.Empty,
                CurrentPassword = "password",
                NewPassword = "newPassword"
            };
            
            UpdatePasswordResponse expectedResponse = new UpdatePasswordResponse
            {
                Success = false,
                Message = EcommerceUserConstants._updatePasswordErrorMessage
            };
            
            UpdatePasswordCommandHandler handler = new UpdatePasswordCommandHandler(Mock.Of<ILogger<UpdatePasswordCommandHandler>>(), this._authenticationServiceMock.Object);
            
            //Act
            UpdatePasswordResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.EqualTo(expectedResponse.Success));
                Assert.That(result.Message, Is.EqualTo(expectedResponse.Message));
            });
        }
        
        [Test]
        public async Task UpdatePasswordCommandHandler_WithoutCurrentPassword_ReturnsFailedResponse()
        {
            //Arrange
            UpdatePasswordCommand command = new UpdatePasswordCommand
            {
                UserName = _userName,
                CurrentPassword = string.Empty,
                NewPassword = "newPassword"
            };
            
            UpdatePasswordResponse expectedResponse = new UpdatePasswordResponse
            {
                Success = false,
                Message = EcommerceUserConstants._updatePasswordErrorMessage
            };
            
            UpdatePasswordCommandHandler handler = new UpdatePasswordCommandHandler(Mock.Of<ILogger<UpdatePasswordCommandHandler>>(), this._authenticationServiceMock.Object);
            
            //Act
            UpdatePasswordResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.EqualTo(expectedResponse.Success));
                Assert.That(result.Message, Is.EqualTo(expectedResponse.Message));
            });
        }
        
        [Test]
        public async Task UpdatePasswordCommandHandler_WithoutNewPassword_ReturnsFailedResponse()
        {
            //Arrange
            UpdatePasswordCommand command = new UpdatePasswordCommand
            {
                UserName = _userName,
                CurrentPassword = "password",
                NewPassword = string.Empty
            };
            
            UpdatePasswordResponse expectedResponse = new UpdatePasswordResponse
            {
                Success = false,
                Message = EcommerceUserConstants._updatePasswordErrorMessage
            };
            
            UpdatePasswordCommandHandler handler = new UpdatePasswordCommandHandler(Mock.Of<ILogger<UpdatePasswordCommandHandler>>(), this._authenticationServiceMock.Object);
            
            //Act
            UpdatePasswordResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.EqualTo(expectedResponse.Success));
                Assert.That(result.Message, Is.EqualTo(expectedResponse.Message));
            });
        }
        
        [Test]
        public async Task UpdatePasswordCommandHandler_WhenUserNotFound_ReturnsFailedResponse()
        {
            //Arrange
            UpdatePasswordCommand command = new UpdatePasswordCommand
            {
                UserName = _userName,
                CurrentPassword = "password",
                NewPassword = "newPassword"
            };
            
            UpdatePasswordResponse expectedResponse = new UpdatePasswordResponse
            {
                Success = false,
                Message = EcommerceUserConstants._updatePasswordErrorMessage
            };
            
            this._authenticationServiceMock.Setup(x => x.GetUserIdByName(It.IsAny<string>())).ReturnsAsync((Guid?)null);

            UpdatePasswordCommandHandler handler = new UpdatePasswordCommandHandler(Mock.Of<ILogger<UpdatePasswordCommandHandler>>(), this._authenticationServiceMock.Object);
            
            //Act
            UpdatePasswordResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.EqualTo(expectedResponse.Success));
                Assert.That(result.Message, Is.EqualTo(expectedResponse.Message));
            });
        }

        #endregion
        
        
    }
}
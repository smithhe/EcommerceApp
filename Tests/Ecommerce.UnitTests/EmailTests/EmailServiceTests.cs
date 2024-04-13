using System;
using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Mail.Models.Enums;
using Ecommerce.Mail.Models.TemplateModels;
using Ecommerce.Mail.Services;
using FluentEmail.Core;
using FluentEmail.Core.Models;
using Microsoft.Extensions.Logging;
using Moq;

namespace Ecommerce.UnitTests.EmailTests
{
    public class EmailServiceTests
    {
        private const string _sendTo = "test@email.com";
        
        private Mock<IFluentEmail> _fluentEmailMock = null!;
        
        [SetUp]
        public void Setup()
        {
            this._fluentEmailMock = new Mock<IFluentEmail>();
        }

        [Test]
        public async Task SendEmailAsync_WhenEmailSendsSuccessfully_LogSuccess()
        {
            //Arrange
            this._fluentEmailMock.Setup(f => f.To(_sendTo)).Returns(this._fluentEmailMock.Object);
            this._fluentEmailMock.Setup(f => f.Subject(It.IsAny<string>())).Returns(this._fluentEmailMock.Object);
            this._fluentEmailMock.Setup(f => f.SendAsync(default(CancellationToken?))).ReturnsAsync(new SendResponse());
            
            EmailService emailService = new EmailService(Mock.Of<ILogger<EmailService>>(), this._fluentEmailMock.Object);
            
            //Act
            await emailService.SendEmailAsync(_sendTo, "Test Subject", EmailTemplate.EmailConfirmation, new EmailConfirmationModel());
            
            //Assert
            this._fluentEmailMock.Verify(f => f.To(It.IsAny<string>()), Times.Once);
            this._fluentEmailMock.Verify(f => f.Subject(It.IsAny<string>()), Times.Once);
            this._fluentEmailMock.Verify(f => f.SendAsync(null), Times.Once);
        }
        
        [Test]
        public void SendEmailAsync_WhenExceptionOnToIsThrown_LogError()
        {
            //Arrange
            this._fluentEmailMock.Setup(f => f.To(_sendTo)).Throws<Exception>();
            
            EmailService emailService = new EmailService(Mock.Of<ILogger<EmailService>>(), this._fluentEmailMock.Object);
            
            //Act & Assert
            Assert.ThrowsAsync<Exception>(() => emailService.SendEmailAsync(_sendTo, "Test Subject", EmailTemplate.EmailConfirmation, new EmailConfirmationModel()));
            
            this._fluentEmailMock.Verify(f => f.To(It.IsAny<string>()), Times.Once);
        }
        
        [Test]
        public void SendEmailAsync_WhenExceptionOnSubjectIsThrown_LogError()
        {
            //Arrange
            this._fluentEmailMock.Setup(f => f.To(_sendTo)).Returns(this._fluentEmailMock.Object);
            this._fluentEmailMock.Setup(f => f.Subject(It.IsAny<string>())).Throws<Exception>();
            
            EmailService emailService = new EmailService(Mock.Of<ILogger<EmailService>>(), this._fluentEmailMock.Object);
            
            //Act & Assert
            Assert.ThrowsAsync<Exception>(() => emailService.SendEmailAsync(_sendTo, "Test Subject", EmailTemplate.EmailConfirmation, new EmailConfirmationModel()));
            
            this._fluentEmailMock.Verify(f => f.To(It.IsAny<string>()), Times.Once);
            this._fluentEmailMock.Verify(f => f.Subject(It.IsAny<string>()), Times.Once);
        }
        
        [Test]
        public void SendEmailAsync_WhenExceptionOnTemplateIsThrown_LogError()
        {
            //Arrange
            this._fluentEmailMock.Setup(f => f.To(_sendTo)).Returns(this._fluentEmailMock.Object);
            this._fluentEmailMock.Setup(f => f.Subject(It.IsAny<string>())).Returns(this._fluentEmailMock.Object);
            
            EmailService emailService = new EmailService(Mock.Of<ILogger<EmailService>>(), this._fluentEmailMock.Object);
            EmailTemplate badTemplate = (EmailTemplate) 1000000000;
            
            //Act & Assert
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => emailService.SendEmailAsync(_sendTo, "Test Subject", badTemplate, new EmailConfirmationModel()));
            
            this._fluentEmailMock.Verify(f => f.To(It.IsAny<string>()), Times.Once);
            this._fluentEmailMock.Verify(f => f.Subject(It.IsAny<string>()), Times.Once);
        }
        
        [Test]
        public void SendEmailAsync_WhenExceptionOnSendAsyncIsThrown_LogError()
        {
            //Arrange
            this._fluentEmailMock.Setup(f => f.To(_sendTo)).Returns(this._fluentEmailMock.Object);
            this._fluentEmailMock.Setup(f => f.Subject(It.IsAny<string>())).Returns(this._fluentEmailMock.Object);
            this._fluentEmailMock.Setup(f => f.SendAsync(default(CancellationToken?))).Throws<Exception>();
            
            EmailService emailService = new EmailService(Mock.Of<ILogger<EmailService>>(), this._fluentEmailMock.Object);
            
            //Act & Assert
            Assert.ThrowsAsync<Exception>(() => emailService.SendEmailAsync(_sendTo, "Test Subject", EmailTemplate.EmailConfirmation, new EmailConfirmationModel()));
            
            this._fluentEmailMock.Verify(f => f.To(It.IsAny<string>()), Times.Once);
            this._fluentEmailMock.Verify(f => f.Subject(It.IsAny<string>()), Times.Once);
            this._fluentEmailMock.Verify(f => f.SendAsync(null), Times.Once);
        }
    }
}
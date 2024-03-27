using System;
using Ecommerce.Mail.Contracts;
using Ecommerce.Mail.Models;
using Ecommerce.Mail.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.Mail
{
    /// <summary>
    /// Static class for registering mail services
    /// </summary>
    public static class MailServiceRegistration
    {
        /// <summary>
        /// Registers the mail services
        /// </summary>
        /// <param name="services">The service collection to add services to</param>
        /// <param name="configuration">The <see cref="IConfiguration"/> instance used for configuration settings.</param>
        /// <exception cref="Exception">Thrown if no mail settings were found in the configuration</exception>
        public static void AddMailServices(this IServiceCollection services, IConfiguration configuration)
        {
            MailSettings? mailSettings = configuration.GetSection("MailSettings").Get<MailSettings>();
            
            if (mailSettings == null)
            {
                throw new Exception("Mail settings not found");
            }
            
            //Register FluentEmail services
            services.AddFluentEmail(mailSettings.UserName)
                .AddRazorRenderer()
                .AddSmtpSender(mailSettings.Host, mailSettings.Port, mailSettings.UserName, mailSettings.Password);
            
            //Register EmailService
            services.AddScoped<IEmailService, EmailService>();
            
        }
    }
}
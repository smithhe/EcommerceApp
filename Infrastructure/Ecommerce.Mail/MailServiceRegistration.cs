using System;
using System.Net.Mail;
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
            
            if (mailSettings == null || string.IsNullOrEmpty(mailSettings.Host) || string.IsNullOrEmpty(mailSettings.UserName) || string.IsNullOrEmpty(mailSettings.Password) || mailSettings.Port == 0)
            {
                throw new Exception("Mail settings not found");
            }
            
            //Use for local testing with MailHog
            // SmtpClient client = new SmtpClient
            // {
            //     EnableSsl = false,
            //     Port = 1025,
            //     Host = "mailhog"
            // };
            
            //Use for sending emails with a real SMTP server
            SmtpClient client = new SmtpClient (mailSettings.Host)
            {
                EnableSsl = true,
                Port = mailSettings.Port,
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential(mailSettings.UserName, mailSettings.Password)
            };
            
            //Register FluentEmail services
            services.AddFluentEmail(mailSettings.UserName)
                .AddRazorRenderer()
                .AddSmtpSender(client);
            
            //Register EmailService
            services.AddScoped<IEmailService, EmailService>();
            
        }
    }
}
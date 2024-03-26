using System.Reflection;
using Microsoft.Extensions.Hosting;
using MassTransit;
using Microsoft.Extensions.Configuration;
using System;

IHostBuilder builder = Host.CreateDefaultBuilder(args);

builder.ConfigureAppConfiguration((hostingContext, config) =>
{
    config.AddEnvironmentVariables("Ecommerce_");
    config.AddUserSecrets<Program>();
});


builder.ConfigureServices((hostContext, services) =>
{
    services.AddMassTransit(brc =>
    {
        brc.SetKebabCaseEndpointNameFormatter();
		
        Assembly? entryAssembly = Assembly.GetEntryAssembly();

        brc.AddConsumers(entryAssembly);

        string? uri = hostContext.Configuration["RabbitMQ:Uri"];
        string? username = hostContext.Configuration["RabbitMQ:Username"];
        string? password = hostContext.Configuration["RabbitMQ:Password"];

        if (string.IsNullOrEmpty(uri) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            Console.WriteLine("Config values not set for Rabbit URI, Username, or Password");
            Console.WriteLine("Exiting....");
            Environment.Exit(1);
        }
		
        brc.UsingRabbitMq((context,cfg) =>
        {
            cfg.Host(uri, "/", h => {
                h.Username(username);
                h.Password(password);
            });

            cfg.PrefetchCount = 1;

            cfg.ConfigureEndpoints(context);
        });
						
						
    });
});

builder.Build().Run();
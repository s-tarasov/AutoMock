using ExampleApp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var serviceProvider = new ServiceCollection()
    .AddTransient<App>()
    .AddSingleton<IRateApiClient, RateApiClient>()
    .AddSingleton<INotificationSender, NotificationSender>()
    .AddSingleton<INotificationRepository, NotificationRepository>()
    .AddLogging(builder =>
    {
        builder.AddConsole();
    })
    .Configure<RateOptions>(configuration)
    .AddOptions()
    .BuildServiceProvider();

var app = serviceProvider.GetRequiredService<App>();
try
{
    await app.Run();
}
catch (Exception ex)
{
    serviceProvider.GetRequiredService<ILogger<App>>().LogCritical(ex, "Application start-up failed");
    throw;
}

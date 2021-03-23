using ExampleApp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ExampleAppTests
{

    public class AppTestsWithContainer
    {
        private readonly App _app;
        private readonly Mock<INotificationSender> _notificationSenderMock;
        private readonly Mock<INotificationRepository> _notificationRepositoryMock;
        private decimal _usdRate;
        private const decimal MaxRate = 50;

        public AppTestsWithContainer()
        {
            var rateApiClientStub = new Mock<IRateApiClient>();
            rateApiClientStub
                .Setup(c => c.GetRatesAsync())
                .ReturnsAsync(() => new RatesResponse(
                    Valute: new Dictionary<string, Rate> 
                    { 
                     ["USD"] = new Rate("Äמככאנ ÑØÀ", _usdRate)
                    }
                ));

            _notificationSenderMock = new Mock<INotificationSender>();
            _notificationRepositoryMock = new Mock<INotificationRepository>();            

            var serviceProvider = new ServiceCollection()
                .AddTransient<App>()
                .AddSingleton(Mock.Of<ILogger<App>>())
                .AddSingleton(Mock.Of<IOptions<RateOptions>>(o => o.Value == new RateOptions { Valute = "USD", MaxRate = MaxRate }))
                .AddSingleton(rateApiClientStub.Object)
                .AddSingleton(_notificationRepositoryMock.Object)
                .AddSingleton(_notificationSenderMock.Object)
                .BuildServiceProvider();

            _app = serviceProvider.GetRequiredService<App>();
        }

        [Fact]
        public async Task Should_DoNothing_When_RateLessThenMaxValue()
        {
            _usdRate = 0;

            await _app.Run();

            _notificationSenderMock.Verify(s => s.Send(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Should_SendNotification_When_RateMoreThenMaxValue()
        {
            _usdRate = MaxRate + 1;
            _notificationRepositoryMock.Setup(r => r.SetShown()).Returns(true);

            await _app.Run();

            _notificationSenderMock.Verify(s => s.Send(It.IsAny<string>()));
        }

        [Fact]
        public async Task Should_DoNothing_When_RateMoreThenMaxValueAndNotificationSent()
        {
            _usdRate = MaxRate + 1;            

            await _app.Run();

            _notificationSenderMock.Verify(s => s.Send(It.IsAny<string>()), Times.Never);
        }
    }
}

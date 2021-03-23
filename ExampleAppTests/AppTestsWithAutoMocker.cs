using ExampleApp;
using Microsoft.Extensions.Options;
using Moq;
using Moq.AutoMock;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ExampleAppTests
{

    public class AppTestsWithAutoMocker
    {
        private readonly App _app;
        private readonly AutoMocker _autoMocker = new AutoMocker();
        private decimal _usdRate;
        private const decimal MaxRate = 50;

        public AppTestsWithAutoMocker()
        {
            _autoMocker.GetMock<IRateApiClient>()
                .Setup(c => c.GetRatesAsync())
                .ReturnsAsync(() => new RatesResponse(
                    Valute: new Dictionary<string, Rate> 
                    { 
                     ["USD"] = new Rate("Äמככאנ ÑØÀ", _usdRate)
                    }
                ));

            _autoMocker.Use<IOptions<RateOptions>>(o => o.Value == new RateOptions { Valute = "USD", MaxRate = MaxRate });

            _app = _autoMocker.CreateInstance<App>();
        }

        [Fact]
        public async Task Should_DoNothing_When_RateLessThenMaxValue()
        {
            _usdRate = 0;

            await _app.Run();

            _autoMocker.Verify<INotificationSender>(s => s.Send(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Should_SendNotification_When_RateMoreThenMaxValue()
        {
            _usdRate = MaxRate + 1;
            _autoMocker.GetMock<INotificationRepository>()
                .Setup(r => r.SetShown())
                .Returns(true);

            await _app.Run();

            _autoMocker.Verify<INotificationSender>(s => s.Send(It.IsAny<string>()));
        }

        [Fact]
        public async Task Should_DoNothing_When_RateMoreThenMaxValueAndNotificationSent()
        {
            _usdRate = MaxRate + 1;            

            await _app.Run();

           _autoMocker.Verify<INotificationSender>(s => s.Send(It.IsAny<string>()), Times.Never);
        }
    }
}

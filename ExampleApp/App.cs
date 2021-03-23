using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace ExampleApp
{
    public class App
    {
        private readonly ILogger<App> _logger;
        private readonly IRateApiClient _rateApiClient;
        private readonly INotificationSender _notificationSender;
        private readonly INotificationRepository _notificationRepository;
        private readonly RateOptions _rateOptions;

        public App(ILogger<App> logger, IRateApiClient rateApiClient, INotificationSender notificationSender,
            IOptions<RateOptions> rateOptions, INotificationRepository notificationRepository) 
        {
            _logger = logger;
            _rateApiClient = rateApiClient;
            _notificationSender = notificationSender;
            _notificationRepository = notificationRepository;
            _rateOptions = rateOptions.Value;
        }

        public async Task Run()
        {
            var rates = await _rateApiClient.GetRatesAsync();

            Rate rate = rates.Valute[_rateOptions.Valute];

            _logger.LogInformation($"rate: {rate}");

            if (rate.Value > _rateOptions.MaxRate && _notificationRepository.SetShown()) 
                _notificationSender.Send($"Курс {rate.Name}: {rate.Value} превысил порогвое значение {_rateOptions.MaxRate}");
        }
    }
}

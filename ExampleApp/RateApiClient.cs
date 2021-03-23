using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ExampleApp
{
    public interface IRateApiClient
    {
        Task<RatesResponse> GetRatesAsync();
    }

    /// <summary>
    /// Клиент для API курсов ЦБ РФ
    /// </summary>
    public class RateApiClient : IRateApiClient
    {
        private static readonly HttpClient _httpClient = new();

        /* JSON example
{
    "Valute": {
        "USD": {
            "ID": "R01235",
            "NumCode": "840",
            "CharCode": "USD",
            "Nominal": 1,
            "Name": "Доллар США",
            "Value": 74.139,
            "Previous": 73.6582
        }
        ...
         
         */
        public Task<RatesResponse> GetRatesAsync()
            => _httpClient.GetFromJsonAsync<RatesResponse>("https://www.cbr-xml-daily.ru/daily_json.js")!;
    }

    public record RatesResponse(Dictionary<string, Rate> Valute);

    public record Rate(string Name, decimal Value);
}

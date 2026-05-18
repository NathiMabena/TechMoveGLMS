using System.Text.Json;
using TechMoveGLMS.Services.Interfaces;

namespace TechMoveGLMS.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly HttpClient _httpClient;

        public CurrencyService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<decimal> GetUsdToZarRateAsync()
        {
            try
            {
                // Free API - no key needed
                var response = await _httpClient.GetStringAsync(
                    "https://open.er-api.com/v6/latest/USD");

                var json = JsonDocument.Parse(response);
                var rate = json.RootElement
                    .GetProperty("rates")
                    .GetProperty("ZAR")
                    .GetDecimal();

                return rate;
            }
            catch
            {
                // Fallback rate if API is down
                return 18.50m;
            }
        }
    }
}
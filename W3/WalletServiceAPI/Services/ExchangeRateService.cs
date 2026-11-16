using System.Text.Json;

namespace WalletServiceAPI.Services;

public class ExchangeRateService : IExchangeRateService
{
    private readonly HttpClient _httpClient;

    public ExchangeRateService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<decimal> ConvertAsync(
        decimal amount, string from, string to)
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative.");

        from = from.ToUpperInvariant();
        to = to.ToUpperInvariant();

        var response = await _httpClient.GetFromJsonAsync<JsonElement>(
            "https://open.er-api.com/v6/latest/USD");

        if (response.GetProperty("result").GetString() != "success")
            throw new InvalidOperationException("Exchange rate provider failed.");

        var rates = response.GetProperty("rates");

        if (!rates.TryGetProperty(from, out var fromRate) ||
            !rates.TryGetProperty(to, out var toRate))
            throw new ArgumentException("Unsupported currency.");

        decimal sourceRate = fromRate.GetDecimal();
        decimal targetRate = toRate.GetDecimal();

        return amount / sourceRate * targetRate;
    }
}
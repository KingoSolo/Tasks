namespace WalletServiceAPI.Services;

public interface IExchangeRateService
{
    Task<decimal> ConvertAsync(decimal amount, string from, string to);
}
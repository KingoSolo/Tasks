using Microsoft.AspNetCore.Mvc;
using WalletServiceAPI.Services;

namespace WalletServiceAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CurrencyController : ControllerBase
{
    private readonly IExchangeRateService _exchangeRateService;

    public CurrencyController(IExchangeRateService exchangeRateService)
    {
        _exchangeRateService = exchangeRateService;
    }

    [HttpGet("convert")]
    public async Task<IActionResult> Convert(
        [FromQuery] decimal amount,
        [FromQuery] string from,
        [FromQuery] string to)
    {
        var converted = await _exchangeRateService.ConvertAsync(
            amount, from, to);

        return Ok(new
        {
            amount,
            from,
            to,
            convertedAmount = Math.Round(converted, 2)
        });
    }
    
}
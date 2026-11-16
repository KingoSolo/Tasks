using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WalletServiceAPI.Dto;
using WalletServiceAPI.Services;

namespace WalletServiceAPI.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class WalletController : ControllerBase
{
    private readonly IWalletService _walletService;
    private readonly IExchangeRateService _exchangeRateService;

    public WalletController(IWalletService walletService, IExchangeRateService exchangeRateService)
    {
        _walletService = walletService;
        _exchangeRateService = exchangeRateService;
    }

    private string UserId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new UnauthorizedAccessException();

    [HttpGet("me")]
    public async Task<IActionResult> GetMyWallet()
    {
        var wallet = await _walletService.GetOrCreateWalletAsync(UserId);
        return Ok(wallet);
    }

    [HttpPost("fund")]
    public async Task<IActionResult> Fund(AmountDto dto)
    {
        await _walletService.FundWalletAsync(UserId, dto.Amount);
        return Ok(new { message = "Wallet funded successfully." });
    }

    [HttpPost("debit")]
    public async Task<IActionResult> Debit(AmountDto dto)
    {
        await _walletService.DebitWalletAsync(UserId, dto.Amount);
        return Ok(new { message = "Wallet debited successfully." });
    }

    [HttpPost("transfer")]
    public async Task<IActionResult> Transfer(TransferDto dto)
    {
        await _walletService.TransferAsync(
            UserId,
            dto.ReceiverWalletAddress,
            dto.Amount,
            dto.Description);

        return Ok(new { message = "Transfer completed successfully." });
    }

    [HttpGet("transactions")]
    public async Task<IActionResult> Transactions(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var transactions = await _walletService.GetTransactionsAsync(
            UserId, page, pageSize);

        return Ok(new
        {
            page,
            pageSize,
            transactions
        });
    }
    [HttpGet("balance/convert")]
public async Task<IActionResult> ConvertBalance([FromQuery] string currency)
{
    var wallet = await _walletService.GetOrCreateWalletAsync(UserId);

    var converted = await _exchangeRateService.ConvertAsync(
        wallet.Balance,
        wallet.Currency,
        currency);

    return Ok(new
    {
        wallet.Balance,
        wallet.Currency,
        targetCurrency = currency.ToUpperInvariant(),
        convertedBalance = Math.Round(converted, 2)
    });
}
}
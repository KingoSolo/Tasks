namespace WalletServiceAPI.Dto;

public class WalletResponseDto
{
    public int WalletId { get; set; }
    public string WalletAddress { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public string Currency { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

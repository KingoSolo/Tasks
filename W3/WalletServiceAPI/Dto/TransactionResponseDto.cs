namespace WalletServiceAPI.Dto;

public class TransactionResponseDto
{
    public int TransactionId { get; set; }
    public decimal Amount { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public string ReferenceId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string TransactionStatus { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

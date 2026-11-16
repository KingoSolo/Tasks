using System.ComponentModel.DataAnnotations;

namespace WalletServiceAPI.Dto;

public class TransferDto
{
    [Required]
    public string ReceiverWalletAddress { get; set; } = "";

    [Range(typeof(decimal), "0.01", "1000000000")]
    public decimal Amount { get; set; }

    public string Description { get; set; } = "";
}
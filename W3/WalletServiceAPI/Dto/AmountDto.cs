using System.ComponentModel.DataAnnotations;

namespace WalletServiceAPI.Dto;

public class AmountDto
{
    [Range(typeof(decimal), "0.01", "1000000000")]
    public decimal Amount { get; set; }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccountServiceProvider.Api.Data.Entities;

public class AccountProfileAddressEntity
{
    [Key, ForeignKey(nameof(AccountProfile))]
    public string AccountProfileId { get; set; } = null!;
    public AccountProfileEntity AccountProfile { get; set; } = null!;
    public string StreetName { get; set; } = null!;
    public string PostalCode { get; set; } = null!;
    public string City { get; set; } = null!;
}

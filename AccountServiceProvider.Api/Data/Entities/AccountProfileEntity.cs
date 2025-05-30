using System.ComponentModel.DataAnnotations;

namespace AccountServiceProvider.Api.Data.Entities;

public class AccountProfileEntity
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Phone { get; set; } = null!;

    public AccountProfileAddressEntity? Address { get; set; }
}
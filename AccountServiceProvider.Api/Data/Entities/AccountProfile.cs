using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AccountServiceProvider.Api.Data.Entities;

[Index(nameof(Email), IsUnique = true)]
public class AccountProfile
{
    [Key]
    public string Id { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Phone { get; set; } = null!;

    public AccountProfileAddress? Address { get; set; }
}
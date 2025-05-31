using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AccountServiceProvider.Api.Data.Entities;

/// <summary>
/// Main database entity of a user's Account Profile.
/// </summary>
[Index(nameof(Email), IsUnique = true)]
public class AccountProfile
{
    /// <summary>
    /// The database entity's Id here is the same as the Identity user's ID in the MVC.
    /// </summary>
    [Key]
    public string Id { get; set; } = null!;

    /// <summary>
    /// The database entity's Email here is the same as the Identity user's Email/UserName in the MVC.
    /// </summary>
    public string Email { get; set; } = null!;

    /// <summary>
    /// The user database entity's first name.
    /// </summary>
    public string FirstName { get; set; } = null!;

    /// <summary>
    /// The user database entity's last name.
    /// </summary>
    public string LastName { get; set; } = null!;

    /// <summary>
    /// The user database entity's phone number in international format.
    /// </summary>
    public string Phone { get; set; } = null!;

    /// <summary>
    /// The user database entity's street address, connected to AccountProfileAddress database entity.
    /// </summary>
    public AccountProfileAddress? Address { get; set; }
}
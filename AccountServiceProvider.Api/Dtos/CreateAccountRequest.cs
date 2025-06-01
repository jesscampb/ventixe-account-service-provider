using System.ComponentModel.DataAnnotations;

namespace AccountServiceProvider.Api.Dtos;

/// <summary>
/// Request DTO used to create a new account profile.
/// </summary>
public class CreateAccountRequest
{
    /// <summary>
    /// The unique Identity user ID (GUID) to associate with this profile.
    /// </summary>
    [Required]
    public string UserId { get; set; } = null!;

    /// <summary>
    /// The unique Identity user Email to associate with this profile.
    /// </summary>
    public string Email { get; set; } = null!;

    /// <summary>
    /// The user's first name.
    /// </summary>
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string FirstName { get; set; } = null!;

    /// <summary>
    /// The user's last name.
    /// </summary>
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string LastName { get; set; } = null!;

    /// <summary>
    /// The user's phone number in international format.
    /// </summary>
    [Required]
    [Phone]
    public string Phone { get; set; } = null!;

    /// <summary>
    /// Street name and house number of the user's address.
    /// </summary>
    [Required]
    [StringLength(200)]
    public string StreetName { get; set; } = null!;

    /// <summary>
    /// Postal code of the user's address, consisting of exactly 5 digits.
    /// </summary>
    [Required]
    [RegularExpression(@"^\d{5}$")]
    public string PostalCode { get; set; } = null!;

    /// <summary>
    /// City name of the user's address.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string City { get; set; } = null!;
}

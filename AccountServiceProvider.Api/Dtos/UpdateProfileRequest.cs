using System.ComponentModel.DataAnnotations;

namespace AccountServiceProvider.Api.Dtos;

/// <summary>
/// Request DTO used to update an existing account profile.
/// </summary>
public class UpdateProfileRequest
{
    /// <summary>
    /// The updated first name.
    /// </summary>
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string FirstName { get; set; } = null!;

    /// <summary>
    /// The updated last name.
    /// </summary>
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string LastName { get; set; } = null!;

    /// <summary>
    /// The updated phone number in international format.
    /// </summary>
    [Required]
    [Phone]
    public string Phone { get; set; } = null!;

    /// <summary>
    /// The updated street address.
    /// </summary>
    [Required]
    [StringLength(200)]
    public string StreetName { get; set; } = null!;

    /// <summary>
    /// The updated postal code, existing of exacty 5 digits.
    /// </summary>
    [Required]
    [RegularExpression(@"^\d{5}$")]
    public string PostalCode { get; set; } = null!;

    /// <summary>
    /// The updated city.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string City { get; set; } = null!;
}

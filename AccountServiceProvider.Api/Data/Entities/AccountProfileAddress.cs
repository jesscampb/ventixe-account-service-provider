using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccountServiceProvider.Api.Data.Entities;

/// <summary>
/// Connected Address database entity of a user's Account Profile.
/// </summary>
public class AccountProfileAddress
{
    /// <summary>
    /// The Id of the connected Account Profile database entity.
    /// </summary>
    [Key, ForeignKey(nameof(AccountProfile))]
    public string AccountProfileId { get; set; } = null!;

    /// <summary>
    /// The connected Account Profile database entity.
    /// </summary>
    public AccountProfile AccountProfile { get; set; } = null!;

    /// <summary>
    /// The database entity's street name and house number of the user's address.
    /// </summary>
    public string StreetName { get; set; } = null!;

    /// <summary>
    /// The database entity's postal code of the user's address, consisting of exactly 5 digits.
    /// </summary>
    public string PostalCode { get; set; } = null!;

    /// <summary>
    /// The database entity's city name of the user's address.
    /// </summary>
    public string City { get; set; } = null!;
}

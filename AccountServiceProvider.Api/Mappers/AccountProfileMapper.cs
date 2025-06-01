using AccountServiceProvider.Api.Data.Entities;
using AccountServiceProvider.Api.Dtos;

namespace AccountServiceProvider.Api.Mappers;

public static class AccountProfileMapper
{
    public static AccountProfile ToEntity(this CreateAccountRequest dto)
        => new()
        {
            Id = dto.UserId,
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Phone = dto.Phone,
            Address = new AccountProfileAddress
            {
                StreetName = dto.StreetName,
                PostalCode = dto.PostalCode,
                City = dto.City
            }
        };

    public static void Map(this AccountProfile entity, UpdateProfileRequest dto)
    {
        entity.FirstName = dto.FirstName;
        entity.LastName = dto.LastName;
        entity.Phone = dto.Phone;

        if (entity.Address == null)
            entity.Address = new AccountProfileAddress { AccountProfileId = entity.Id };

        entity.Address.StreetName = dto.StreetName;
        entity.Address.PostalCode = dto.PostalCode;
        entity.Address.City = dto.City;
    }
}

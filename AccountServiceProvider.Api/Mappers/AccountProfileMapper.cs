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

    public static AccountProfileDto ToDto(this AccountProfile entity)
        => new()
        {
            Id = entity.Id,
            Email = entity.Email,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Phone = entity.Phone,
            StreetName = entity.Address?.StreetName ?? string.Empty,
            PostalCode = entity.Address?.PostalCode ?? string.Empty,
            City = entity.Address?.City ?? string.Empty
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

using AccountServiceProvider.Api.Data.Entities;
using AccountServiceProvider.Api.Dtos;
using Azure.Core;

namespace AccountServiceProvider.Api.Mappers;

public static class AccountProfileMapper
{
    public static AccountProfile ToEntity(this CreateAccountRequest request)
        => new()
        {
            Id = request.UserId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Phone = request.Phone,
            Address = new AccountProfileAddress
            {
                StreetName = request.StreetName,
                PostalCode = request.PostalCode,
                City = request.City
            }
        };
}

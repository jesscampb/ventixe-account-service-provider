using AccountServiceProvider.Api.Data.Contexts;
using AccountServiceProvider.Api.Data.Entities;
using AccountServiceProvider.Api.Dtos;
using AccountServiceProvider.Api.Services.Models;
using Microsoft.EntityFrameworkCore;

namespace AccountServiceProvider.Api.Services;

public class AccountService(AccountDbContext context)
{
    private readonly AccountDbContext _context = context;

    public async Task<AccountResult<bool>> ExistsAsync(string email)
    {
        var exists = await _context.Profiles.AnyAsync(x => x.Email == email);

        return new AccountResult<bool>
        {
            Succeeded = true,
            Result = exists,
            Message = exists ? "User exists" : "User does not exist"
        };
    }

    public async Task<AccountResult<AccountProfile>> CreateAsync(CreateAccountRequest request)
    {
        var profile = new AccountProfile
        {
            Email = request.Email,
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

        await _context.Profiles.AddAsync(profile);
        await _context.SaveChangesAsync();

        return new AccountResult<AccountProfile>
        {
            Succeeded = true,
            Result = profile,
            Message = "Account profile created."
        };
    }
}

using AccountServiceProvider.Api.Data.Contexts;
using AccountServiceProvider.Api.Data.Entities;
using AccountServiceProvider.Api.Dtos;
using AccountServiceProvider.Api.Services.Models;
using Microsoft.EntityFrameworkCore;

namespace AccountServiceProvider.Api.Services;

public class AccountService(AccountDbContext context) : IAccountService
{
    private readonly AccountDbContext _context = context;

    public async Task<bool> ExistsAsync(string id)
    {
        return await _context.Profiles.AnyAsync(x => x.Id == id);
    }

    public async Task<AccountResult<AccountProfile>> CreateAsync(CreateAccountRequest request)
    {
        if (await ExistsAsync(request.UserId))
        {
            return new AccountResult<AccountProfile>
            {
                Succeeded = false,
                Result = null!,
                Message = "Account profile already exists."
            };
        }

        var profile = new AccountProfile
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

        await _context.Profiles.AddAsync(profile);
        await _context.SaveChangesAsync();

        return new AccountResult<AccountProfile>
        {
            Succeeded = true,
            Result = profile,
            Message = "Account profile created."
        };
    }

    public async Task<AccountResult<AccountProfile>> GetByIdAsync(string id)
    {
        var profile = await _context.Profiles
            .Include(p => p.Address)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (profile == null)
        {
            return new AccountResult<AccountProfile>
            {
                Succeeded = false,
                Result = null!,
                Message = "Account profile not found."
            };
        }

        return new AccountResult<AccountProfile>
        {
            Succeeded = true,
            Result = profile,
            Message = "Account profile retrieved."
        };
    }

    public async Task<AccountResult<AccountProfile>> UpdateAsync(string id, UpdateProfileRequest request)
    {
        var getProfileResult = await GetByIdAsync(id);

        if (!getProfileResult.Succeeded)
            return getProfileResult;

        var profile = getProfileResult.Result!;

        profile.FirstName = request.FirstName;
        profile.LastName = request.LastName;
        profile.Phone = request.Phone;

        if (profile.Address == null)
            profile.Address = new AccountProfileAddress { AccountProfileId = id };

        profile.Address.StreetName = request.StreetName;
        profile.Address.PostalCode = request.PostalCode;
        profile.Address.City = request.City;


        await _context.SaveChangesAsync();

        return new AccountResult<AccountProfile>
        {
            Succeeded = true,
            Result = profile,
            Message = "Account profile information updated."
        };
    }

    public async Task<AccountResult> DeleteAsync(string id)
    {
        var getProfileResult = await GetByIdAsync(id);

        if (!getProfileResult.Succeeded)
        {
            return new AccountResult
            {
                Succeeded = false,
                Message = getProfileResult.Message
            };
        }

        var profile = getProfileResult.Result!;

        _context.Profiles.Remove(profile);
        await _context.SaveChangesAsync();

        return new AccountResult
        {
            Succeeded = true,
            Message = "Account profile deleted."
        };
    }
}

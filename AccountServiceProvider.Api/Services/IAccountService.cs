using AccountServiceProvider.Api.Data.Entities;
using AccountServiceProvider.Api.Dtos;
using AccountServiceProvider.Api.Services.Models;

namespace AccountServiceProvider.Api.Services;

public interface IAccountService
{
    Task<AccountResult<AccountProfile>> CreateAsync(CreateAccountRequest request);
    Task<AccountResult> DeleteAsync(string id);
    Task<bool> ExistsAsync(string id);
    Task<AccountResult<AccountProfile>> GetByIdAsync(string id);
    Task<AccountResult<AccountProfile>> UpdateAsync(string id, UpdateProfileRequest request);
}
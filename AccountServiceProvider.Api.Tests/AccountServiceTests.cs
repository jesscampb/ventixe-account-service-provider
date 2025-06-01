using AccountServiceProvider.Api.Data.Contexts;
using AccountServiceProvider.Api.Data.Entities;
using AccountServiceProvider.Api.Dtos;
using AccountServiceProvider.Api.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace AccountServiceProvider.Api.Tests;

public class AccountServiceTests : IDisposable
{
    private readonly AccountDbContext _context;
    private readonly AccountService _accountService;

    public AccountServiceTests()
    {
        var options = new DbContextOptionsBuilder<AccountDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AccountDbContext(options);
        _accountService = new AccountService(_context);
    }

    private async Task SeedData(IEnumerable<AccountProfile> profiles)
    {
        await _context.Profiles.AddRangeAsync(profiles);
        await _context.SaveChangesAsync();
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnTrue_WhenProfileExists()
    {
        // Arrange
        var accountId = "existing-id";
        await SeedData(new List<AccountProfile> { new AccountProfile { Id = accountId, Email = "test@test.com", FirstName = "Test", LastName = "User", Phone = "123" } });

        // Act
        var result = await _accountService.ExistsAsync(accountId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnFalse_WhenProfileDoesNotExist()
    {
        // Arrange  
        var accountId = "non-existing-id";

        // Act
        var result = await _accountService.ExistsAsync(accountId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnSuccess_WhenProfileDoesNotExist()
    {
        // Arrange
        var request = new CreateAccountRequest
        {
            UserId = "new-user-id",
            Email = "john.doe@example.com", // Added Email
            FirstName = "John",
            LastName = "Doe",
            Phone = "1234567890",
            StreetName = "123 Main St",
            PostalCode = "12345",
            City = "Testville"
        };

        // Act
        var result = await _accountService.CreateAsync(request);

        // Assert
        Assert.True(result.Succeeded);
        Assert.NotNull(result.Result);
        Assert.Equal(request.UserId, result.Result.Id);
        Assert.Equal(request.Email, result.Result.Email); // Added assertion for Email in result
        Assert.Equal("Account profile created.", result.Message);

        var savedProfile = await _context.Profiles.FindAsync(request.UserId);
        Assert.NotNull(savedProfile);
        Assert.Equal(request.FirstName, savedProfile.FirstName);
        Assert.Equal(request.Email, savedProfile.Email); // Added assertion for Email in saved profile
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnFailure_WhenProfileAlreadyExists()
    {
        // Arrange
        var existingUserId = "existing-user-id";
        // Ensure the seeded data also has an email, as it's now required by the entity
        await SeedData(new List<AccountProfile> { new AccountProfile { Id = existingUserId, Email = "existing@example.com", FirstName = "Old", LastName = "User", Phone = "000" } });

        // The request to create an already existing profile should also include an email
        var request = new CreateAccountRequest { UserId = existingUserId, Email = "another.email@example.com", FirstName = "Test", LastName = "User", Phone = "123", StreetName = "Street", PostalCode = "12345", City = "City" };

        // Act
        var result = await _accountService.CreateAsync(request);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Null(result.Result);
        Assert.Equal("Account profile already exists.", result.Message);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnProfile_WhenProfileExists()
    {
        // Arrange
        var accountId = "existing-id";
        var expectedProfileData = new AccountProfile
        {
            Id = accountId,
            FirstName = "Jane",
            LastName = "Doe",
            Email = "jane@example.com",
            Phone = "0987654321",
            Address = new AccountProfileAddress { AccountProfileId = accountId, StreetName = "456 Oak Ave", PostalCode = "54321", City = "Testburg" }
        };
        await SeedData(new List<AccountProfile> { expectedProfileData });

        // Act
        var result = await _accountService.GetByIdAsync(accountId);

        // Assert
        Assert.True(result.Succeeded);
        Assert.NotNull(result.Result);
        Assert.Equal(accountId, result.Result.Id);
        Assert.Equal(expectedProfileData.Email, result.Result.Email); // Added assertion for Email
        Assert.NotNull(result.Result.Address);
        Assert.Equal(expectedProfileData.Address?.StreetName, result.Result.Address?.StreetName);
        Assert.Equal("Account profile retrieved.", result.Message);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNotFound_WhenProfileDoesNotExist()
    {
        // Arrange
        var accountId = "non-existing-id";

        // Act
        var result = await _accountService.GetByIdAsync(accountId);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Null(result.Result);
        Assert.Equal("Account profile not found.", result.Message);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnSuccess_WhenProfileExists()
    {
        // Arrange
        var accountId = "existing-id";
        var initialProfile = new AccountProfile
        {
            Id = accountId,
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            Phone = "1234567890",
            Address = new AccountProfileAddress { AccountProfileId = accountId, StreetName = "123 Main St", PostalCode = "12345", City = "Testville" }
        };
        await SeedData(new List<AccountProfile> { initialProfile });

        var updateRequest = new UpdateProfileRequest // UpdateProfileRequest does not have Email, so Email won't be updated
        {
            FirstName = "John Updated",
            LastName = "Doe Updated",
            Phone = "1112223333",
            StreetName = "789 Pine St",
            PostalCode = "67890",
            City = "New Testville"
        };

        // Act
        var result = await _accountService.UpdateAsync(accountId, updateRequest);

        // Assert
        Assert.True(result.Succeeded);
        Assert.NotNull(result.Result);
        Assert.Equal(updateRequest.FirstName, result.Result.FirstName);
        Assert.Equal(initialProfile.Email, result.Result.Email); // Email should remain unchanged
        Assert.NotNull(result.Result.Address);
        Assert.Equal(updateRequest.StreetName, result.Result.Address?.StreetName);
        Assert.Equal("Account profile information updated.", result.Message);

        var updatedProfileInDb = await _context.Profiles.Include(p => p.Address).FirstOrDefaultAsync(p => p.Id == accountId);
        Assert.NotNull(updatedProfileInDb);
        Assert.Equal(updateRequest.FirstName, updatedProfileInDb.FirstName);
        Assert.Equal(initialProfile.Email, updatedProfileInDb.Email); // Verify Email in DB is unchanged
        Assert.Equal(updateRequest.StreetName, updatedProfileInDb.Address?.StreetName);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNotFound_WhenProfileDoesNotExist()
    {
        // Arrange
        var accountId = "non-existing-id";
        var updateRequest = new UpdateProfileRequest { FirstName = "Test", LastName = "User", Phone = "123", StreetName = "Street", PostalCode = "12345", City = "City" };

        // Act
        var result = await _accountService.UpdateAsync(accountId, updateRequest);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Null(result.Result);
        Assert.Equal("Account profile not found.", result.Message);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnSuccess_WhenProfileExists()
    {
        // Arrange
        var accountId = "existing-id";
        await SeedData(new List<AccountProfile> { new AccountProfile { Id = accountId, Email = "delete@example.com", FirstName = "Delete", LastName = "Me", Phone = "555" } });

        // Act
        var result = await _accountService.DeleteAsync(accountId);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal("Account profile deleted.", result.Message);

        var deletedProfile = await _context.Profiles.FindAsync(accountId);
        Assert.Null(deletedProfile);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnNotFound_WhenProfileDoesNotExist()
    {
        // Arrange
        var accountId = "non-existing-id";

        // Act
        var result = await _accountService.DeleteAsync(accountId);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal("Account profile not found.", result.Message);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

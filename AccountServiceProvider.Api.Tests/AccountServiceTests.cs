using AccountServiceProvider.Api.Data.Contexts;
using AccountServiceProvider.Api.Data.Entities;
using AccountServiceProvider.Api.Dtos;
using AccountServiceProvider.Api.Services;
using AccountServiceProvider.Api.Services.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace AccountServiceProvider.Api.Tests;

public class AccountServiceTests
{
    private readonly Mock<AccountDbContext> _mockContext;
    private readonly Mock<DbSet<AccountProfile>> _mockProfilesDbSet;
    private readonly AccountService _accountService;

    public AccountServiceTests()
    {
        _mockContext = new Mock<AccountDbContext>(new DbContextOptions<AccountDbContext>());
        _mockProfilesDbSet = new Mock<DbSet<AccountProfile>>();

        _mockContext.Setup(c => c.Profiles).Returns(_mockProfilesDbSet.Object);
        _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1); // Simulate successful save

        _accountService = new AccountService(_mockContext.Object);
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnTrue_WhenProfileExists()
    {
        // Arrange
        var accountId = "existing-id";
        _mockProfilesDbSet.Setup(m => m.AnyAsync(It.IsAny<Expression<Func<AccountProfile, bool>>>(), default))
            .ReturnsAsync(true);

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
        _mockProfilesDbSet.Setup(m => m.AnyAsync(It.IsAny<Expression<Func<AccountProfile, bool>>>(), default))
            .ReturnsAsync(false);

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
            FirstName = "John", 
            LastName = "Doe", 
            Phone = "1234567890", 
            StreetName = "123 Main St", 
            PostalCode = "12345", 
            City = "Testville" 
        };

        _mockProfilesDbSet.Setup(m => m.AnyAsync(It.IsAny<Expression<Func<AccountProfile, bool>>>(), default))
            .ReturnsAsync(false); // Simulate profile does not exist

        _mockProfilesDbSet.Setup(m => m.AddAsync(It.IsAny<AccountProfile>(), default))
            .ReturnsAsync(Mock.Of<EntityEntry<AccountProfile>>());

        // Act
        var result = await _accountService.CreateAsync(request);

        // Assert
        Assert.True(result.Succeeded);
        Assert.NotNull(result.Result);
        Assert.Equal(request.UserId, result.Result.Id);
        Assert.Equal("Account profile created.", result.Message);
        _mockProfilesDbSet.Verify(m => m.AddAsync(It.Is<AccountProfile>(p => p.Id == request.UserId), default), Times.Once);
        _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnFailure_WhenProfileAlreadyExists()
    {
        // Arrange
        var request = new CreateAccountRequest { UserId = "existing-user-id", /* other properties filled similarly */ };
        _mockProfilesDbSet.Setup(m => m.AnyAsync(It.IsAny<Expression<Func<AccountProfile, bool>>>(), default))
            .ReturnsAsync(true); // Simulate profile already exists

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
        var expectedProfile = new AccountProfile 
        { 
            Id = accountId, 
            FirstName = "Jane", 
            LastName = "Doe", 
            Email = "jane@example.com", 
            Phone = "0987654321",
            Address = new AccountProfileAddress { AccountProfileId = accountId, StreetName = "456 Oak Ave", PostalCode = "54321", City = "Testburg" }
        };
        _mockProfilesDbSet.Setup(m => m.FirstOrDefaultAsync(It.IsAny<Expression<Func<AccountProfile, bool>>>(), default))
            .ReturnsAsync(expectedProfile);

        // Act
        var result = await _accountService.GetByIdAsync(accountId);

        // Assert
        Assert.True(result.Succeeded);
        Assert.NotNull(result.Result);
        Assert.Equal(accountId, result.Result.Id);
        Assert.Equal(expectedProfile.Address?.StreetName, result.Result.Address?.StreetName); // Example check for included Address
        Assert.Equal("Account profile retrieved.", result.Message);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNotFound_WhenProfileDoesNotExist()
    {
        // Arrange
        var accountId = "non-existing-id";
        _mockProfilesDbSet.Setup(m => m.FirstOrDefaultAsync(It.IsAny<Expression<Func<AccountProfile, bool>>>(), default))
            .ReturnsAsync((AccountProfile)null!);

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
        var updateRequest = new UpdateProfileRequest 
        { 
            FirstName = "John Updated", 
            LastName = "Doe Updated", 
            Phone = "1112223333", 
            StreetName = "789 Pine St", 
            PostalCode = "67890", 
            City = "New Testville" 
        };
        var existingProfile = new AccountProfile 
        { 
            Id = accountId, 
            FirstName = "John", 
            LastName = "Doe", 
            Phone = "1234567890",
            Address = new AccountProfileAddress { AccountProfileId = accountId, StreetName = "123 Main St", PostalCode = "12345", City = "Testville" }
        };

        _mockProfilesDbSet.Setup(m => m.FirstOrDefaultAsync(It.IsAny<Expression<Func<AccountProfile, bool>>>(), default))
            .ReturnsAsync(existingProfile);

        // Act
        var result = await _accountService.UpdateAsync(accountId, updateRequest);

        // Assert
        Assert.True(result.Succeeded);
        Assert.NotNull(result.Result);
        Assert.Equal(updateRequest.FirstName, result.Result.FirstName);
        Assert.Equal(updateRequest.StreetName, result.Result.Address?.StreetName);
        Assert.Equal("Account profile information updated.", result.Message);
        _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once); // SaveChanges is called by UpdateAsync after GetByIdAsync
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNotFound_WhenProfileDoesNotExist()
    {
        // Arrange
        var accountId = "non-existing-id";
        var updateRequest = new UpdateProfileRequest { /* properties */ };
        _mockProfilesDbSet.Setup(m => m.FirstOrDefaultAsync(It.IsAny<Expression<Func<AccountProfile, bool>>>(), default))
            .ReturnsAsync((AccountProfile)null!);

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
        var existingProfile = new AccountProfile { Id = accountId };

        _mockProfilesDbSet.Setup(m => m.FirstOrDefaultAsync(It.IsAny<Expression<Func<AccountProfile, bool>>>(), default))
            .ReturnsAsync(existingProfile);
        _mockProfilesDbSet.Setup(m => m.Remove(It.IsAny<AccountProfile>()))
            .Returns(Mock.Of<EntityEntry<AccountProfile>>());

        // Act
        var result = await _accountService.DeleteAsync(accountId);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal("Account profile deleted.", result.Message);
        _mockProfilesDbSet.Verify(m => m.Remove(existingProfile), Times.Once);
        _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Exactly(2)); // Once for GetByIdAsync (if it saves), once for DeleteAsync
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnNotFound_WhenProfileDoesNotExist()
    {
        // Arrange
        var accountId = "non-existing-id";
        _mockProfilesDbSet.Setup(m => m.FirstOrDefaultAsync(It.IsAny<Expression<Func<AccountProfile, bool>>>(), default))
            .ReturnsAsync((AccountProfile)null!);

        // Act
        var result = await _accountService.DeleteAsync(accountId);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal("Account profile not found.", result.Message);
    }
}

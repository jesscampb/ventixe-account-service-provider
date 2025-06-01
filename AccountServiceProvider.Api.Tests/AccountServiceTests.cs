using AccountServiceProvider.Api.Data.Contexts;
using AccountServiceProvider.Api.Data.Entities;
using AccountServiceProvider.Api.Dtos;
using AccountServiceProvider.Api.Services;
using AccountServiceProvider.Api.Services.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace AccountServiceProvider.Api.Tests;

public class AccountServiceTests
{
    private readonly Mock<AccountDbContext> _mockContext;
    private readonly AccountService _accountService;

    public AccountServiceTests()
    {
        _mockContext = new Mock<AccountDbContext>(new DbContextOptions<AccountDbContext>());

        _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1); // Simulate successful save

        _accountService = new AccountService(_mockContext.Object);
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnTrue_WhenProfileExists()
    {
        // Arrange
        var accountId = "existing-id";
        var profiles = new List<AccountProfile> { new AccountProfile { Id = accountId, Email = "test@test.com", FirstName = "Test", LastName = "User", Phone = "123" } };
        _mockContext.Setup(x => x.Profiles).ReturnsDbSet(profiles);

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
        var profiles = new List<AccountProfile>();
        _mockContext.Setup(x => x.Profiles).ReturnsDbSet(profiles);

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

        var profiles = new List<AccountProfile>();
        _mockContext.Setup(x => x.Profiles).ReturnsDbSet(profiles);

        // Act
        var result = await _accountService.CreateAsync(request);

        // Assert
        Assert.True(result.Succeeded);
        Assert.NotNull(result.Result);
        Assert.Equal(request.UserId, result.Result.Id);
        Assert.Equal("Account profile created.", result.Message);
        _mockContext.Verify(m => m.Profiles.AddAsync(It.Is<AccountProfile>(p => p.Id == request.UserId), default), Times.Once);
        _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnFailure_WhenProfileAlreadyExists()
    {
        // Arrange
        var request = new CreateAccountRequest { UserId = "existing-user-id", FirstName = "Test", LastName = "User", Phone = "123", StreetName = "Street", PostalCode = "12345", City = "City" };
        var profiles = new List<AccountProfile> { new AccountProfile { Id = "existing-user-id", Email = "existing@test.com", FirstName = "Old", LastName = "User", Phone = "000" } };
        _mockContext.Setup(x => x.Profiles).ReturnsDbSet(profiles);

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
        var profiles = new List<AccountProfile> { expectedProfile };
        _mockContext.Setup(x => x.Profiles).ReturnsDbSet(profiles);

        // Act
        var result = await _accountService.GetByIdAsync(accountId);

        // Assert
        Assert.True(result.Succeeded);
        Assert.NotNull(result.Result);
        Assert.Equal(accountId, result.Result.Id);
        Assert.Equal(expectedProfile.Address?.StreetName, result.Result.Address?.StreetName);
        Assert.Equal("Account profile retrieved.", result.Message);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNotFound_WhenProfileDoesNotExist()
    {
        // Arrange
        var accountId = "non-existing-id";
        var profiles = new List<AccountProfile>();
        _mockContext.Setup(x => x.Profiles).ReturnsDbSet(profiles);

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
            Email = "john@example.com",
            Phone = "1234567890",
            Address = new AccountProfileAddress { AccountProfileId = accountId, StreetName = "123 Main St", PostalCode = "12345", City = "Testville" }
        };
        var profiles = new List<AccountProfile> { existingProfile };
        _mockContext.Setup(x => x.Profiles).ReturnsDbSet(profiles);

        // Act
        var result = await _accountService.UpdateAsync(accountId, updateRequest);

        // Assert
        Assert.True(result.Succeeded);
        Assert.NotNull(result.Result);
        Assert.Equal(updateRequest.FirstName, result.Result.FirstName);
        Assert.Equal(updateRequest.StreetName, result.Result.Address?.StreetName);
        Assert.Equal("Account profile information updated.", result.Message);
        _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNotFound_WhenProfileDoesNotExist()
    {
        // Arrange
        var accountId = "non-existing-id";
        var updateRequest = new UpdateProfileRequest { FirstName = "Test", LastName = "User", Phone = "123", StreetName = "Street", PostalCode = "12345", City = "City" };
        var profiles = new List<AccountProfile>();
        _mockContext.Setup(x => x.Profiles).ReturnsDbSet(profiles);

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
        var existingProfile = new AccountProfile { Id = accountId, Email = "delete@example.com", FirstName = "Delete", LastName = "Me", Phone = "555" };
        var profiles = new List<AccountProfile> { existingProfile };
        _mockContext.Setup(x => x.Profiles).ReturnsDbSet(profiles);

        // Act
        var result = await _accountService.DeleteAsync(accountId);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal("Account profile deleted.", result.Message);
        _mockContext.Verify(m => m.Profiles.Remove(It.Is<AccountProfile>(p => p.Id == accountId)), Times.Once);
        _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnNotFound_WhenProfileDoesNotExist()
    {
        // Arrange
        var accountId = "non-existing-id";
        var profiles = new List<AccountProfile>();
        _mockContext.Setup(x => x.Profiles).ReturnsDbSet(profiles);

        // Act
        var result = await _accountService.DeleteAsync(accountId);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal("Account profile not found.", result.Message);
    }
}

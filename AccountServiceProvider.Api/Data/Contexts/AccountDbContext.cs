using AccountServiceProvider.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AccountServiceProvider.Api.Data.Contexts;

public class AccountDbContext(DbContextOptions<AccountDbContext> options) : DbContext(options)
{
    public DbSet<AccountProfile> Profiles { get; set; }
    public DbSet<AccountProfileAddress> ProfileAddresses { get; set; }
}

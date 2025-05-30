using AccountServiceProvider.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AccountServiceProvider.Api.Data.Contexts;

public class AccountDbContext(DbContextOptions<AccountDbContext> options) : DbContext(options)
{
    public DbSet<AccountProfile> Profiles { get; set; }
    public DbSet<AccountProfileAddress> ProfileAddresses { get; set; }


    // När ett konto raderas från databasen så försvinner även adressen som är kopplad (cascading)
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<AccountProfile>()
            .HasOne(p => p.Address)
            .WithOne(a => a.AccountProfile)
            .HasForeignKey<AccountProfileAddress>(a => a.AccountProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        base.OnModelCreating(modelBuilder);
    }
}

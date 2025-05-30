using Microsoft.EntityFrameworkCore;

namespace AccountServiceProvider.Api.Data.Contexts;

public class AccountDbContext(DbContextOptions<AccountDbContext> options) : DbContext(options)
{
}

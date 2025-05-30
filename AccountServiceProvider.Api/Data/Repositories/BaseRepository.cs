using AccountServiceProvider.Api.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace AccountServiceProvider.Api.Data.Repositories;


public interface IBaseRepository<TEntity> where TEntity : class
{
}

public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
{
    protected readonly AccountDbContext _context;
    protected readonly DbSet<TEntity> _table;

    protected BaseRepository(AccountDbContext context, DbSet<TEntity> table)
    {
        _context = context;
        _table = context.Set<TEntity>();
    }

    public virtual async Task<bool> ExistsAsync(string email)
    {
        var result = await _table.AnyAsync();
        return result;
    }
}
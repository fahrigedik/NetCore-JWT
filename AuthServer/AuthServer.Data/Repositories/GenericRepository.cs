
using System.Linq.Expressions;
using AuthServer.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AuthServer.Data.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly DbContext _context;
    private readonly DbSet<T> _dbSet;

    public GenericRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }
    public async Task<T> GetByIdAsync(int id)
    {
        var entity = await _dbSet.FindAsync(id);

        if (entity is not null)
        {
            _dbSet.Entry(entity).State = EntityState.Detached;
        }

        return entity;
    }


    public async Task<IEnumerable<T>> GetAllAsync()
    {
        var entities = await _dbSet.ToListAsync();
        return entities;
    }

    public IQueryable<T> Where(Expression<Func<T, bool>> predicate)
    {
        return _dbSet.Where(predicate);
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public void Remove(T entity)
    {
        _dbSet.Remove(entity);
    }

    public T Update(T entity)
    { 
       _context.Entry(entity).State = EntityState.Modified;
       var newEntity = _dbSet.Update(entity);
       return newEntity.Entity;

    }
}


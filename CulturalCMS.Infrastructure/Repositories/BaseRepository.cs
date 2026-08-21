using CulturalCMS.Application.Interfaces;
using CulturalCMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CulturalCMS.Infrastructure.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly CulturalDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(CulturalDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public virtual async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);
   
        public virtual Task UpdateAsync(T entity)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            return Task.CompletedTask;
        }

        public virtual async Task<bool> DeleteAsync(int id)
        {
            T? existingEntity = await _dbSet.FindAsync(id);
            if (existingEntity is null) return false;
            _dbSet.Remove(existingEntity);
            return true;
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default) 
            => await _dbSet.ToListAsync(cancellationToken);
       
        public virtual async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
            => await _dbSet.FindAsync(id, cancellationToken);
    }
}

using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CommonServices.Repositories
{
    public class GenericRepository<Db,T> : IGenericRepository<T>
         where Db: DbContext where T : class
    {
        private readonly Db _db;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(Db dbContext)
        {
            _db = dbContext;
            _dbSet = _db.Set<T>();
        }

        /// <summary>Adds a new entity (does not save immediately).</summary>
        public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
        }

        /// <summary>Adds multiple entities (does not save immediately).</summary>
        public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddRangeAsync(entities, cancellationToken);
        }

        /// <summary>Deletes an entity by its primary key.</summary>
        public async Task<bool> DeleteAsync(object id, CancellationToken cancellationToken = default)
        {
            var entity = await _dbSet.FindAsync(new object[] { id }, cancellationToken);
            if (entity == null) return false;

            _dbSet.Remove(entity);
            return true;
        }

        /// <summary>Deletes multiple entities.</summary>
        public void RemoveRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        /// <summary>Finds entities by predicate (read-only).</summary>
        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);
        }

        /// <summary>Retrieves all entities (read-only).</summary>
        public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking().ToListAsync(cancellationToken);
        }

        /// <summary>Retrieves a single entity by primary key.</summary>
        public async Task<T?> GetAsync(object id, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
        }

        /// <summary>Updates an existing entity (does not save immediately).</summary>
        public void Update(T entity)
        {
            _db.Entry(entity).State = EntityState.Modified;
        }

        /// <summary>Saves all pending changes.</summary>
        public async Task<int> SaveAsync(CancellationToken cancellationToken = default)
        {
            return await _db.SaveChangesAsync(cancellationToken);
        }
    }
}

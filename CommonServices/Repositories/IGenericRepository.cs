using System.Linq.Expressions;

namespace CommonServices.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        /// <summary>Adds a new entity (does not save immediately).</summary>
        Task AddAsync(T entity, CancellationToken cancellationToken = default);

        /// <summary>Adds multiple entities (does not save immediately).</summary>
        Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

        /// <summary>Deletes an entity by its primary key.</summary>
        Task<bool> DeleteAsync(object id, CancellationToken cancellationToken = default);

        /// <summary>Deletes multiple entities.</summary>
        void RemoveRange(IEnumerable<T> entities);

        /// <summary>Finds entities by predicate (read-only).</summary>
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        /// <summary>Retrieves all entities (read-only).</summary>
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);

        /// <summary>Retrieves a single entity by primary key.</summary>
        Task<T?> GetAsync(object id, CancellationToken cancellationToken = default);

        /// <summary>Updates an existing entity (does not save immediately).</summary>
        void Update(T entity);

        /// <summary>Saves all pending changes to the database.</summary>
        Task<int> SaveAsync(CancellationToken cancellationToken = default);
    }
}

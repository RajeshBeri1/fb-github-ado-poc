using System.Linq.Expressions;
using Lib.WebAPI.DbModels;
using Lib.WebAPI.DTOs;

namespace Lib.WebAPI.Business.Interfaces
{
    /// <summary>
    /// IModelBaseRepository
    /// </summary>
    public interface IModelBaseRepository<T> where T : DbModelBase
    {
        /// <summary>
        /// Adds the asynchronous.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<T> AddAsync(T item, CancellationToken cancellationToken);

        /// <summary>
        /// Adds the or update asynchronous.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="match">The match.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<T> AddOrUpdateAsync(T item, Expression<Func<T, object>> match, CancellationToken cancellationToken);

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task AddRangeAsync(IEnumerable<T> items, CancellationToken cancellationToken);

        /// <summary>
        /// Anies the asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="predicate">The predicate.</param>
        Task<bool> AnyAsync(CancellationToken cancellationToken, Expression<Func<T, bool>>? predicate = null);

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="start">The start.</param>
        /// <param name="count">The count.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="orderByName">Name of the order by.</param>
        /// <param name="ascending">if set to <c>true</c> [ascending].</param>
        Task<IEnumerable<T>> GetAsync(
            CancellationToken cancellationToken,
            int start = 0, int count = 0,
            Expression<Func<T, bool>>? predicate = null,
            Expression<Func<T, object>>? orderBy = null,
            string? orderByName = null,
            bool ascending = true);

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <typeparam name="TDto">The type of the dto.</typeparam>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="start">The start.</param>
        /// <param name="count">The count.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="orderByName">Name of the order by.</param>
        /// <param name="ascending">if set to <c>true</c> [ascending].</param>
        Task<(ICollection<TDto> Items, int TotalCount)> GetAsync<TDto>(
                    CancellationToken cancellationToken,
                    int start = 0, int count = 0,
                    Expression<Func<T, bool>>? predicate = null,
                    Expression<Func<T, object>>? orderBy = null,
                    string? orderByName = null,
                    bool ascending = true)
                    where TDto : ModelBaseDTO;

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <typeparam name="TDto">The type of the dto.</typeparam>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="start">The start.</param>
        /// <param name="count">The count.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="orderByName">Name of the order by.</param>
        /// <param name="ascending">if set to <c>true</c> [ascending].</param>
        Task<(ICollection<TDto> Items, int TotalCount)> GetAllAsync<TDto>(
                    CancellationToken cancellationToken,
                    int start = 0, int count = 0,
                    Expression<Func<T, bool>>? predicate = null,
                    Expression<Func<T, object>>? orderBy = null,
                    string? orderByName = null,
                    bool ascending = true)
                    where TDto : ModelBaseDTO;

        /// <summary>
        /// Gets the by identifier asynchronous.
        /// </summary>
        /// <typeparam name="TDto">The type of the dto.</typeparam>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<TDto> GetByIdAsync<TDto>(Guid id, CancellationToken cancellationToken)
            where TDto : ModelBaseDTO;

        /// <summary>
        /// Gets the by identifier asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken);

        /// <summary>
        /// Removes the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        void Remove(Expression<Func<T, bool>>? predicate = null);

        /// <summary>
        /// Softs the delete asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<T> SoftDeleteAsync(Guid id, CancellationToken cancellationToken);

        /// <summary>
        /// Gets a collection of all entities.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="removed">The Removed.</param>
        /// <returns>A collection of all entities</returns>
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken, bool removed = false);

        /// <summary>
        /// Softs the restore asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<T> SoftRestoreAsync(Guid id, CancellationToken cancellationToken);
    }
}
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Lib.WebAPI.Business.Interfaces;
using Lib.WebAPI.DbModels;
using Lib.WebAPI.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Lib.WebAPI.Business
{
    /// <summary>
    /// ModelBaseRepository
    /// </summary>
    public class ModelBaseRepository<T> : IModelBaseRepository<T>
        where T : DbModelBase
    {
        /// <summary>
        /// The mapper
        /// </summary>
        protected readonly IMapper mapper;

        private readonly DbSet<T> data;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelBaseRepository{T}" /> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// <param name="mapper">The mapper.</param>
        public ModelBaseRepository(IPortalDbContext dbContext, IMapper mapper)
        {
            data = dbContext.Set<T>();
            this.mapper = mapper;
        }

        /// <summary>
        /// Adds the asynchronous.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<T> AddAsync(T item, CancellationToken cancellationToken)
        {
            return (await data.AddAsync(item, cancellationToken)).Entity;
        }

        /// <summary>
        /// Adds the or update asynchronous.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="match">The match.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<T> AddOrUpdateAsync(T item, Expression<Func<T, object>> match, CancellationToken cancellationToken)
        {
            await data.Upsert(item).On(match).RunAsync(cancellationToken);
            return await GetByIdAsync(item.Id, cancellationToken);
        }

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task AddRangeAsync(IEnumerable<T> items, CancellationToken cancellationToken)
        {
            await data.AddRangeAsync(items, cancellationToken);
        }

        /// <summary>
        /// Anies the asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="predicate">The predicate.</param>
        public async Task<bool> AnyAsync(CancellationToken cancellationToken, Expression<Func<T, bool>>? predicate = null)
        {
            var query = Get();

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return await query.AnyAsync(cancellationToken);
        }

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
        public async Task<(ICollection<TDto> Items, int TotalCount)> GetAsync<TDto>(
            CancellationToken cancellationToken,
            int start = 0, int count = 0,
            Expression<Func<T, bool>>? predicate = null,
            Expression<Func<T, object>>? orderBy = null,
            string? orderByName = null,
            bool ascending = true)
            where TDto : ModelBaseDTO
        {
            var query = Get();

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            if (orderBy != null)
            {
                query = ascending ? query.OrderBy(orderBy) : query.OrderByDescending(orderBy);
            }

            if (!string.IsNullOrWhiteSpace(orderByName))
            {
                query = ascending ? query.OrderBy(orderByName) : query.OrderBy($"{orderByName} desc");
            }

            // default order, when not explicitly ordered
            if ((start > 0 || count > 0) && query.Expression.Type != typeof(IOrderedQueryable<T>))
            {
                query = query.OrderBy(x => x.Id);
            }

            if (start > 0)
            {
                query = query.Skip(start);
            }

            if (count > 0)
            {
                query = query.Take(count);
            }

            var items = await query.AsSingleQuery().ProjectTo<TDto>(mapper.ConfigurationProvider).ToListAsync(cancellationToken);

            return (items, totalCount);
        }

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
        public async Task<(ICollection<TDto> Items, int TotalCount)> GetAllAsync<TDto>(
            CancellationToken cancellationToken,
            int start = 0, int count = 0,
            Expression<Func<T, bool>>? predicate = null,
            Expression<Func<T, object>>? orderBy = null,
            string? orderByName = null,
            bool ascending = true)
            where TDto : ModelBaseDTO
        {
            var query = GetAll();

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            if (orderBy != null)
            {
                query = ascending ? query.OrderBy(orderBy) : query.OrderByDescending(orderBy);
            }

            if (!string.IsNullOrWhiteSpace(orderByName))
            {
                query = ascending ? query.OrderBy(orderByName) : query.OrderBy($"{orderByName} desc");
            }

            // default order, when not explicitly ordered
            if ((start > 0 || count > 0) && query.Expression.Type != typeof(IOrderedQueryable<T>))
            {
                query = query.OrderBy(x => x.Id);
            }

            if (start > 0)
            {
                query = query.Skip(start);
            }

            if (count > 0)
            {
                query = query.Take(count);
            }

            var items = await query.AsSingleQuery().ProjectTo<TDto>(mapper.ConfigurationProvider).ToListAsync(cancellationToken);

            return (items, totalCount);
        }

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
        public async Task<IEnumerable<T>> GetAsync(
            CancellationToken cancellationToken,
            int start = 0, int count = 0,
            Expression<Func<T, bool>>? predicate = null,
            Expression<Func<T, object>>? orderBy = null,
            string? orderByName = null,
            bool ascending = true)
        {
            var query = Get();

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (orderBy != null)
            {
                query = ascending ? query.OrderBy(orderBy) : query.OrderByDescending(orderBy);
            }

            if (!string.IsNullOrWhiteSpace(orderByName))
            {
                query = ascending ? query.OrderBy(orderByName) : query.OrderBy($"{orderByName} desc");
            }

            // default order, when not explicitly ordered
            if ((start > 0 || count > 0) && query.Expression.Type != typeof(IOrderedQueryable<T>))
            {
                query = query.OrderBy(x => x.Id);
            }

            if (start > 0)
            {
                query = query.Skip(start);
            }

            if (count > 0)
            {
                query = query.Take(count);
            }

            var items = await query.AsSingleQuery().ToListAsync(cancellationToken);

            return items;
        }

        /// <summary>
        /// Gets the by identifier asynchronous.
        /// </summary>
        /// <typeparam name="TDto">The type of the dto.</typeparam>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<TDto> GetByIdAsync<TDto>(Guid id, CancellationToken cancellationToken)
            where TDto : ModelBaseDTO
        {
            return await Get().Where(x => x.Id == id).AsSplitQuery().ProjectTo<TDto>(mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync(cancellationToken) ?? throw new KeyNotFoundException($"{typeof(T).Name} {id} not found.");
        }

        /// <summary>
        /// Gets the by identifier asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await Get().Where(x => x.Id == id).AsSplitQuery()
                .FirstOrDefaultAsync(cancellationToken) ?? throw new KeyNotFoundException($"{typeof(T).Name} {id} not found.");
        }

        /// <summary>
        /// Removes the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        public void Remove(Expression<Func<T, bool>>? predicate = null)
        {
            if (predicate != null)
            {
                data.RemoveRange(data.Where(predicate).ToList());
            }
            else
            {
                data.RemoveRange(data.ToList());
            }
        }

        /// <summary>
        /// Softs the delete asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<T> SoftDeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var item = await GetByIdAsync(id, cancellationToken);
            item.Removed = true;
            return item;
        }

        /// <summary>
        /// Gets a collection of all entities.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="removed">The Removed.</param>
        /// <returns>A collection of all entities</returns>
        public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken, bool removed = false)
        {
            return await data.Where(x => x.Removed == removed).ToListAsync();
        }

        /// <summary>
        /// Gets this instance.
        /// </summary>
        protected IQueryable<T> Get()
        {
            return data.Where(x => !x.Removed);
        }


        /// <summary>
        /// Gets this instance.
        /// </summary>
        protected IQueryable<T> GetAll()
        {
            return data;
        }

        /// <summary>
        /// Softs the restore asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<T> SoftRestoreAsync(Guid id, CancellationToken cancellationToken)
        {
            var item = await GetRemovedByIdAsync(id, cancellationToken);
            item.Removed = false;
            return item;
        }

        /// <summary>
        /// Gets the by identifier asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<T> GetRemovedByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await data.Where(x => x.Removed && x.Id == id).AsSplitQuery()
                .FirstOrDefaultAsync(cancellationToken) ?? throw new KeyNotFoundException($"{typeof(T).Name} {id} not found.");
        }
    }
}
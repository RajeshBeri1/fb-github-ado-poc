using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Lib.WebAPI.Business.Interfaces;
using Lib.WebAPI.DbModels;
using Lib.WebAPI.DTOs;
using Lib.WebAPI.Extensions;
using Lib.WebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Lib.WebAPI.Business
{
    /// <summary>
    /// NamedModelBaseRepository
    /// </summary>
    public class NamedModelBaseRepository<T> : ModelBaseRepository<T>, INamedModelBaseRepository<T>
        where T : NamedDbModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NamedModelBaseRepository{T}" />
        /// class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// <param name="mapper">The mapper.</param>
        public NamedModelBaseRepository(IPortalDbContext dbContext, IMapper mapper)
            : base(dbContext, mapper)
        {
        }

        /// <summary>
        /// Gets the by name asynchronous.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<T> GetByNameAsync(string name, CancellationToken cancellationToken)
        {
            return await Get().AsSplitQuery().FirstOrDefaultAsync(x => x.Name == name, cancellationToken)
                ?? throw new KeyNotFoundException($"{typeof(T).Name} with name '{name}' not found.");
        }

        /// <summary>
        /// Gets the by name asynchronous.
        /// </summary>
        /// <typeparam name="TDto">The type of the dto.</typeparam>
        /// <param name="name">The name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<TDto> GetByNameAsync<TDto>(string name, CancellationToken cancellationToken) where TDto : NamedModelBaseDTO
        {
            return await Get().AsSplitQuery().ProjectTo<TDto>(mapper.ConfigurationProvider).FirstOrDefaultAsync(x => x.Name == name, cancellationToken)
                ?? throw new KeyNotFoundException($"{typeof(T).Name} with name '{name}' not found.");
        }

        /// <summary>
        /// Searches the asynchronous.
        /// </summary>
        /// <typeparam name="TDto">The type of the dto.</typeparam>
        /// <param name="namedSearch">The named search.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="predicate">The predicate.</param>
        public async Task<(ICollection<TDto> Items, int TotalCount)> SearchAsync<TDto>(
            NamedSearch namedSearch,
            CancellationToken cancellationToken,
            Expression<Func<T, bool>>? predicate = null)
            where TDto : NamedModelBaseDTO
        {
            Expression<Func<T, bool>> search = x => namedSearch.SearchText == null || x.Name.Contains(namedSearch.SearchText);

            if (predicate != null)
            {
                search = search.AndAlso(predicate);
            }

            return await GetAsync<TDto>(cancellationToken, namedSearch.Start, namedSearch.Count, search,
                orderByName: namedSearch.OrderBy, ascending: namedSearch.OrderAscending);
        }


        /// <summary>
        /// Searches the asynchronous.
        /// </summary>
        /// <typeparam name="TDto">The type of the dto.</typeparam>
        /// <param name="namedSearch">The named search.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="predicate">The predicate.</param>
        public async Task<(ICollection<TDto> Items, int TotalCount)> SearchAllAsync<TDto>(
            NamedSearch namedSearch,
            CancellationToken cancellationToken,
            Expression<Func<T, bool>>? predicate = null)
            where TDto : NamedModelBaseDTO
        {
            Expression<Func<T, bool>> search = x => namedSearch.SearchText == null || x.Name.Contains(namedSearch.SearchText);

            if (predicate != null)
            {
                search = search.AndAlso(predicate);
            }

            return await GetAllAsync<TDto>(cancellationToken, namedSearch.Start, namedSearch.Count, search,
                orderByName: namedSearch.OrderBy, ascending: namedSearch.OrderAscending);
        }
    }
}
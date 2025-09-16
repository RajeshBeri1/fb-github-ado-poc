using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Lib.WebAPI.DbModels;
using Lib.WebAPI.DTOs;
using Lib.WebAPI.Models;

namespace Lib.WebAPI.Business.Interfaces
{
    /// <summary>
    /// INamedModelBaseRepository
    /// </summary>
    public interface INamedModelBaseRepository<T> : IModelBaseRepository<T> where T : NamedDbModelBase
    {
        /// <summary>
        /// Gets the by name asynchronous.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<T> GetByNameAsync(string name, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the by name asynchronous.
        /// </summary>
        /// <typeparam name="TDto">The type of the dto.</typeparam>
        /// <param name="name">The name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<TDto> GetByNameAsync<TDto>(string name, CancellationToken cancellationToken) where TDto : NamedModelBaseDTO;

        /// <summary>
        /// Searches the asynchronous.
        /// </summary>
        /// <typeparam name="TDto">The type of the dto.</typeparam>
        /// <param name="namedSearch">The named search.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="predicate">The predicate.</param>
        Task<(ICollection<TDto> Items, int TotalCount)> SearchAsync<TDto>(
                    NamedSearch namedSearch,
                    CancellationToken cancellationToken,
                    Expression<Func<T, bool>>? predicate = null)
                    where TDto : NamedModelBaseDTO;


        /// <summary>
        /// Searches the asynchronous.
        /// </summary>
        /// <typeparam name="TDto">The type of the dto.</typeparam>
        /// <param name="namedSearch">The named search.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="predicate">The predicate.</param>
        Task<(ICollection<TDto> Items, int TotalCount)> SearchAllAsync<TDto>(
                                                        NamedSearch namedSearch,
                                                        CancellationToken cancellationToken,
                                                        Expression<Func<T, bool>>? predicate = null)
                                                        where TDto : NamedModelBaseDTO;
    }
}
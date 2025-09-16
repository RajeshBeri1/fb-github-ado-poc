using Lib.WebAPI.DbModels;
using Lib.WebAPI.DTOs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.WebAPI.Business.Interfaces
{
    /// <summary>
    /// FieldInfo services interface
    /// </summary>
    public interface IFieldInfoServices
    {
        /// <summary>
        /// Create the CreateFieldInfo asynchronous
        /// </summary>
        /// <param name="fieldInfoDto">fieldInfoDto</param>
        /// <param name="cancellationToken">cancellationToken</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task<bool> CreateFieldInfoAsync(List<FieldInfoDto> fieldInfoDto, CancellationToken cancellationToken);

        /// <summary>
        /// Update the UpdateFieldInfo asynchronous
        /// </summary>
        /// <param name="updateFieldInfoDto">updateFieldInfoDto</param>
        /// <param name="cancellationToken">cancellationToken</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task UpdateFieldInfoAsync(UpdateFieldInfoDto updateFieldInfoDto, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the Fieldinfo list asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="removed">The Removed.</param>
        Task<List<FieldInfo>> GetAllAsync(CancellationToken cancellationToken, bool removed = false);

        /// <summary>
        /// Deletes the asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken);
    }
}

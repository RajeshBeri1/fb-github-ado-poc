using Lib.WebAPI.DbModels;
using Lib.WebAPI.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.WebAPI.Business.Interfaces
{
    /// <summary>
    /// DefaultTemplateServices services interface
    /// </summary>
    public interface IDefaultTemplateServices
    {
        /// <summary>
        /// Create the default templates asynchronous
        /// </summary>
        /// <param name="createDefaultTemplates">createDefaultTemplates</param>
        /// <param name="cancellationToken">cancellationToken</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task<bool> CreateDefaultTemplateAsync(List<DefaultTemplateDTO> createDefaultTemplates, CancellationToken cancellationToken);

        /// <summary>
        /// Update the default Template  asynchronous
        /// </summary>
        /// <param name="updateDefaultTemplate">updateDefaultTemplate</param>
        /// <param name="cancellationToken">cancellationToken</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task<bool> UpdateDefaultTemplateAsync(UpdateDefaultTemplateDTO updateDefaultTemplate, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the default template details list asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="removed">The Removed.</param>
        Task<List<DefaultTemplate>> GetAllAsync(CancellationToken cancellationToken, bool removed = false);

        /// <summary>
        /// Deletes the default template asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken);

        /// <summary>
        /// Check the create or update Default Component async.
        /// </summary>
        /// <param name="createUpdateDefaultComponentDTO">The duplicate name check Dto.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<bool> CreateUpdateDefaultComponentAsync(CreateUpdateDefaultComponentDTO createUpdateDefaultComponentDTO, CancellationToken cancellationToken);
    }
}

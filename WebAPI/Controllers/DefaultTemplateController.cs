using Hangfire;
using Lib.WebAPI.Business;
using Lib.WebAPI.Business.Interfaces;
using Lib.WebAPI.DbModels;
using Lib.WebAPI.DTOs;
using Lib.WebAPI.Enumerations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Diagnostics.CodeAnalysis;

namespace WebAPI.Controllers
{
    /// <summary>
    /// DefaultTemplateController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ExcludeFromCodeCoverage]
    public class DefaultTemplateController : ControllerBase
    {
        private readonly IUserProvider userProvider;
        private readonly IDefaultTemplateServices defaultTemplateServices;
        private readonly DefaultTemplateUpdater defaultTemplateUpdater;
        private readonly IHttpContextAccessor httpContextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultTemplateController" />
        /// class.
        /// </summary>
        /// <param name="userProvider">The user provider.</param>
        /// <param name="defaultTemplateServices">The common services.</param>
        /// <param name="defaultTemplateUpdater">The default template updater.</param>
        /// <param name="httpContextAccessor">The HTTP context accessor.</param>
        public DefaultTemplateController(IUserProvider userProvider, IDefaultTemplateServices defaultTemplateServices, DefaultTemplateUpdater defaultTemplateUpdater, IHttpContextAccessor httpContextAccessor)
        {
            this.userProvider = userProvider;
            this.defaultTemplateServices = defaultTemplateServices;
            this.defaultTemplateUpdater = defaultTemplateUpdater;
            this.httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Create the default templates.
        /// </summary>
        /// <param name="createDefaultTemplates">createDefaultTemplates</param>
        /// <param name="cancellationToken">cancellationToken</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost("[action]")]
        //[EnableRateLimiting("fixed")]
        public async Task<IActionResult> CreateDefaultTemplate(List<DefaultTemplateDTO> createDefaultTemplates, CancellationToken cancellationToken)
        {
            bool isCreated = await defaultTemplateServices.CreateDefaultTemplateAsync(createDefaultTemplates, cancellationToken);
            if (isCreated)
            {
                Guid omniClientId = Guid.Empty;
                bool isUpdate = false;
                // trigger background job to update default template for all clients in the system
                string ansidValue = GetClaimValue(OmniAuthenticationHandler.AuthenticationScheme);
                var backgroundJobCancellation = new CancellationTokenSource(TimeSpan.FromHours(2));
                await defaultTemplateUpdater.RunAsync(ansidValue, backgroundJobCancellation.Token, omniClientId, isUpdate);
                return Ok("Created default template successfully");
            }
            else
            {
                return Ok("Record already exist");
            }
        }

        /// <summary>
        /// Update the default Template.
        /// </summary>
        /// <param name="updateDefaultTemplate">updateDefaultTemplate</param>
        /// <param name="cancellationToken">cancellationToken</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPatch("[action]")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> UpdateDefaultTemplate(UpdateDefaultTemplateDTO updateDefaultTemplate, CancellationToken cancellationToken)
        {
            bool isUpdated = await defaultTemplateServices.UpdateDefaultTemplateAsync(updateDefaultTemplate, cancellationToken);
            if (isUpdated)
            {
                Guid omniClientId = Guid.Empty;
                bool isUpdate = false;
                // trigger background job to update default template for all clients in the system
                string ansidValue = GetClaimValue(OmniAuthenticationHandler.AuthenticationScheme);
                var backgroundJobCancellation = new CancellationTokenSource(TimeSpan.FromHours(2));
                await defaultTemplateUpdater.RunAsync(ansidValue, backgroundJobCancellation.Token, omniClientId, isUpdate);
                return Ok("Defaul template Info data updated successfully");
            }
            else
            {
                return Ok("Failed to update the default template record");
            }
        }

        /// <summary>
        /// Get all the default template details list.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="removed">The Removed.</param>
        [HttpPost("[action]")]
        public async Task<List<DefaultTemplate>> List(CancellationToken cancellationToken, bool removed = false)
        {
            return await defaultTemplateServices.GetAllAsync(cancellationToken, removed);
        }

        /// <summary>
        /// Delete the default template record.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            await defaultTemplateServices.DeleteAsync(id, user.Id, cancellationToken);
            return Ok("Defaul template record deleted successfully");
        }

        /// <summary>
        /// Get all the default template details list.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="isUpdate">The isUpdate.</param>
        [HttpPost("[action]")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> CreateOrUpdateDefaultTemplateForAllClients(CancellationToken cancellationToken, bool isUpdate = false)
        {
            Guid omniClientId = Guid.Empty;
            // trigger background job to update default template for all clients in the system
            string ansidValue = GetClaimValue(OmniAuthenticationHandler.AuthenticationScheme);
            var backgroundJobCancellation = new CancellationTokenSource(TimeSpan.FromHours(2));
            await defaultTemplateUpdater.RunAsync(ansidValue, backgroundJobCancellation.Token, omniClientId, isUpdate);
            return await Task.FromResult<IActionResult>(Ok("Defaul template Info data updated successfully"));
        }

        /// <summary>
        /// Check the create or update Default Component.
        /// </summary>
        /// <param name="createUpdateDefaultComponentDTO">The duplicate name check Dto.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpPost("[action]")]
        public async Task<IActionResult> CreateUpdateDefaultComponent(CreateUpdateDefaultComponentDTO createUpdateDefaultComponentDTO, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            bool isUpdated = await defaultTemplateServices.CreateUpdateDefaultComponentAsync(createUpdateDefaultComponentDTO, cancellationToken);
            if (isUpdated)
            {
                return Ok($"Default Configuration setting for component:{createUpdateDefaultComponentDTO.ComponentName} is set to true and row updated successfully");
            }
            else
            {
                return Ok($"Default Configuration setting is set to false for component:{createUpdateDefaultComponentDTO.ComponentName} is set to false and row updated successfully.");
            }
        }

        private string? GetClaimValue(string key)
        {
            return httpContextAccessor?.HttpContext?.User?.FindFirst(key)?.Value;
        }
    }
}

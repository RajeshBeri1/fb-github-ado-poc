using System.Diagnostics.CodeAnalysis;
using Lib.WebAPI.Business;
using Lib.WebAPI.Business.Interfaces;
using Lib.WebAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    /// <summary>
    /// SummaryTemplateController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ExcludeFromCodeCoverage]
    public class SummaryTemplateController : ControllerBase
    {
        private readonly SummaryTemplateControllerLogic controllerLogic;
        private readonly IUserProvider userProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="SummaryTemplateController" />
        /// class.
        /// </summary>
        /// <param name="controllerLogic">The controller logic.</param>
        /// <param name="userProvider">The user provider.</param>
        public SummaryTemplateController(SummaryTemplateControllerLogic controllerLogic, IUserProvider userProvider)
        {
            this.controllerLogic = controllerLogic;
            this.userProvider = userProvider;
        }

        /// <summary>
        /// Creates the specified summary template.
        /// </summary>
        /// <param name="summaryTemplate">The summary template.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpPost("[action]")]
        public async Task<SummaryTemplateDetailsDTO> Create(SummaryTemplateCreateDTO summaryTemplate, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            return await controllerLogic.CreateAsync(summaryTemplate, user, cancellationToken);
        }

        /// <summary>
        /// Deletes the specified summary template.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpDelete("[action]/{id}")]
        public async Task<SummaryTemplateInfoDTO> Delete(Guid id, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            return await controllerLogic.DeleteAsync(id, user.Id, cancellationToken);
        }

        /// <summary>
        /// Gets the specified summary template.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpGet("[action]/{id}")]
        public async Task<SummaryTemplateDetailsDTO> Get(Guid id, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            return await controllerLogic.GetAsync(id, user.Id, cancellationToken);
        }

        /// <summary>
        /// Lists the specified summary templates.
        /// </summary>
        /// <param name="search">The search.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpPost("[action]")]
        public async Task<SummaryTemplateInfoListDTO> List(SummaryTemplateSearchDTO search, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            return await controllerLogic.GetAsync(search, user.Id, cancellationToken);
        }

        /// <summary>
        /// Updates the specified summary template.
        /// </summary>
        /// <param name="summaryTemplate">The summary template.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpPost("[action]")]
        public async Task<SummaryTemplateDetailsDTO> Update(SummaryTemplateUpdateDTO summaryTemplate, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            return await controllerLogic.UpdateAsync(summaryTemplate, user.Id, cancellationToken);
        }
    }
}
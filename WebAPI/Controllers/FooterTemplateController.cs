using System.Diagnostics.CodeAnalysis;
using Lib.WebAPI.Business.Interfaces;
using Lib.WebAPI.DTOs;
using Lib.WebAPI.Enumerations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    /// <summary>
    /// FooterTemplateController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ExcludeFromCodeCoverage]
    public class FooterTemplateController : ControllerBase
    {
        private readonly IFooterTemplateControllerLogic controllerLogic;
        private readonly IUserProvider userProvider;
        private readonly ICommonServices commonServices;

        /// <summary>
        /// Initializes a new instance of the <see cref="FooterTemplateController" />
        /// class.
        /// </summary>
        /// <param name="controllerLogic">The controller logic.</param>
        /// <param name="userProvider">The user provider.</param>
        /// <param name="commonServices">The common services.</param>
        public FooterTemplateController(IFooterTemplateControllerLogic controllerLogic, IUserProvider userProvider, ICommonServices commonServices)
        {
            this.controllerLogic = controllerLogic;
            this.userProvider = userProvider;
            this.commonServices = commonServices;
        }

        /// <summary>
        /// Creates the footer template.
        /// </summary>
        /// <param name="footerTemplate">The footer template.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpPost("[action]")]
        public async Task<FooterTemplateDetailsDTO> Create(FooterTemplateCreateDTO footerTemplate, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            return await controllerLogic.CreateAsync(footerTemplate, user, cancellationToken);
        }

        /// <summary>
        /// Deletes the footer template.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpDelete("[action]/{id}")]
        public async Task<FooterTemplateInfoDTO> Delete(Guid id, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            return await controllerLogic.DeleteAsync(id, user.Id, cancellationToken);
        }

        /// <summary>
        /// Gets the footer template.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpGet("[action]/{id}")]
        public async Task<FooterTemplateDetailsDTO> Get(Guid id, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            return await controllerLogic.GetAsync(id, user.Id, cancellationToken);
        }

        /// <summary>
        /// Lists the footer templates.
        /// </summary>
        /// <param name="search">The search.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpPost("[action]")]
        public async Task<FooterTemplateInfoListDTO> List(FooterTemplateSearchDTO search, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            return await controllerLogic.GetAsync(search, user.Id, cancellationToken);
        }

        /// <summary>
        /// Updates the footer template.
        /// </summary>
        /// <param name="footerTemplate">The footer template.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpPost("[action]")]
        public async Task<FooterTemplateDetailsDTO> Update(FooterTemplateUpdateDTO footerTemplate, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            return await controllerLogic.UpdateAsync(footerTemplate, user.Id, cancellationToken);
        }

        /// <summary>
        /// Check the duplicate Footer component name.
        /// </summary>
        /// <param name="duplicateNameCheckDto">The duplicate name check Dto.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpPost("[action]")]
        public async Task<bool> CheckDuplicateComponentName(DuplicateNameCheckDto duplicateNameCheckDto, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            duplicateNameCheckDto.ComponentName = FlowChartComponent.Footer;
            return await commonServices.CheckDuplicateComponentNameAsync(duplicateNameCheckDto, user, cancellationToken);
        }

        /// <summary>
        /// Restores the template.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpPost("[action]/{id}")]
        public async Task<FooterTemplateInfoDTO> Restore(Guid id, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            return await controllerLogic.RestoreAsync(id, user.Id, cancellationToken);
        }
    }
}
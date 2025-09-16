using System.Diagnostics.CodeAnalysis;
using Lib.WebAPI.Business;
using Lib.WebAPI.Business.Interfaces;
using Lib.WebAPI.DTOs;
using Lib.WebAPI.Enumerations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    /// <summary>
    /// CalendarOverlayTemplateController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ExcludeFromCodeCoverage]
    public class CalendarOverlayTemplateController : ControllerBase
    {
        private readonly CalendarOverlayTemplateControllerLogic controllerLogic;
        private readonly IUserProvider userProvider;
        private readonly ICommonServices commonServices;

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="CalendarOverlayTemplateController" /> class.
        /// </summary>
        /// <param name="controllerLogic">The controller logic.</param>
        /// <param name="userProvider">The user provider.</param>
        /// <param name="commonServices">The common services.</param>
        public CalendarOverlayTemplateController(CalendarOverlayTemplateControllerLogic controllerLogic, IUserProvider userProvider, ICommonServices commonServices)
        {
            this.controllerLogic = controllerLogic;
            this.userProvider = userProvider;
            this.commonServices = commonServices;
        }

        /// <summary>
        /// Creates the specified overlay template.
        /// </summary>
        /// <param name="overlayTemplate">The overlay template.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpPost("[action]")]
        public async Task<CalendarOverlayTemplateDetailsDTO> Create(CalendarOverlayTemplateCreateDTO overlayTemplate, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            return await controllerLogic.CreateAsync(overlayTemplate, user, cancellationToken);
        }

        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpDelete("[action]/{id}")]
        public async Task<CalendarOverlayTemplateInfoDTO> Delete(Guid id, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            return await controllerLogic.DeleteAsync(id, user.Id, cancellationToken);
        }

        /// <summary>
        /// Gets the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpGet("[action]/{id}")]
        public async Task<CalendarOverlayTemplateDetailsDTO> Get(Guid id, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            return await controllerLogic.GetAsync(id, user.Id, cancellationToken);
        }

        /// <summary>
        /// Lists the specified search.
        /// </summary>
        /// <param name="search">The search.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpPost("[action]")]
        public async Task<CalendarOverlayTemplateInfoListDTO> List(CalendarOverlayTemplateSearchDTO search, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            return await controllerLogic.GetAsync(search, user.Id, cancellationToken);
        }

        /// <summary>
        /// Updates the specified overlay template.
        /// </summary>
        /// <param name="overlayTemplate">The overlay template.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpPost("[action]")]
        public async Task<CalendarOverlayTemplateDetailsDTO> Update(CalendarOverlayTemplateUpdateDTO overlayTemplate, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            return await controllerLogic.UpdateAsync(overlayTemplate, user.Id, cancellationToken);
        }

        /// <summary>
        /// Check the duplicate calendar overlay component name.
        /// </summary>
        /// <param name="duplicateNameCheckDto">The duplicate name check Dto.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpPost("[action]")]
        public async Task<bool> CheckDuplicateComponentName(DuplicateNameCheckDto duplicateNameCheckDto, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            duplicateNameCheckDto.ComponentName = FlowChartComponent.CalendarOverlay;
            return await commonServices.CheckDuplicateComponentNameAsync(duplicateNameCheckDto, user, cancellationToken);
        }

        /// <summary>
        /// Restores the template.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpPost("[action]/{id}")]
        public async Task<CalendarOverlayTemplateInfoDTO> Restore(Guid id, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            return await controllerLogic.RestoreAsync(id, user.Id, cancellationToken);
        }
    }
}
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
    /// ThemeController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ExcludeFromCodeCoverage]
    public class ThemeTemplateController : ControllerBase
    {
        private readonly ThemeTemplateControllerLogic controllerLogic;
        private readonly IUserProvider userProvider;
        private readonly ICommonServices commonServices;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThemeTemplateController" />
        /// class.
        /// </summary>
        /// <param name="controllerLogic">The controller logic.</param>
        /// <param name="userProvider">The user provider.</param>
        /// <param name="commonServices">The common services.</param>
        public ThemeTemplateController(ThemeTemplateControllerLogic controllerLogic, IUserProvider userProvider, ICommonServices commonServices)
        {
            this.controllerLogic = controllerLogic;
            this.userProvider = userProvider;
            this.commonServices = commonServices;
        }

        /// <summary>
        /// Creates the specified theme template.
        /// </summary>
        /// <param name="themeTemplate">The theme template.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpPost("[action]")]
        public async Task<ThemeTemplateDetailsDTO> Create(ThemeTemplateCreateDTO themeTemplate, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            return await controllerLogic.CreateAsync(themeTemplate, user, cancellationToken);
        }

        /// <summary>
        /// Deletes the specified theme template.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpDelete("[action]/{id}")]
        public async Task<ThemeTemplateInfoDTO> Delete(Guid id, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            return await controllerLogic.DeleteAsync(id, user.Id, cancellationToken);
        }

        /// <summary>
        /// Gets the specified theme template.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpGet("[action]/{id}")]
        public async Task<ThemeTemplateDetailsDTO> Get(Guid id, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            return await controllerLogic.GetAsync(id, user.Id, cancellationToken);
        }

        /// <summary>
        /// Lists the theme templates.
        /// </summary>
        /// <param name="search">The search.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpPost("[action]")]
        public async Task<ThemeTemplateInfoListDTO> List(ThemeTemplateSearchDTO search, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            return await controllerLogic.GetAsync(search, user.Id, cancellationToken);
        }

        /// <summary>
        /// Updates the specified theme template.
        /// </summary>
        /// <param name="themeTemplate">The theme template.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpPost("[action]")]
        public async Task<ThemeTemplateDetailsDTO> Update(ThemeTemplateUpdateDTO themeTemplate, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            return await controllerLogic.UpdateAsync(themeTemplate, user.Id, cancellationToken);
        }

        /// <summary>
        /// Check the duplicate Theme component name.
        /// </summary>
        /// <param name="duplicateNameCheckDto">The duplicate name check Dto.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpPost("[action]")]
        public async Task<bool> CheckDuplicateComponentName(DuplicateNameCheckDto duplicateNameCheckDto, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            duplicateNameCheckDto.ComponentName = FlowChartComponent.Theme;
            return await commonServices.CheckDuplicateComponentNameAsync(duplicateNameCheckDto, user, cancellationToken);
        }

        /// <summary>
        /// Restores the template.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpPost("[action]/{id}")]
        public async Task<ThemeTemplateInfoDTO> Restore(Guid id, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            return await controllerLogic.RestoreAsync(id, user.Id, cancellationToken);
        }
    }
}
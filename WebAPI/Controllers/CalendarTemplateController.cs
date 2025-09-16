using System.Diagnostics.CodeAnalysis;
using Lib.WebAPI.Business.Interfaces;
using Lib.WebAPI.DTOs;
using Lib.WebAPI.Enumerations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    /// <summary>
    /// CalendarTemplateController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ExcludeFromCodeCoverage]
    public class CalendarTemplateController : ControllerBase
    {
        private readonly ICalendarTemplateControllerLogic controllerLogic;
        private readonly IUserProvider userProvider;
        private readonly ICommonServices commonServices;

        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarTemplateController" />
        /// class.
        /// </summary>
        /// <param name="controllerLogic">The controller logic.</param>
        /// <param name="userProvider">The user provider.</param>
        /// <param name="commonServices">The common services.</param>
        public CalendarTemplateController(ICalendarTemplateControllerLogic controllerLogic, IUserProvider userProvider, ICommonServices commonServices)
        {
            this.controllerLogic = controllerLogic;
            this.userProvider = userProvider;
            this.commonServices = commonServices;
        }

        /// <summary>
        /// Creates the calendar template.
        /// </summary>
        /// <param name="calendarTemplate">The calendar template.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpPost("[action]")]
        public async Task<CalendarTemplateDetailsDTO> Create(CalendarTemplateCreateDTO calendarTemplate, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            return await controllerLogic.CreateAsync(calendarTemplate, user, cancellationToken);
        }

        /// <summary>
        /// Deletes the calendar template.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpDelete("[action]/{id}")]
        public async Task<CalendarTemplateInfoDTO> Delete(Guid id, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            return await controllerLogic.DeleteAsync(id, user.Id, cancellationToken);
        }

        /// <summary>
        /// Gets the calendar template.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpGet("[action]/{id}")]
        public async Task<CalendarTemplateDetailsDTO> Get(Guid id, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            return await controllerLogic.GetAsync(id, user.Id, cancellationToken);
        }

        /// <summary>
        /// Lists the calendar templates.
        /// </summary>
        /// <param name="search">The search.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpPost("[action]")]
        public async Task<CalendarTemplateInfoListDTO> List(CalendarTemplateSearchDTO search, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            return await controllerLogic.GetAsync(search, user.Id, cancellationToken);
        }

        /// <summary>
        /// Updates the calendar template.
        /// </summary>
        /// <param name="calendarTemplate">The calendar template.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpPost("[action]")]
        public async Task<CalendarTemplateDetailsDTO> Update(CalendarTemplateUpdateDTO calendarTemplate, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            return await controllerLogic.UpdateAsync(calendarTemplate, user.Id, cancellationToken);
        }

        /// <summary>
        /// Check the duplicate calendar component name.
        /// </summary>
        /// <param name="duplicateNameCheckDto">The duplicate name check Dto.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpPost("[action]")]
        public async Task<bool> CheckDuplicateComponentName(DuplicateNameCheckDto duplicateNameCheckDto, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            duplicateNameCheckDto.ComponentName = FlowChartComponent.Calendar;
            return await commonServices.CheckDuplicateComponentNameAsync(duplicateNameCheckDto, user, cancellationToken);
        }

        /// <summary>
        /// Restores the template.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpPost("[action]/{id}")]
        public async Task<CalendarTemplateInfoDTO> Restore(Guid id, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            return await controllerLogic.RestoreAsync(id, user.Id, cancellationToken);
        }
    }
}
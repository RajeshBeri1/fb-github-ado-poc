using System.Diagnostics.CodeAnalysis;
using Lib.WebAPI.Business;
using Lib.WebAPI.Business.Interfaces;
using Lib.WebAPI.DTOs;
using Lib.WebAPI.Models;
using Lib.WebAPI.Models.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    /// <summary>
    /// DataController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ExcludeFromCodeCoverage]
    public class DataController : ControllerBase
    {
        private readonly DataControllerLogic controllerLogic;
        private readonly DataControllerLogicPmds dataControllerLogicPmds;
        private readonly IClientControllerLogic clientControllerLogic;
        private readonly IUserProvider userProvider;
        private readonly SecurityLogic securityLogic;
        private readonly IHttpContextAccessor httpContextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataController" /> class.
        /// </summary>
        /// <param name="controllerLogic">The controller logic.</param>
        /// <param name="dataControllerLogicPmds">The data controller logic pmds.</param>
        /// <param name="clientControllerLogic">The client Controller logic.</param>
        /// <param name="userProvider">The user provider.</param>
        /// <param name="securityLogic">The securityLogic</param>
        /// <param name="httpContextAccessor">The http context accessor.</param>
        public DataController(DataControllerLogic controllerLogic, DataControllerLogicPmds dataControllerLogicPmds, IUserProvider userProvider, IClientControllerLogic clientControllerLogic, SecurityLogic securityLogic, IHttpContextAccessor httpContextAccessor)
        {
            this.controllerLogic = controllerLogic;
            this.dataControllerLogicPmds = dataControllerLogicPmds;
            this.userProvider = userProvider;
            this.clientControllerLogic = clientControllerLogic;
            this.securityLogic = securityLogic;
            this.httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Gets the broadcast calendar.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpGet("[action]/{year}")]
        public async Task<IEnumerable<IEnumerable<CalendarItem>>> GetBroadcastCalendar(long year, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            return await controllerLogic.GetBroadcastCalendarAsync(year, user.Id, cancellationToken);
        }

        /// <summary>
        /// Gets the broadcast calendar.
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpGet("[action]/{startDate}/{endDate}")]
        public async Task<IEnumerable<IEnumerable<CalendarItem>>> GetBroadcastCalendar(DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            return await controllerLogic.GetBroadcastCalendarAsync(startDate, endDate, user.Id, cancellationToken);
        }

        /// <summary>
        /// Gets the client calendar.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <param name="omniClientId">The omni client identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpGet("[action]/{year}/{omniClientId}")]
        public async Task<IEnumerable<IEnumerable<CalendarItem>>> GetClientCalendar(string year, Guid omniClientId, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            return await controllerLogic.GetClientCalendarAsync(year, omniClientId, user.Id, cancellationToken);
        }

        /// <summary>
        /// Gets the client calendar.
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="omniClientId">The omni client identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpGet("[action]/{startDate}/{endDate}/{omniClientId}")]
        public async Task<IEnumerable<IEnumerable<CalendarItem>>> GetClientCalendar(
            DateTime startDate, DateTime endDate, Guid omniClientId, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            return await controllerLogic.GetClientCalendarAsync(startDate, endDate, omniClientId, user.Id, cancellationToken);
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <param name="dataRequest">The data request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpPost("[action]")]
        public async Task<FlowchartData> GetData(DataRequestDTO dataRequest, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            await securityLogic.CheckUserHasClientAccessAsync(dataRequest.OmniClientId, user.Id, cancellationToken);
            var omniClientInfoDTO = await clientControllerLogic.GetAllOmniClientDetailsByOmniClientId(dataRequest.OmniClientId, cancellationToken);
            return omniClientInfoDTO.Version > 1? await dataControllerLogicPmds.GetDataAsync(dataRequest, omniClientInfoDTO.Client.IncludeSourceMediaToolsData, user.Id, cancellationToken) : await controllerLogic.GetDataAsync(dataRequest, omniClientInfoDTO.Client.IncludeSourceMediaToolsData, user.Id, cancellationToken);
        }

        /// <summary>
        /// Gets the distinct data.
        /// </summary>
        /// <param name="distinctData">The distinct data.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpPost("[action]")]
        public async Task<ICollection<object?>> GetDistinctData(DistinctDataDTO distinctData, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            await securityLogic.CheckUserHasClientAccessAsync(distinctData.OmniClientId, user.Id, cancellationToken);
            var omniClientInfoDTO = await clientControllerLogic.GetAllOmniClientDetailsByOmniClientId(distinctData.OmniClientId, cancellationToken);
            string ansidValue = GetClaimValue(OmniAuthenticationHandler.AuthenticationScheme);
            return omniClientInfoDTO.Version > 1 ? await dataControllerLogicPmds.GetDistinctDataAsync(distinctData, omniClientInfoDTO.Client.IncludeSourceMediaToolsData, ansidValue, user.Id, cancellationToken) : await controllerLogic.GetDistinctDataAsync(distinctData, omniClientInfoDTO.Client.IncludeSourceMediaToolsData, user.Id, cancellationToken);
        }

        /// <summary>
        /// Gets the header data.
        /// </summary>
        /// <param name="headerDataRequest">The header data request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpPost("[action]")]
        public async Task<HeaderData> GetHeaderData(HeaderDataRequestDTO headerDataRequest, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            await securityLogic.CheckUserHasClientAccessAsync(headerDataRequest.OmniClientId, user.Id, cancellationToken);
            var omniClientInfoDTO = await clientControllerLogic.GetAllOmniClientDetailsByOmniClientId(headerDataRequest.OmniClientId, cancellationToken);
            return omniClientInfoDTO.Version > 1 ? await dataControllerLogicPmds.GetHeaderDataAsync(headerDataRequest, omniClientInfoDTO.Client.IncludeSourceMediaToolsData, user.Id, cancellationToken) : await controllerLogic.GetHeaderDataAsync(headerDataRequest, omniClientInfoDTO.Client.IncludeSourceMediaToolsData, user.Id, cancellationToken);
        }

        /// <summary>
        /// Gets the summary data.
        /// </summary>
        /// <param name="summaryDataRequest">The summary data request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpPost("[action]")]
        public async Task<SummaryData> GetSummaryData(SummaryDataRequestDTO summaryDataRequest, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            return await controllerLogic.GetSummaryDataAsync(summaryDataRequest, user.Id, cancellationToken);
        }

        /// <summary>
        /// Check the omni guid exists in Planit AMD database.
        /// </summary>
        /// <param name="omniClientId">omniClientId</param>
        /// <param name="cancellationToken">cancellationToken</param>
        [HttpPost("[action]")]
        public async Task<bool> CheckOmniGuidExists(Guid omniClientId, CancellationToken cancellationToken)
        {
            var omniClientInfoDTO = await clientControllerLogic.GetAllOmniClientDetailsByOmniClientId(omniClientId, cancellationToken);
            return await controllerLogic.CheckOmniGuidExistsAsync(omniClientId, omniClientInfoDTO.Client.IncludeSourceMediaToolsData, cancellationToken);
        }

        /// <summary>
        /// Gets the previous level distinct data.
        /// </summary>
        /// <param name="distinctData">The distinct data.</param>
        /// <param name="ansid">The ansid</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpPost("[action]")]
        public async Task<ICollection<DataDTO>> GetPreviousLevelDistinctData(DistinctDataDTO distinctData, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            await securityLogic.CheckUserHasClientAccessAsync(distinctData.OmniClientId, user.Id, cancellationToken);
            var omniClientInfoDTO = await clientControllerLogic.GetAllOmniClientDetailsByOmniClientId(distinctData.OmniClientId, cancellationToken);

            return omniClientInfoDTO.Version > 1 ? await dataControllerLogicPmds.GetPreiousLevelDistinctDataAsync(distinctData, omniClientInfoDTO.Client.IncludeSourceMediaToolsData, user.Id, cancellationToken) : await controllerLogic.GetPreviousLevelDistinctDataAsync(distinctData, omniClientInfoDTO.Client.IncludeSourceMediaToolsData, user.Id, cancellationToken);
        }

        private string? GetClaimValue(string key)
        {
            return httpContextAccessor?.HttpContext?.User?.FindFirst(key)?.Value;
        }
    }
}
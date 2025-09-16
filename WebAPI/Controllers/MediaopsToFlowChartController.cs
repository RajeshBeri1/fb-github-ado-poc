using DocumentFormat.OpenXml.Office2010.Excel;
using Lib.Annalect.Business;
using Lib.MediaopsToFlowChart.Business;
using Lib.MediaopsToFlowChart.DTO;
using Lib.MediaopsToFlowChart.Models;
using Lib.WebAPI.Business;
using Lib.WebAPI.Business.Interfaces;
using Lib.WebAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Diagnostics.CodeAnalysis;

namespace WebAPI.Controllers
{
    /// <summary>
    /// MediaopsToFlowChartController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ExcludeFromCodeCoverage]
    public class MediaopsToFlowChartController : ControllerBase
    {
        private readonly IUserProvider userProvider;
        private readonly MediaopsToFlowChartImportApi mediaopsToFlowChartImportApi;
        private readonly MediaopsControllerLogic mediaopsControllerLogic;
        private readonly AnnalectApiClient apiClient;
        private readonly IClientControllerLogic clientControllerLogic;

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaopsToFlowChartController" /> class.
        /// </summary>
        /// <param name="userProvider">The user provider.</param>
        /// <param name="mediaopsToFlowChartImportApi">The mediaopsToFlowChartImportApi provider.</param>
        /// <param name="mediaopsControllerLogic">The mediaopsControllerLogic.</param>
        /// <param name="apiClient">The apiClient.</param>
        /// <param name="clientControllerLogic">The clientControllerLogic.</param>
        public MediaopsToFlowChartController(
            IUserProvider userProvider,
            MediaopsToFlowChartImportApi mediaopsToFlowChartImportApi,
            MediaopsControllerLogic mediaopsControllerLogic,
            AnnalectApiClient apiClient,
            IClientControllerLogic clientControllerLogic)
        {
            this.userProvider = userProvider;
            this.mediaopsToFlowChartImportApi = mediaopsToFlowChartImportApi;
            this.mediaopsControllerLogic = mediaopsControllerLogic;
            this.apiClient = apiClient;
            this.clientControllerLogic = clientControllerLogic;
        }

        /// <summary>
        /// Get Media Hierarchy Column details by cliendId.
        /// </summary>
        /// <param name="mediaopsColumnSearchDTO">The client omni guid.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpPost("[action]")]
        public async Task<List<FlowchartDataDictionaryColumnDetails>> Get(MediaopsColumnSearchDTO mediaopsColumnSearchDTO, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            var header = Request.Headers[HeaderNames.Authorization].ToString();
            var split = header.Split(" ", 2);
            var client = await clientControllerLogic.GetAsync(Guid.Parse(mediaopsColumnSearchDTO.ClientId), user.Id, cancellationToken);
            var data = await mediaopsControllerLogic.GetTableAndColumnDetailsFromMediaopsAsync(mediaopsColumnSearchDTO, client, cancellationToken);
            return data;
        }
    }
}

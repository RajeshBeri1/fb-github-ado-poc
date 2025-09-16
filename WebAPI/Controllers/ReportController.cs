using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;
using Lib.WebAPI.Business;
using Lib.WebAPI.Business.Interfaces;
using Lib.WebAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    /// <summary>
    /// ReportController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ExcludeFromCodeCoverage]
    public class ReportController : ControllerBase
    {
        private readonly ReportControllerLogic controllerLogic;
        private readonly IUserProvider userProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportController" /> class.
        /// </summary>
        /// <param name="controllerLogic">The controller logic.</param>
        /// <param name="userProvider">The user provider.</param>
        public ReportController(ReportControllerLogic controllerLogic, IUserProvider userProvider)
        {
            this.controllerLogic = controllerLogic;
            this.userProvider = userProvider;
        }

        /// <summary>
        /// Deletes the specified report.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpDelete("[action]/{id}")]
        public async Task<ReportInfoDTO> Delete(Guid id, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            return await controllerLogic.DeleteAsync(id, user.Id, cancellationToken);
        }

        /// <summary>
        /// Downloads the specified report.
        /// </summary>
        /// <param name="id">The report identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpGet("[action]/{id}")]
        [Produces(MediaTypeNames.Application.Octet)]
        public async Task<IActionResult> Download(Guid id, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            var (output, fileName) = await controllerLogic.DownloadAsync(id, user.Id, cancellationToken);
            return File(output, MediaTypeNames.Application.Octet, fileName);
        }

        /// <summary>
        /// Downloads the specified latest Flowchart template report by flowChartTemplateId.
        /// </summary>
        /// <param name="id">The flowchart identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpGet("[action]/{id}")]
        [Produces(MediaTypeNames.Application.Octet)]
        public async Task<IActionResult> DownloadByFlowChartTemplateId(Guid id, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            var (output, fileName) = await controllerLogic.DownloadByFlowChartTemplateId(id, user.Id, cancellationToken);
            return File(output, MediaTypeNames.Application.Octet, fileName);
        }

        /// <summary>
        /// Gets the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpGet("[action]/{id}")]
        public async Task<ReportInfoDTO> Get(Guid id, CancellationToken cancellationToken)
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
        public async Task<ReportInfoListDTO> List(ReportSearchDTO search, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            return await controllerLogic.GetAsync(search.FlowchartTemplateId, search, user.Id, cancellationToken);
        }

        /// <summary>
        /// Publishes the specified report.
        /// </summary>
        /// <param name="reportPublish">The report publish.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpPost("[action]")]
        public async Task<ReportInfoDTO> Publish(ReportPublishDTO reportPublish, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            return await controllerLogic.PublishAsync(reportPublish, user, cancellationToken);
        }

        /// <summary>
        /// Restores the flowchart.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpPost("[action]/{id}")]
        public async Task<FlowchartTemplateInfoDTO> RestoreFlowchart(Guid id, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            return await controllerLogic.RestoreFlowchartAsync(id, user, cancellationToken);
        }
    }
}
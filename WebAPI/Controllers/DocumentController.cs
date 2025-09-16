using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;
using Lib.WebAPI.Business;
using Lib.WebAPI.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    /// <summary>
    /// DocumentController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ExcludeFromCodeCoverage]
    public class DocumentController : ControllerBase
    {
        private readonly DocumentControllerLogic controllerLogic;
        private readonly IUserProvider userProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentController" /> class.
        /// </summary>
        /// <param name="controllerLogic">The controller logic.</param>
        /// <param name="userProvider">The user provider.</param>
        public DocumentController(DocumentControllerLogic controllerLogic,IUserProvider userProvider)
        {
            this.controllerLogic = controllerLogic;
            this.userProvider = userProvider;
        }

        /// <summary>
        /// Generates the excel document run.
        /// </summary>
        /// <param name="runConfigurationId">The run configuration identifier.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpGet("[action]/{runConfigurationId}/{fileName}")]
        [Produces(MediaTypeNames.Application.Octet)]
        // TODO #937 Implement call with bearer token in frontend to be able to remove [AllowAnonymous]
        //[AllowAnonymous]
        public async Task<IActionResult> GenerateExcelDocumentRun(Guid runConfigurationId, string fileName, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            try
            {
                var output = await controllerLogic.GenerateExcelDocumentRunAsync(runConfigurationId, user.Id, fileName,cancellationToken);
                return File(output, MediaTypeNames.Application.Octet, fileName);
            }
            catch (FileNotFoundException ex)
            {
                return NotFound("No run configuration exists for this template");
            }
        }

        /// <summary>
        /// Generates the excel document template.
        /// </summary>
        /// <param name="flowchartTemplateId">The flowchart template identifier.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpGet("[action]/{flowchartTemplateId}/{fileName}")]
        [Produces(MediaTypeNames.Application.Octet)]
        //[AllowAnonymous]
        public async Task<IActionResult> GenerateExcelDocumentTemplate(Guid flowchartTemplateId, string fileName, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            var output = await controllerLogic.GenerateExcelDocumentTemplateAsync(flowchartTemplateId, user.Id, fileName, cancellationToken);
            return File(output, MediaTypeNames.Application.Octet, fileName);
        }

        /// <summary>
        /// Generates the excel document by report Id.
        /// </summary>
        /// <param name="reportId">The report identifier.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpGet("[action]/{reportId}/{fileName}")]
        [Produces(MediaTypeNames.Application.Octet)]
        [AllowAnonymous]
        public async Task<IActionResult> GenerateExcelDocumentReportId(Guid reportId, string fileName, CancellationToken cancellationToken)
        {
            //var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            var (output, reportName) = await controllerLogic.GenerateExcelDocumentReportIdAsync(reportId, fileName, cancellationToken);
            return File(output, MediaTypeNames.Application.Octet, reportName);
        }

        /// <summary>
        /// Generates the excel document by flowchartTemplateVersionHistoryId.
        /// </summary>
        /// <param name="flowchartTemplateVersionHistoryId">The flowchartTemplateVersionHistory identifier.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpGet("[action]/{flowchartTemplateVersionHistoryId}/{fileName}")]
        [Produces(MediaTypeNames.Application.Octet)]
        //[AllowAnonymous]
        public async Task<IActionResult> GenerateExcelDocumentByFlowchartTemplateVersionHistoryId(Guid flowchartTemplateVersionHistoryId, string fileName, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            var (output, reportName) = await controllerLogic.GenerateExcelDocumentByFlowchartTemplateVersionHistoryIdAsync(flowchartTemplateVersionHistoryId, user.Id, fileName, cancellationToken);
            return File(output, MediaTypeNames.Application.Octet, reportName);
        }
    }
}
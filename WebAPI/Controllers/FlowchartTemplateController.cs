using System.Diagnostics.CodeAnalysis;
using Lib.Annalect.Business;
using Lib.WebAPI.Business;
using Lib.WebAPI.Business.Interfaces;
using Lib.WebAPI.DTOs;
using Lib.WebAPI.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace WebAPI.Controllers
{
    /// <summary>
    /// FlowchartTemplateController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ExcludeFromCodeCoverage]
    public class FlowchartTemplateController : ControllerBase
    {
        private readonly IFlowchartTemplateControllerLogic controllerLogic;
        private readonly RunConfigurationControllerLogic runConfigLogic;
        private readonly IUserProvider userProvider;
        private readonly AnnalectApiClient annalectApiClient;
        private readonly SecurityLogic securityLogic;
        private readonly IHttpContextAccessor httpContextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="FlowchartTemplateController" />
        /// class.
        /// </summary>
        /// <param name="controllerLogic">The controller logic.</param>
        /// <param name="runConfigLogic">The runconfig logic</param>
        /// <param name="userProvider">The user provider.</param>
        /// <param name="annalectApiClient">The annalectApiClient.</param>
        /// <param name="securityLogic">The securityLogic</param>
        /// <param name="httpContextAccessor">The httpContextAccessor</param>
        public FlowchartTemplateController(IFlowchartTemplateControllerLogic controllerLogic, RunConfigurationControllerLogic runConfigLogic, IUserProvider userProvider, AnnalectApiClient annalectApiClient, SecurityLogic securityLogic, IHttpContextAccessor httpContextAccessor)
        {
            this.controllerLogic = controllerLogic;
            this.runConfigLogic = runConfigLogic;
            this.userProvider = userProvider;
            this.annalectApiClient = annalectApiClient;
            this.securityLogic = securityLogic;
            this.httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Creates a new template.
        /// </summary>
        /// <param name="flowchartTemplate">The flowchart template.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpPost("[action]")]
        public async Task<FlowchartTemplateDetailsDTO> Create(FlowchartTemplateCreateDTO flowchartTemplate, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            var template = await controllerLogic.CreateAsync(flowchartTemplate, user, cancellationToken);

            return template;
        }

        /// <summary>
        /// Deletes the template.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpDelete("[action]/{id}")]
        public async Task<FlowchartTemplateInfoDTO> Delete(Guid id, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            return await controllerLogic.DeleteAsync(id, user.Id, cancellationToken);
        }

        /// <summary>
        /// Gets the template by id.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpGet("[action]/{id}")]
        public async Task<FlowchartTemplateDetailsDTO> Get(Guid id, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            return await controllerLogic.GetAsync(id, user.Id, cancellationToken);
        }

        /// <summary>
        /// Lists the templates.
        /// </summary>
        /// <param name="search">The search.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpPost("[action]")]
        public async Task<FlowchartTemplateInfoListDTO> List(FlowchartTemplateSearchDTO search, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            return await controllerLogic.GetAsync(search, user.Id, cancellationToken);
        }

        /// <summary>
        /// Updates the template.
        /// </summary>
        /// <param name="update">The update.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpPost("[action]")]
        public async Task<FlowchartTemplateDetailsDTO> Update(FlowchartTemplateUpdateDTO update, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            return await controllerLogic.UpdateAsync(update, user.Id, cancellationToken);
        }

        /// <summary>
        /// Search the template details from Flowchart Template Version history table.
        /// </summary>
        /// <param name="search">The search.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpPost("[action]")]
        public async Task<FlowchartTemplateDetailsDTO> Search(FlowchartTemplateVersionSearchDTO search, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            return await controllerLogic.SearchAsync(search, user.Id, cancellationToken);
        }

        /// <summary>
        /// Check the duplicate template name.
        /// </summary>
        /// <param name="flowchartTemplate">The flowchart template.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpPost("[action]")]
        public async Task<bool> CheckDuplicateTemplateName(FlowchartTemplateDuplicateCheckDTO flowchartTemplate, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            return await controllerLogic.CheckDuplicateTemplateNameAsync(flowchartTemplate, user, cancellationToken);
        }

        /// <summary>
        /// Update the flowchart template name by using flowchart template Id .
        /// </summary>
        /// <param name="updateflowchartTemplateName">The flowchart template.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpPost("[action]")]
        public async Task<string> UpdateFlowChartTemplateNameById(UpdateFlowchartTemplateNameDto updateflowchartTemplateName, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            return await controllerLogic.UpdateFlowChartTemplateNameByIdAsync(updateflowchartTemplateName, user.Id, cancellationToken);
        }

        /// <summary>
        /// Restores the template.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpPost("[action]/{id}")]
        public async Task<FlowchartTemplateInfoDTO> Restore(Guid id, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            return await controllerLogic.RestoreAsync(id, user.Id, cancellationToken);
        }

        /// <summary>
        /// Get the flowchart template component names with client list.
        /// </summary>
        /// <param name="flowchartShareTemplateInfoDTO">The flowchartShareTemplateInfoDTO.</param>
        /// <param name="cancellationToken">The cancellation token.</param>// FlowchartTemplateShareInfoDTO
        [HttpPost("[action]")]
        public async Task<FlowchartTemplateShareInfoDTO> FlowchartTemplateShareInfo(FlowchartShareTemplateInfoDTO flowchartShareTemplateInfoDTO, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            var header = Request.Headers[HeaderNames.Authorization].ToString();
            var split = header.Split(" ", 2);
            var data = await annalectApiClient.GetOtherClientListAsync(flowchartShareTemplateInfoDTO.OmniClientId, split[1], cancellationToken);
            var clientList = data != null ? data.Select(x => new ClientDetails() { ClientId = x.ClientId, Name = x.Name, }).ToList() : new List<ClientDetails>();
            var result = await controllerLogic.FlowchartTemplateShareInfoAsync(flowchartShareTemplateInfoDTO.TemplateId, clientList, user.Id, cancellationToken);
            return result;
        }


        /// <summary>
        /// Save flowchart share template.
        /// </summary>
        /// <param name="flowChartTemplateShareDetailsDTO">The flowChartTemplateShareDetailsDTO.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpPost("[action]")]
        public async Task<string> SaveSharetemplate(FlowChartTemplateShareDetailsDTO flowChartTemplateShareDetailsDTO, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            await securityLogic.CheckUserHasClientAccessAsync(flowChartTemplateShareDetailsDTO.CurrentOmniClientId, user.Id, cancellationToken);
            string ansidValue = GetClaimValue(OmniAuthenticationHandler.AuthenticationScheme);
            var result = await controllerLogic.SaveSharetemplatetAsync(flowChartTemplateShareDetailsDTO, ansidValue, user.Id, cancellationToken);
            return result ? "Share template created successfully" : "Share template creation failed";
        }

        /// <summary>
        /// Save As flowchart template.
        /// </summary>
        /// <param name="saveFlowchartDTO">The Save Flowchart template.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpPost("[action]")]
        public async Task<ReportInfoDTO> SaveAs(SaveFlowchartDTO saveFlowchartDTO, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            var template = await controllerLogic.SaveAsAsync(saveFlowchartDTO, user, cancellationToken);
            return template;
        }

        /// <summary>
        /// Get Default FlowchartTemplate list By Template Name.
        /// </summary>
        /// <param name="templateName">The search.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpPost("[action]")]
        public async Task<List<SharedFlowchartTemplateDetails>> GetDefaultFlowchartTemplateListByTemplateName(string templateName, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            return await controllerLogic.GetFlowchartTemplateListByTemplateNameAsync(templateName, cancellationToken);
        }

        private string? GetClaimValue(string key)
        {
            return httpContextAccessor?.HttpContext?.User?.FindFirst(key)?.Value;
        }
    }
}
using Lib.WebAPI.DbModels;
using Lib.WebAPI.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Lib.WebAPI.Business.Interfaces
{
    /// <summary>
    /// IFlowchartTemplateControllerLogic
    /// </summary>
    public interface IFlowchartTemplateControllerLogic
    {
        /// <summary>
        /// Creates the asynchronous.
        /// </summary>
        /// <param name="flowchartTemplate">The flowchart template.</param>
        /// <param name="user">The user.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<FlowchartTemplateDetailsDTO> CreateAsync(
            FlowchartTemplateCreateDTO flowchartTemplate, UserDetailsDTO user, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes the asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<FlowchartTemplateInfoDTO> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <param name="search">The search.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<FlowchartTemplateInfoListDTO> GetAsync(FlowchartTemplateSearchDTO search, Guid userId, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <param name="search">The search.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<FlowchartTemplateDetailsDTO> SearchAsync(FlowchartTemplateVersionSearchDTO search, Guid userId, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<FlowchartTemplateDetailsDTO> GetAsync(Guid id, Guid userId, CancellationToken cancellationToken);

        /*/// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <param name="reportId">The identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<FlowchartTemplateDetailsDTO> GetFlowchartByReportIdAsync(Guid reportId, Guid userId, CancellationToken cancellationToken);*/

        /// <summary>
        /// Updates the asynchronous.
        /// </summary>
        /// <param name="update">The update.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<FlowchartTemplateDetailsDTO> UpdateAsync(
            FlowchartTemplateUpdateDTO update, Guid userId, CancellationToken cancellationToken);

        /// <summary>
        /// Check the duplicate template name asynchronous.
        /// </summary>
        /// <param name="flowchartTemplate">The flowchart template.</param>
        /// <param name="user">The user.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<bool> CheckDuplicateTemplateNameAsync(
            FlowchartTemplateDuplicateCheckDTO flowchartTemplate, UserDetailsDTO user, CancellationToken cancellationToken);

        /// <summary>
        /// Update the flowchart template name by using flowchart template Id asynchronous.
        /// </summary>
        /// <param name="updateflowchartTemplateName">The UpdateFlowchartTemplateNameDto template.</param>
        /// <param name="userId">The userId.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<string> UpdateFlowChartTemplateNameByIdAsync(
            UpdateFlowchartTemplateNameDto updateflowchartTemplateName, Guid userId, CancellationToken cancellationToken);

        /// <summary>
        /// Restores the asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<FlowchartTemplateInfoDTO> RestoreAsync(Guid id, Guid userId, CancellationToken cancellationToken);

        /// <summary>
        /// Get the flowchart template share info asynchronous.
        /// </summary>
        /// <param name="templateId">The templateId.</param>
        /// <param name="clientList">The clientList.</param>
        /// <param name="userId">The userId.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<FlowchartTemplateShareInfoDTO> FlowchartTemplateShareInfoAsync(Guid templateId, List<ClientDetails> clientList, Guid userId, CancellationToken cancellationToken);

        /// <summary>
        /// Save flowchart share template asynchronous.
        /// </summary>
        /// <param name="flowChartTemplateShareDetailsDTO">The flowChartTemplateShareDetailsDTO.</param>
        /// <param name="ansid">The ansid.</param>
        /// <param name="userId">The userId.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="isUpdate">The isUpdate.</param>
        Task<bool> SaveSharetemplatetAsync(FlowChartTemplateShareDetailsDTO flowChartTemplateShareDetailsDTO, string ansid, Guid userId, CancellationToken cancellationToken, bool isUpdate = false);

        /// <summary>
        /// Save As flowchart template asynchronous.
        /// </summary>
        /// <param name="saveFlowchartDTO">The saveFlowchartDTO.</param>
        /// <param name="user">The user.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<ReportInfoDTO> SaveAsAsync(SaveFlowchartDTO saveFlowchartDTO, UserDetailsDTO user, CancellationToken cancellationToken);

        /// <summary>
        /// Update default template by flowchart template name asynchronous.
        /// </summary>
        /// <param name="defaultTemplates">The userId.</param>
        /// <param name="ansid">The ansid.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="omniClientId">The omniClientId</param>
        /// <param name="isUpdate">The isUpdate.</param>
        Task<bool> UpdateDefaultTemplateAsync(FlowchartTemplate defaultTemplates, string ansid, CancellationToken cancellationToken, Guid? omniClientId, bool isUpdate = false);

        /// <summary>
        /// Get All default flowchart template list asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<List<FlowchartTemplate>> GetAllDefaultFlowchartTemplateListAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Update the default template for all clients asynchronous.
        /// </summary>
        /// <param name="ansid">The ansid.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="omniClientId">The omniClientId</param>
        /// <param name="isUpdate">The isUpdate.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task UpdateDefaultTemplateForAllClientAsync(string ansid, CancellationToken cancellationToken, Guid? omniClientId, bool isUpdate = false);


        /// <summary>
        /// Get Default FlowchartTemplate list By Template Name asynchronous.
        /// </summary>
        /// <param name="templateName">The search.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<List<SharedFlowchartTemplateDetails>> GetFlowchartTemplateListByTemplateNameAsync(string templateName, CancellationToken cancellationToken);
    }
}
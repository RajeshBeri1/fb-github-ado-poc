using Lib.WebAPI.DbModels;
using Lib.WebAPI.DTOs;

namespace Lib.WebAPI.Business.Interfaces
{
    /// <summary>
    /// Migration ControllerLogic interface
    /// </summary>
    public interface IMigrationControllerLogic
    {
        /// <summary>
        /// Create the migration data asynchronous.
        /// </summary>
        /// <param name="createPlannedMediaMigrationDTOs">createPlannedMediaMigrationDTOs</param>
        /// <param name="cancellationToken">cancellationToken</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task<bool> CreateMigrationAsync(List<CreatePlannedMediaMigrationDTO> createPlannedMediaMigrationDTOs, CancellationToken cancellationToken);

        /// <summary>
        /// Update the migration data asynchronous
        /// </summary>
        /// <param name="updatePlannedMediaMigrationDTO">updatePlannedMediaMigrationDTO</param>
        /// <param name="cancellationToken">cancellationToken</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task UpdateMigrationAsync(UpdatePlannedMediaMigrationDTO updatePlannedMediaMigrationDTO, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the migration list asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="removed">The Removed.</param>
        Task<List<PlannedMediaMigrationColumnMapping>> GetAllAsync(CancellationToken cancellationToken, bool removed = false);

        /// <summary>
        /// Deletes the migration asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken);

        /// <summary>
        /// Migrate flowchart defintion by using omniGuid asynchronous.
        /// </summary>
        /// <param name="omniClientId">The omniClientId identifier.</param>
        /// <param name="connectionId">The connectionId.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<bool> MigrateClientToNewVersionAsync(Guid omniClientId, string connectionId, CancellationToken cancellationToken);

        /// <summary>
        /// Migrate flowchart defintion to V1 by using omniGuid asynchronous.
        /// </summary>
        /// <param name="omniClientId">The omniClientId identifier.</param>
        /// /// <param name="connectionId">The connectionId.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<bool> MigrateClientToInitialVersionAsync(Guid omniClientId, string connectionId, CancellationToken cancellationToken);

        /// <summary>
        /// Get MediaHierarchy Column Uses Details asynchronous.
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="omniClientId">The omniClientId identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<MediaHierarchyColumnsDetailsResultDTO> GetMediaHierarchyColumnUsesDetailsAsync(string? name, Guid? omniClientId, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the updated header details for PMDS.
        /// </summary>
        /// <param name="res">The header definition details DTO.</param>
        /// <param name="ht">The Header template</param>
        /// <param name="clientversion">The client version.</param>
        /// <param name="mappingDetails">The mapping details.</param>
        /// <param name="duplicates">The duplicates</param>
        /// <param name="cancellationToken">The cancellationToken</param>
        /// <param name="omniClientId">The OmniClientId</param>
        Task<HeaderTemplate> GetUpdatedHeaderForPMDS(HeaderDefinitionDetailsDTO res, HeaderTemplate ht, int clientversion, List<PlannedMediaMigrationColumnMapping> mappingDetails, List<string>? duplicates, CancellationToken cancellationToken, Guid? omniClientId = null);

        /// <summary>
        /// Gets the updated MediaHierarchy details for PMDS.
        /// </summary>
        /// <param name="res">The MediaHierarchy definition details DTO.</param>
        /// <param name="mht"> The Media Hierarchy template</param>
        /// <param name="clientversion">The client version.</param>
        /// <param name="mappingDetails">The mapping details.</param>
        /// <param name="duplicates">The duplicates</param>
        /// <param name="cancellationToken">The cancellationToken</param>
        /// <param name="omniClientId">The OmniClientId</param>
        Task<MediaHierarchyTemplate> GetUpdatedMediaHierarchyForPMDS(MediaHierarchyDefinitionDetailsDTO res, MediaHierarchyTemplate mht, int clientversion, List<PlannedMediaMigrationColumnMapping> mappingDetails, List<string>? duplicates, CancellationToken cancellationToken, Guid? omniClientId = null);

        /// <summary>
        /// Gets the updated right hand totals details for PMDS.
        /// </summary>
        /// <param name="res">The right hand totals definition details DTO.</param>
        /// <param name="tt">The Total template</param>
        /// <param name="clientversion">The client version.</param>
        /// <param name="mappingDetails">The mapping details.</param>
        /// <param name="duplicates">The duplicates</param>
        /// <param name="cancellationToken">The cancellationToken</param>
        /// <param name="omniClientId">The OmniClientId</param>
        Task<TotalsTemplate> GetUpdatedRightHandTotalsForPMDS(RightHandTotalsDefinitionDetailsDTO res, TotalsTemplate tt, int clientversion, List<PlannedMediaMigrationColumnMapping> mappingDetails, List<string>? duplicates, CancellationToken cancellationToken, Guid? omniClientId = null);

        /// <summary>
        /// Gets the updated grand total details for PMDS.
        /// </summary>
        /// <param name="res">The grand total definition details DTO.</param>
        /// <param name="gtt">The Grand Total template</param>
        /// <param name="clientversion">The client version.</param>
        /// <param name="mappingDetails">The mapping details.</param>
        /// <param name="duplicates">The duplicates</param>
        /// <param name="cancellationToken">The cancellationToken</param>
        /// <param name="omniClientId">The OmniClientId</param>
        Task<GrandTotalTemplate> GetUpdatedGrandTotalForPMDS(GrandTotalDefintionDetailsDTO res, GrandTotalTemplate gtt, int clientversion, List<PlannedMediaMigrationColumnMapping> mappingDetails, List<string>? duplicates, CancellationToken cancellationToken, Guid? omniClientId = null);

        /// <summary>
        /// Gets the updated themes details for PMDS.
        /// </summary>
        /// <param name="themes">The theme definition details DTO.</param>
        /// <param name="th">The theme template</param>
        /// <param name="clientversion">The client version.</param>
        /// <param name="mappingDetails">The mapping details.</param>
        /// <param name="duplicates">The duplicates</param>
        /// <param name="cancellationToken">The cancellationToken</param>
        /// <param name="omniClientId">The OmniClientId</param>
        Task<ThemeTemplate> GetUpdatedThemeTemplateForPMDS(ThemeDefinitionDetailsDTO themes, ThemeTemplate th, int clientversion, List<PlannedMediaMigrationColumnMapping> mappingDetails, List<string>? duplicates, CancellationToken cancellationToken, Guid? omniClientId = null);

        /// <summary>
        /// Gets the updated flowcharts details for PMDS.
        /// </summary>
        /// <param name="flowcharts">The flowchart definition details DTO.</param>
        /// <param name="ft">The Flowchart template</param>
        /// <param name="clientversion">The client version.</param>
        /// <param name="mappingDetails">The mapping details.</param>
        /// <param name="duplicates">The duplicates</param>
        /// <param name="cancellationToken">The cancellationToken</param>
        /// <param name="omniClientId">The OmniClientId</param>
        Task<FlowchartTemplate> GetUpdatedFlowchartForPMDS(FlowchartDefinitionDetailsDTO flowcharts, FlowchartTemplate ft, int clientversion, List<PlannedMediaMigrationColumnMapping> mappingDetails, List<string>? duplicates, CancellationToken cancellationToken, Guid? omniClientId = null);

        /// <summary>
        /// Gets the updated flowchartVersionHistorties details for PMDS.
        /// </summary>
        /// <param name="flowchartVersionHistorties">The flowchartVersionHistorties definition details DTO.</param>
        /// <param name="ftv">The flowchart template version history</param>
        /// <param name="clientversion">The client version.</param>
        /// <param name="mappingDetails">The mapping details.</param>
        /// <param name="duplicates">The duplicates</param>
        /// <param name="cancellationToken">The cancellationToken</param>
        /// <param name="omniClientId">The OmniClientId</param>
        Task<FlowchartTemplatesVersionHistorty> GetUpdatedFlowchartTemplatesVersionHistortiesForPMDS(FlowchartTemplatesVersionHistortiesDefinitionDetailsDTO flowchartVersionHistorties, FlowchartTemplatesVersionHistorty ftv, int clientversion, List<PlannedMediaMigrationColumnMapping> mappingDetails, List<string>? duplicates, CancellationToken cancellationToken, Guid? omniClientId = null);

        /// <summary>
        /// Gets the updated RunConfiguration details for PMDS.
        /// </summary>
        /// <param name="res">The RunConfiguration definition details DTO.</param>
        /// <param name="rc">The Run configuration</param>
        /// <param name="clientversion">The client version.</param>
        /// <param name="mappingDetails">The mapping details.</param>
        /// <param name="duplicates">The duplicates</param>
        /// <param name="cancellationToken">The cancellationToken</param>
        /// <param name="omniClientId">The OmniClientId</param>
        Task<RunConfiguration> GetUpdatedRunConfigurationForPMDS(RunConfigurationDefinitionDetailsDTO res, RunConfiguration rc, int clientversion, List<PlannedMediaMigrationColumnMapping> mappingDetails, List<string>? duplicates, CancellationToken cancellationToken, Guid? omniClientId = null);
    }
}

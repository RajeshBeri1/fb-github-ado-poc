using Lib.WebAPI.DbModels;

namespace Lib.WebAPI.Business.Interfaces
{
    /// <summary>
    /// IPortalUnitOfWork
    /// </summary>
    public interface IPortalUnitOfWork
    {
        /// <summary>
        /// Gets or sets the broadcast calendars.
        /// </summary>
        /// <value>The broadcast calendars.</value>
        public IModelBaseRepository<BroadcastCalendar> BroadcastCalendars { get; set; }

        /// <summary>
        /// Gets or sets the calendar overlay templates.
        /// </summary>
        /// <value>The calendar overlay templates.</value>
        public INamedModelBaseRepository<CalendarOverlayTemplate> CalendarOverlayTemplates { get; set; }

        /// <summary>
        /// Gets or sets the calendar templates.
        /// </summary>
        /// <value>The calendar templates.</value>
        public INamedModelBaseRepository<CalendarTemplate> CalendarTemplates { get; set; }

        /// <summary>
        /// Gets or sets the client calendars.
        /// </summary>
        /// <value>The client calendars.</value>
        public IModelBaseRepository<ClientCalendar> ClientCalendars { get; set; }

        /// <summary>
        /// Gets or sets the client column aliases.
        /// </summary>
        /// <value>The client column aliases.</value>
        public IModelBaseRepository<ClientColumnAlias> ClientColumnAliases { get; set; }

        /// <summary>
        /// Gets or sets the clients.
        /// </summary>
        /// <value>The clients.</value>
        public INamedModelBaseRepository<Client> Clients { get; set; }

        /// <summary>
        /// Gets or sets the data dictionary tables.
        /// </summary>
        /// <value>The data dictionary tables.</value>
        public INamedModelBaseRepository<DataDictionaryTable> DataDictionaryTables { get; set; }

        /// <summary>
        /// Gets or sets the templates.
        /// </summary>
        /// <value>The templates.</value>
        public INamedModelBaseRepository<FlowchartTemplate> FlowchartTemplates { get; set; }

        /// <summary>
        /// Gets or sets the FlowchartTemplatesVersionHistorties.
        /// </summary>
        /// <value>The FlowchartTemplatesVersionHistorties.</value>
        public INamedModelBaseRepository<FlowchartTemplatesVersionHistorty> FlowchartTemplatesVersionHistorties { get; set; }

        /// <summary>
        /// Gets or sets the footer templates.
        /// </summary>
        /// <value>The footer templates.</value>
        public INamedModelBaseRepository<FooterTemplate> FooterTemplates { get; set; }

        /// <summary>
        /// Gets or sets the grand total templates.
        /// </summary>
        /// <value>The grand total templates.</value>
        public INamedModelBaseRepository<GrandTotalTemplate> GrandTotalTemplates { get; set; }

        /// <summary>
        /// Gets or sets the header templates.
        /// </summary>
        /// <value>The header templates.</value>
        public INamedModelBaseRepository<HeaderTemplate> HeaderTemplates { get; set; }

        /// <summary>
        /// Gets or sets the media hierarchy templates.
        /// </summary>
        /// <value>The media hierarchy templates.</value>
        public INamedModelBaseRepository<MediaHierarchyTemplate> MediaHierarchyTemplates { get; set; }

        /// <summary>
        /// Gets or sets the omni clients.
        /// </summary>
        /// <value>The omni clients.</value>
        public IModelBaseRepository<OmniClient> OmniClients { get; set; }

        /// <summary>
        /// Gets or sets the reports.
        /// </summary>
        /// <value>The reports.</value>
        public INamedModelBaseRepository<Report> Reports { get; set; }

        /// <summary>
        /// Gets or sets the run configurations.
        /// </summary>
        /// <value>The run configurations.</value>
        public INamedModelBaseRepository<RunConfiguration> RunConfigurations { get; set; }

        /// <summary>
        /// Gets or sets the summary templates.
        /// </summary>
        /// <value>The summary templates.</value>
        public INamedModelBaseRepository<SummaryTemplate> SummaryTemplates { get; set; }

        /// <summary>
        /// Gets or sets the theme templates.
        /// </summary>
        /// <value>The theme templates.</value>
        public INamedModelBaseRepository<ThemeTemplate> ThemeTemplates { get; set; }

        /// <summary>
        /// Gets or sets the totals templates.
        /// </summary>
        /// <value>The totals templates.</value>
        public INamedModelBaseRepository<TotalsTemplate> TotalsTemplates { get; set; }

        /// <summary>
        /// Gets or sets the users.
        /// </summary>
        /// <value>The users.</value>
        public INamedModelBaseRepository<User> Users { get; set; }

        /// <summary>
        /// Gets or sets the users.
        /// </summary>
        /// <value>The users.</value>
        public INamedModelBaseRepository<FieldInfo> FieldInfos { get; set; }

        /// <summary>
        /// Gets or sets the plannedMediaToTestSchemaMappings.
        /// </summary>
        /// <value>The plannedMediaToTestSchemaMappings.</value>
        public IModelBaseRepository<PlannedMediaMigrationColumnMapping> PlannedMediaMigrationColumnMappings { get; set; }

        /// <summary>
        /// Gets or sets the ClientMapping.
        /// </summary>
        /// <value>The ClientMapping.</value>
        public IModelBaseRepository<ClientMapping> ClientMapping { get; set; }

        /// <summary>
        /// Gets or sets the DefaultTemplates.
        /// </summary>
        /// <value>The DefaultTemplates.</value>
        public INamedModelBaseRepository<DefaultTemplate> DefaultTemplates { get; set; }

        /// <summary>
        /// Saves the changes asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
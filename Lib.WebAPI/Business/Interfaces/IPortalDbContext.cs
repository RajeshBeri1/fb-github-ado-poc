using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lib.WebAPI.DbModels;
using Microsoft.EntityFrameworkCore;

namespace Lib.WebAPI.Business.Interfaces
{
    /// <summary>
    /// IPortalDbContext
    /// </summary>
    public interface IPortalDbContext
    {
        /// <summary>
        /// Gets or sets the reports.
        /// </summary>
        /// <value>The reports.</value>
        public DbSet<Report> Reports { get; set; }

        /// <summary>
        /// Gets or sets the FlowchartTemplates.
        /// </summary>
        /// <value>The FlowchartTemplates.</value>
        public DbSet<FlowchartTemplate> FlowchartTemplates { get; set; }

        /// <summary>
        /// Gets or sets the Flowchart templates Version Histories.
        /// </summary>
        /// <value>The Flowchart templates version Histories.</value>
        public DbSet<FlowchartTemplatesVersionHistorty> FlowchartTemplatesVersionHistorties { get; set; }

        /// <summary>
        /// Gets or sets the run configurations.
        /// </summary>
        /// <value>The run configurations.</value>
        public DbSet<RunConfiguration> RunConfigurations { get; set; }

        /// <summary>
        /// Gets or sets the FieldInfos.
        /// </summary>
        /// <value>The FieldInfos.</value>
        public DbSet<FieldInfo> FieldInfos { get; set; }

        /// <summary>
        /// Gets or sets the OmniClients.
        /// </summary>
        /// <value>The OmniClients.</value>
        public DbSet<OmniClient> OmniClients { get; set; }


        /// <summary>
        /// Gets or sets the Clients.
        /// </summary>
        /// <value>The Clients.</value>
        public DbSet<Client> Clients { get; set; }

        /// <summary>
        /// Gets or sets the CalendarTemplates.
        /// </summary>
        /// <value>The CalendarTemplates.</value>
        public DbSet<CalendarTemplate> CalendarTemplates { get; set; }

        /// <summary>
        /// Gets or sets the CalendarOverlayTemplates.
        /// </summary>
        /// <value>The CalendarOverlayTemplates.</value>
        public DbSet<CalendarOverlayTemplate> CalendarOverlayTemplates { get; set; }

        /// <summary>
        /// Gets or sets the MediaHierarchyTemplates.
        /// </summary>
        /// <value>The MediaHierarchyTemplates.</value>
        public DbSet<MediaHierarchyTemplate> MediaHierarchyTemplates { get; set; }

        /// <summary>
        /// Gets or sets the HeaderTemplates.
        /// </summary>
        /// <value>The HeaderTemplates.</value>
        public DbSet<HeaderTemplate> HeaderTemplates { get; set; }

        /// <summary>
        /// Gets or sets the ThemeTemplates.
        /// </summary>
        /// <value>The ThemeTemplates.</value>
        public DbSet<ThemeTemplate> ThemeTemplates { get; set; }

        /// <summary>
        /// Gets or sets the FooterTemplates.
        /// </summary>
        /// <value>The FooterTemplates.</value>
        public DbSet<FooterTemplate> FooterTemplates { get; set; }

        /// <summary>
        /// Gets or sets the GrandTotalTemplates.
        /// </summary>
        /// <value>The GrandTotalTemplates.</value>
        public DbSet<GrandTotalTemplate> GrandTotalTemplates { get; set; }

        /// <summary>
        /// Gets or sets the TotalsTemplates.
        /// </summary>
        /// <value>The TotalsTemplates.</value>
        public DbSet<TotalsTemplate> TotalsTemplates { get; set; }


        /// <summary>
        /// Gets or sets the plannedMediaToTestSchemaMappings.
        /// </summary>
        /// <value>The plannedMediaToTestSchemaMappings.</value>
        public DbSet<PlannedMediaMigrationColumnMapping> PlannedMediaMigrationColumnMappings { get; set; }

        /// <summary>
        /// Gets or sets the ClientMapping.
        /// </summary>
        /// <value>The ClientMapping.</value>
        public DbSet<ClientMapping> ClientMapping { get; set; }

        /// <summary>
        /// Gets or sets the DataDictionaryTables.
        /// </summary>
        /// <value>The DataDictionaryTables.</value>
        public DbSet<DataDictionaryTable> DataDictionaryTables { get; set; }

        /// <summary>
        /// Gets or sets the DefaultTemplates.
        /// </summary>
        /// <value>The DefaultTemplates.</value>
        public DbSet<DefaultTemplate> DefaultTemplates { get; set; }

        /// <summary>
        /// Gets or sets the users.
        /// </summary>
        /// <value>The users.</value>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// Saves the changes asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Sets this instance.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
    }
}
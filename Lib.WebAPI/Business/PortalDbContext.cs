using System.Diagnostics.CodeAnalysis;
using Lib.Athena.Consts;
using Lib.WebAPI.Business.Interfaces;
using Lib.WebAPI.DbModels;
using Microsoft.EntityFrameworkCore;

namespace Lib.WebAPI.Business
{
    /// <summary>
    /// PortalDbContext
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class PortalDbContext : DbContext, IPortalDbContext
    {
        /// <summary>
        /// Gets or sets the broadcast calendars.
        /// </summary>
        /// <value>The broadcast calendars.</value>
        public DbSet<BroadcastCalendar> BroadcastCalendars { get; set; } = default!;

        /// <summary>
        /// Gets or sets the calendar overlay templates.
        /// </summary>
        /// <value>The calendar overlay templates.</value>
        public DbSet<CalendarOverlayTemplate> CalendarOverlayTemplates { get; set; } = default!;

        /// <summary>
        /// Gets or sets the calendar templates.
        /// </summary>
        /// <value>The calendar templates.</value>
        public DbSet<CalendarTemplate> CalendarTemplates { get; set; } = default!;

        /// <summary>
        /// Gets or sets the client calendars.
        /// </summary>
        /// <value>The client calendars.</value>
        public DbSet<ClientCalendar> ClientCalendars { get; set; } = default!;

        /// <summary>
        /// Gets or sets the client column aliases.
        /// </summary>
        /// <value>The client column aliases.</value>
        public DbSet<ClientColumnAlias> ClientColumnAliases { get; set; } = default!;

        /// <summary>
        /// Gets or sets the clients.
        /// </summary>
        /// <value>The clients.</value>
        public DbSet<Client> Clients { get; set; } = default!;

        /// <summary>
        /// Gets or sets the data dictionary tables.
        /// </summary>
        /// <value>The data dictionary tables.</value>
        public DbSet<DataDictionaryTable> DataDictionaryTables { get; set; } = default!;

        /// <summary>
        /// Gets or sets the templates.
        /// </summary>
        /// <value>The templates.</value>
        public DbSet<FlowchartTemplate> FlowchartTemplates { get; set; } = default!;

        /// <summary>
        /// Gets or sets the Flowchart templates Version.
        /// </summary>
        /// <value>The Flowchart templates versions.</value>
        public DbSet<FlowchartTemplatesVersionHistorty> FlowchartTemplatesVersionHistorties { get; set; } = default!;

        /// <summary>
        /// Gets or sets the footer templates.
        /// </summary>
        /// <value>The footer templates.</value>
        public DbSet<FooterTemplate> FooterTemplates { get; set; } = default!;

        /// <summary>
        /// Gets or sets the grand total templates.
        /// </summary>
        /// <value>The grand total templates.</value>
        public DbSet<GrandTotalTemplate> GrandTotalTemplates { get; set; } = default!;

        /// <summary>
        /// Gets or sets the header templates.
        /// </summary>
        /// <value>The header templates.</value>
        public DbSet<HeaderTemplate> HeaderTemplates { get; set; } = default!;

        /// <summary>
        /// Gets or sets the media hierarchy templates.
        /// </summary>
        /// <value>The media hierarchy templates.</value>
        public DbSet<MediaHierarchyTemplate> MediaHierarchyTemplates { get; set; } = default!;

        /// <summary>
        /// Gets or sets the omni clients.
        /// </summary>
        /// <value>The omni clients.</value>
        public DbSet<OmniClient> OmniClients { get; set; } = default!;

        /// <summary>
        /// Gets or sets the reports.
        /// </summary>
        /// <value>The reports.</value>
        public DbSet<Report> Reports { get; set; } = default!;

        /// <summary>
        /// Gets or sets the run configurations.
        /// </summary>
        /// <value>The run configurations.</value>
        public DbSet<RunConfiguration> RunConfigurations { get; set; } = default!;

        /// <summary>
        /// Gets or sets the summary templates.
        /// </summary>
        /// <value>The summary templates.</value>
        public DbSet<SummaryTemplate> SummaryTemplates { get; set; } = default!;

        /// <summary>
        /// Gets or sets the theme templates.
        /// </summary>
        /// <value>The theme templates.</value>
        public DbSet<ThemeTemplate> ThemeTemplates { get; set; } = default!;

        /// <summary>
        /// Gets or sets the totals templates.
        /// </summary>
        /// <value>The totals templates.</value>
        public DbSet<TotalsTemplate> TotalsTemplates { get; set; } = default!;

        /// <summary>
        /// Gets or sets the users.
        /// </summary>
        /// <value>The users.</value>
        public DbSet<User> Users { get; set; } = default!;

        /// <summary>
        /// Gets or sets the FieldInfos.
        /// </summary>
        /// <value>The FieldInfos.</value>
        public DbSet<FieldInfo> FieldInfos { get; set; } = default!;

        /// <summary>
        /// Gets or sets the PlannedMediaToTestSchemaMappings.
        /// </summary>
        /// <value>The PlannedMediaToTestSchemaMappings.</value>
        public DbSet<PlannedMediaMigrationColumnMapping> PlannedMediaMigrationColumnMappings { get; set; } = default!;

        /// <summary>
        /// Gets or sets the ClientMapping.
        /// </summary>
        /// <value>The ClientMapping.</value>
        public DbSet<ClientMapping> ClientMapping { get; set; } = default!;

        /// <summary>
        /// Gets or sets the DefaultTemplates.
        /// </summary>
        /// <value>The DefaultTemplates.</value>
        public DbSet<DefaultTemplate> DefaultTemplates { get; set; } = default!;

        /// <summary>
        /// Initializes a new instance of the <see cref="PortalDbContext" /> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public PortalDbContext(DbContextOptions<PortalDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// <para>
        /// Override this method to configure the database (and other options) to be used
        /// for this context. This method is called for each instance of the context that
        /// is created. The base implementation does nothing.
        /// </para>
        /// <para>
        /// In situations where an instance of <see
        /// cref="T:Microsoft.EntityFrameworkCore.DbContextOptions" /> may or may not have
        /// been passed to the constructor, you can use <see
        /// cref="P:Microsoft.EntityFrameworkCore.DbContextOptionsBuilder.IsConfigured" />
        /// to determine if the options have already been set, and skip some or all of the
        /// logic in <see
        /// cref="M:Microsoft.EntityFrameworkCore.DbContext.OnConfiguring(Microsoft.EntityFrameworkCore.DbContextOptionsBuilder)"
        /// />.
        /// </para>
        /// </summary>
        /// <param name="optionsBuilder">
        /// A builder used to create or modify options for this context. Databases (and
        /// other extensions) typically define extension methods on this object that allow
        /// you to configure the context.
        /// </param>
        /// <remarks>
        /// See <see href="https://aka.ms/efcore-docs-dbcontext">DbContext lifetime,
        /// configuration, and initialization</see> for more information.
        /// </remarks>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();

            base.OnConfiguring(optionsBuilder);
        }

        /// <summary>
        /// Override this method to further configure the model that was discovered by
        /// convention from the entity types exposed in <see
        /// cref="T:Microsoft.EntityFrameworkCore.DbSet`1" /> properties on your derived
        /// context. The resulting model may be cached and re-used for subsequent
        /// instances of your derived context.
        /// </summary>
        /// <param name="modelBuilder">
        /// The builder being used to construct the model for this context. Databases (and
        /// other extensions) typically define extension methods on this object that allow
        /// you to configure aspects of the model that are specific to a given database.
        /// </param>
        /// <remarks>
        /// <para>
        /// If a model is explicitly set on the options for this context (via <see
        /// cref="M:Microsoft.EntityFrameworkCore.DbContextOptionsBuilder.UseModel(Microsoft.EntityFrameworkCore.Metadata.IModel)"
        /// />) then this method will not be run.
        /// </para>
        /// <para>
        /// See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and
        /// relationships</see> for more information.
        /// </para>
        /// </remarks>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            Seed(modelBuilder);

            // set the currency default value to LLL (Local Currency)
            modelBuilder.Entity<MediaHierarchyTemplate>().Property(x => x.Currency).HasDefaultValue("LLL");

            base.OnModelCreating(modelBuilder);
        }

        private void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DataDictionaryTable>().HasData(
                    new DataDictionaryTable
                    {
                        Id = new Guid("6f2ce863-ece8-442c-b5fb-bd59075914a9"),
                        Name = AthenaConsts.CampaignTable,
                        DataDictionaryColumns = string.Empty,
                    },
                    new DataDictionaryTable
                    {
                        Id = new Guid("3e185d41-9e21-44dc-9532-cd4bda1b5a0d"),
                        Name = AthenaConsts.MediaBriefsTable,
                        DataDictionaryColumns = string.Empty,
                    },
                    new DataDictionaryTable
                    {
                        Id = new Guid("f0867476-5d91-4fbf-8ebf-c5308309f4ea"),
                        Name = AthenaConsts.MediaPlansTable,
                        DataDictionaryColumns = string.Empty,
                    },
                    new DataDictionaryTable
                    {
                        Id = new Guid("604762ba-d137-44f9-a996-e51352d83699"),
                        Name = AthenaConsts.ClientRollPeriodTable,
                        DataDictionaryColumns = string.Empty,
                    },
                    new DataDictionaryTable
                    {
                        Id = new Guid("620bdd28-2967-440b-b25f-a5695986824b"),
                        Name = AthenaConsts.RollPeriodTable,
                        DataDictionaryColumns = string.Empty,
                    },
                    new DataDictionaryTable
                    {
                        Id = new Guid("e13dda1a-48a3-4e26-8a76-620bb10a2c6c"),
                        Name = AthenaConsts.Channel,
                        DataDictionaryColumns = string.Empty,
                    },
                    new DataDictionaryTable
                    {
                        Id = new Guid("0d771867-cc9a-40a5-900e-f57528f91d0e"),
                        Name = AthenaConsts.Supplier,
                        DataDictionaryColumns = string.Empty,
                    },
                    new DataDictionaryTable
                    {
                        Id = new Guid("a1b4a058-d33d-4e9b-950c-5ca3cd438a7c"),
                        Name = AthenaConsts.Placement,
                        DataDictionaryColumns = string.Empty,
                    },
                    new DataDictionaryTable
                    {
                        Id = new Guid("e5cdf0ab-0455-43f7-953e-6b230513c3e6"),
                        Name = AthenaConsts.Budget,
                        DataDictionaryColumns = string.Empty,
                    }
                );
        }
    }
}
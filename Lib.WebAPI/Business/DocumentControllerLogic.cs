using System.Reflection;
using AutoMapper;
using DocumentFormat.OpenXml.CustomProperties;
using DocumentFormat.OpenXml.Office2013.WebExtension;
using DocumentFormat.OpenXml.Office2013.WebExtentionPane;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.VariantTypes;
using Lib.Common.Business.Interfaces;
using Lib.WebAPI.Business.Interfaces;
using Lib.WebAPI.DbModels;
using Lib.WebAPI.Enumerations;
using Lib.WebAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Throw;

namespace Lib.WebAPI.Business
{
    /// <summary>
    /// DocumentControllerLogic
    /// </summary>
    public class DocumentControllerLogic
    {
        /// <summary>
        /// The excel file extension
        /// </summary>
        public const string ExcelFileExtension = ".xlsx";
        private const string ExcelTemplate = $"Template.xlsx";
        private const string FlowchartIdPropertyName = "FlowchartTemplateId";
        private const string FlowchartTemplateVersionHistoryId = "FlowchartTemplateVersionHistoryId";
        private const string FlowchartTemplateVersion = "Version";
        private const string FlowchartTemplateVersionHistoryName = "TemplateName";
        private const string ReportIdPropertyName = "ReportId";
        private const string FormatId = "{D5CDD505-2E9C-101B-9397-08002B2CF9AE}";
        private const string RunConfigurationIdPropertyName = "RunConfigurationId";
        private readonly ILog<DocumentControllerLogic> log;
        private readonly IPortalUnitOfWork portal;
        private readonly IOptionsMonitor<TaskpaneConfig> taskpaneConfig;
        private readonly SecurityLogic securityLogic;
        private readonly IMapper mapper;
        private readonly AzureBlobStorage storage;
        private readonly IOptionsMonitor<StorageConfig> storageConfig;
        private readonly IPortalDbContext portalDbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentControllerLogic" />
        /// class.
        /// </summary>
        /// <param name="taskpaneConfig">The taskpane configuration.</param>
        /// <param name="portal">The portal.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="securityLogic">The securityLogic.</param>
        /// <param name="log">The log.</param>
        /// <param name="storage">The storage.</param>
        /// <param name="storageConfig">The storageConfig.</param>
        /// <param name="portalDbContext">The portalDbContext.</param>
        public DocumentControllerLogic(
            IOptionsMonitor<TaskpaneConfig> taskpaneConfig,
            IPortalUnitOfWork portal,
            IMapper mapper,
            SecurityLogic securityLogic,
            ILog<DocumentControllerLogic> log,
            AzureBlobStorage storage,
            IOptionsMonitor<StorageConfig> storageConfig,
            IPortalDbContext portalDbContext)
        {
            this.taskpaneConfig = taskpaneConfig;
            this.portal = portal;
            this.log = log;
            this.mapper = mapper;
            this.securityLogic = securityLogic;
            this.storage = storage;
            this.storageConfig = storageConfig;
            this.portalDbContext = portalDbContext;
        }

        /// <summary>
        /// Checks the template id asynchronous.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        private async Task<bool> IsTemplateId(Guid id, CancellationToken cancellationToken)
        {
            return await portal.FlowchartTemplates.AnyAsync(cancellationToken, t => t.Id == id);
        }

        /// <summary>
        /// Checks the run configuration id asynchronous.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        private async Task<bool> IsRunConfigurationId(Guid id, CancellationToken cancellationToken)
        {
            var recordExists = await portal.RunConfigurations.AnyAsync(cancellationToken, t => t.Id == id);
            return recordExists;
        }

        /// <summary>
        /// Gets the default run configuration id by template asynchronous.
        /// </summary>
        /// <param name="templateId">The template id.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        private async Task<Guid> GetDefaultRunConfigIdByTemplateId(Guid templateId, CancellationToken cancellationToken)
        {
            var latestRunConfig =
                await portal.RunConfigurations.GetAsync(cancellationToken, 0, 1,
                    r => r.FlowchartTemplateId == templateId,
                    r=>r.CreatedDate,
                    null,
                    false);
            return latestRunConfig != null ? latestRunConfig.First().Id : Guid.Empty;
        }

        /// <summary>
        /// Generates the excel document run asynchronous.
        /// </summary>
        /// <param name="runConfigurationId">The run configuration identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<Stream> GenerateExcelDocumentRunAsync(Guid runConfigurationId, Guid userId, string fileName, CancellationToken cancellationToken)
        {
            var isRunConfigId = await IsRunConfigurationId(runConfigurationId, cancellationToken);
            var data = await portalDbContext.RunConfigurations.FirstOrDefaultAsync(x => x.Id == runConfigurationId);
            if (data != null)
            {
                await securityLogic.CheckUserHasClientAccessAsync(data.FlowchartTemplate.OmniClientId, userId, cancellationToken);
            }
            var actualRunConfigId = runConfigurationId;

            if (!isRunConfigId)
            {
                var templateId = runConfigurationId;
                log.Add(LogLevel.Warning, $"ID {runConfigurationId} is not a valid run configuration Id. Checking if it's a template ID");
                if (await IsTemplateId(templateId, cancellationToken))
                {
                    log.Add(LogLevel.Information, $"ID {templateId} is a valid template ID. Looking up the latest run config for it.");
                    var defaultRunconfig=
                        await GetDefaultRunConfigIdByTemplateId(templateId, cancellationToken);
                    if (defaultRunconfig == Guid.Empty)
                    {
                        log.Add(
                            LogLevel
                                .Warning, string.Format(
                                @"No valid run configuration found for template id {0}. Indicating that run is not available, yet",
                                templateId));
                        throw new FileNotFoundException($"No run configuration exists for template {templateId}");
                    }

                    actualRunConfigId = defaultRunconfig;
                }
            }

            log.Add(LogLevel.Information, $"Generating Excel document for run configuration: {actualRunConfigId}");

            fileName.Throw().IfEmpty().IfNotEndsWith(ExcelFileExtension, StringComparison.InvariantCultureIgnoreCase);

            // TODO: THIS IS INSECURE SINCE THE ROUTE IS ANONYMOUS. Fix by securing the route or looking up the runconfiguration before calling this method
            RunConfiguration runConfiguration;
            try
            {
                runConfiguration = await portal.RunConfigurations.GetByIdAsync(actualRunConfigId, cancellationToken);

            }
            catch (Exception ex)
            {
                log.Add(LogLevel.Error, $"Error retrieving run configuration with id {actualRunConfigId}", ex);
                throw;
            }

            var output = await GetExcelTemplateAsync(cancellationToken);

            using (var doc = SpreadsheetDocument.Open(output, true))
            {
                AddTaskpane(doc);
                AddCustomProperty(doc, nameof(TaskpaneMode), TaskpaneMode.Run.ToString());
                AddCustomProperty(doc, RunConfigurationIdPropertyName, runConfiguration.Id.ToString());

                // TODO: THIS SHOULD BE REMOVED WHEN FRONTEND IS FIXED (frontend should load the run template based on the reference in run configuration)
                AddCustomProperty(doc, FlowchartIdPropertyName, runConfiguration.FlowchartTemplateId.ToString());
            }

            output.Position = 0;

            return output;
        }

        /// <summary>
        /// Generates the excel document template asynchronous.
        /// </summary>
        /// <param name="flowchartTemplateId">The flowchart template identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<Stream> GenerateExcelDocumentTemplateAsync(
            Guid flowchartTemplateId, Guid userId, string fileName, CancellationToken cancellationToken)
        {
            log.Add(LogLevel.Information, $"Generating Excel document for flowchart template: {flowchartTemplateId}");

            var data = await portalDbContext.FlowchartTemplates.FirstOrDefaultAsync(x => x.Id == flowchartTemplateId);
            if (data != null)
            {
                await securityLogic.CheckUserHasClientAccessAsync(data.OmniClientId, userId, cancellationToken);
            }

            fileName.Throw().IfEmpty().IfNotEndsWith(ExcelFileExtension, StringComparison.InvariantCultureIgnoreCase);

            var output = await GetExcelTemplateAsync(cancellationToken);

            using (var doc = SpreadsheetDocument.Open(output, true))
            {
                AddTaskpane(doc);
                AddCustomProperty(doc, nameof(TaskpaneMode), TaskpaneMode.Edit.ToString());
                AddCustomProperty(doc, FlowchartIdPropertyName, flowchartTemplateId.ToString());
            }

            output.Position = 0;

            return output;
        }

        /// <summary>
        /// Generates the excel document by reportId asynchronous.
        /// </summary>
        /// <param name="reportId">The report identifier.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<(Stream Stream, string FileName)> GenerateExcelDocumentReportIdAsync(Guid reportId, string fileName, CancellationToken cancellationToken)
        {
            log.Add(LogLevel.Information, $"Generating Excel document for report Id: {reportId}");

            fileName.Throw().IfEmpty().IfNotEndsWith(ExcelFileExtension, StringComparison.InvariantCultureIgnoreCase);

            var report = await portal.Reports.GetByIdAsync(reportId, cancellationToken);

            #region // shows report without edit option

            // var output = await storage.DownloadAsync($"{reportId}{ExcelFileExtension}", storageConfig.CurrentValue.ContainerName, cancellationToken);

            // return (output, $"{report.Name}{ExcelFileExtension}");
            #endregion

            #region // shows same template with edit option

            var output = await GetExcelTemplateAsync(cancellationToken);

            using (var doc = SpreadsheetDocument.Open(output, true))
            {
                AddTaskpane(doc);
                AddCustomProperty(doc, nameof(TaskpaneMode), TaskpaneMode.Edit.ToString());
                AddCustomProperty(doc, FlowchartIdPropertyName, report.FlowchartTemplateId.ToString());
                // AddCustomProperty(doc, RunConfigurationIdPropertyName, report.RunConfigurationId.ToString());
            }

            output.Position = 0;

            return (output, $"{report.Name}{ExcelFileExtension}");
            #endregion

            #region // show based on reportId and RunConfigurationId -- used the above predefined method

            // var output = await GenerateExcelDocumentRunAsync(report.RunConfigurationId, fileName, cancellationToken);
            // return (output, $"{report.Name}{ExcelFileExtension}");
            #endregion
        }

        /// <summary>
        /// Generates the excel document by flowchartTemplateVersionHistoryId asynchronous.
        /// </summary>
        /// <param name="flowchartTemplateVersionHistoryId">The report identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<(Stream Stream, string FileName)> GenerateExcelDocumentByFlowchartTemplateVersionHistoryIdAsync(Guid flowchartTemplateVersionHistoryId, Guid userId, string fileName, CancellationToken cancellationToken)
        {
            log.Add(LogLevel.Information, $"Generating Excel document for report Id: {flowchartTemplateVersionHistoryId}");

            var data = await portalDbContext.FlowchartTemplatesVersionHistorties.FirstOrDefaultAsync(x => x.Id == flowchartTemplateVersionHistoryId);
            if (data != null)
            {
                await securityLogic.CheckUserHasClientAccessAsync(data.FlowchartTemplate.OmniClientId, userId, cancellationToken);
            }

            fileName.Throw().IfEmpty().IfNotEndsWith(ExcelFileExtension, StringComparison.InvariantCultureIgnoreCase);

            var flowchartTemplateVersionHistoryData = await portal.FlowchartTemplatesVersionHistorties.GetByIdAsync(flowchartTemplateVersionHistoryId, cancellationToken);

            #region //shows same template with edit option

            var output = await GetExcelTemplateAsync(cancellationToken);

            using (var doc = SpreadsheetDocument.Open(output, true))
            {
                AddTaskpane(doc);
                AddCustomProperty(doc, nameof(TaskpaneMode), TaskpaneMode.Edit.ToString());
                AddCustomProperty(doc, FlowchartIdPropertyName, flowchartTemplateVersionHistoryData.FlowchartTemplateId.ToString());
                AddCustomProperty(doc, FlowchartTemplateVersionHistoryId, flowchartTemplateVersionHistoryData.Id.ToString());
                AddCustomProperty(doc, FlowchartTemplateVersion, Convert.ToString(flowchartTemplateVersionHistoryData.Version));
            }

            output.Position = 0;

            return (output, $"{flowchartTemplateVersionHistoryData.Name}{ExcelFileExtension}");
            #endregion
        }

        /// <summary>
        /// Methods to add document custom property.
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        private void AddCustomProperty(SpreadsheetDocument doc, string name, string value)
        {
            var part = doc.CustomFilePropertiesPart ?? doc.AddCustomFilePropertiesPart();
            part.Properties ??= new Properties();

            var id = part.Properties.Select(x => x as CustomDocumentProperty).Max(x => x?.PropertyId?.Value) ?? 1;

            part.Properties.Append(new CustomDocumentProperty
            {
                FormatId = FormatId,
                Name = name,
                VTLPWSTR = new VTLPWSTR(value),
                PropertyId = ++id,
            });
        }

        /// <summary>
        /// Methods to add web extension taskpane.
        /// </summary>
        /// <param name="doc">The doc.</param>
        private void AddTaskpane(SpreadsheetDocument doc)
        {
            var part = doc.WebExTaskpanesPart ?? doc.AddWebExTaskpanesPart();

            part.Taskpanes ??= new Taskpanes();

            var taskpane = new WebExtensionTaskpane
            {
                DockState = taskpaneConfig.CurrentValue.DockState,
                Visibility = taskpaneConfig.CurrentValue.Visibility,
                Width = taskpaneConfig.CurrentValue.Width,
                Row = taskpaneConfig.CurrentValue.Row,
            };

            var reference = new WebExtensionPartReference
            {
                Id = $"rId{Guid.NewGuid()}".Replace("-", string.Empty),
            };

            taskpane.Append(reference);

            part.Taskpanes.Append(taskpane);

            var extPart = part.AddNewPart<WebExtensionPart>(reference.Id.Value!);

            extPart.WebExtension = new WebExtension
            {
                Id = $"{{{taskpaneConfig.CurrentValue.Id}}}",
            };

            extPart.WebExtension.Append(new WebExtensionStoreReference
            {
                Id = taskpaneConfig.CurrentValue.Id.ToString(),
                Version = taskpaneConfig.CurrentValue.Version,
                Store = taskpaneConfig.CurrentValue.Store,
                StoreType = taskpaneConfig.CurrentValue.StoreType,
            });

            extPart.WebExtension.Append(new WebExtensionPropertyBag(new WebExtensionProperty
            {
                Name = "Office.AutoShowTaskpaneWithDocument",
                Value = "true",
            }));
            extPart.WebExtension.Append(new WebExtensionBindingList());
            extPart.WebExtension.Append(new WebExtensionReferenceList());
            extPart.WebExtension.Append(new Snapshot());
        }

        /// <summary>
        /// Gets the excel document by Excel Template asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        private async Task<Stream> GetExcelTemplateAsync(CancellationToken cancellationToken)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var name = assembly.GetManifestResourceNames().FirstOrDefault(x => x.EndsWith(ExcelTemplate))
                ?? throw new FileNotFoundException($"Excel template ressource not found.");

            var output = new MemoryStream();

            using var stream = assembly.GetManifestResourceStream(name)
                ?? throw new Exception("Could not get manifest resource stream.");

            await stream.CopyToAsync(output, cancellationToken);

            output.Position = 0;

            return output;
        }
    }
}
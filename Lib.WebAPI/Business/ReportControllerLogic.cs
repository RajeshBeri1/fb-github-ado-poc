using AutoMapper;
using DocumentFormat.OpenXml.Packaging;
using Lib.Common.Business;
using Lib.Common.Business.Interfaces;
using Lib.WebAPI.Business.Interfaces;
using Lib.WebAPI.DbModels;
using Lib.WebAPI.DTOs;
using Lib.WebAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Lib.WebAPI.Business
{
    /// <summary>
    /// ReportControllerLogic
    /// </summary>
    public class ReportControllerLogic
    {
        private const string DataFileExtension = ".data";
        private const string FlowchartTemplateFileExtension = ".template";
        private const string RunConfigFileExtension = ".settings";
        private readonly ILog<ReportControllerLogic> log;
        private readonly IMapper mapper;
        private readonly IPortalUnitOfWork portal;
        private readonly SecurityLogic securityLogic;
        private readonly AzureBlobStorage storage;
        private readonly IOptionsMonitor<StorageConfig> storageConfig;
        private readonly IPortalDbContext portalDbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportControllerLogic" /> class.
        /// </summary>
        /// <param name="storage">The storage.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="log">The log.</param>
        /// <param name="securityLogic">The security logic.</param>
        /// <param name="portal">The portal.</param>
        /// <param name="portalDbContext">The portalDbContext.</param>
        /// <param name="storageConfig">The storage configuration.</param>
        public ReportControllerLogic(
            AzureBlobStorage storage,
            IMapper mapper,
            ILog<ReportControllerLogic> log,
            SecurityLogic securityLogic,
            IPortalUnitOfWork portal,
            IOptionsMonitor<StorageConfig> storageConfig,
            IPortalDbContext portalDbContext)
        {
            this.storage = storage;
            this.mapper = mapper;
            this.log = log;
            this.securityLogic = securityLogic;
            this.portal = portal;
            this.storageConfig = storageConfig;
            this.portalDbContext = portalDbContext;
        }

        /// <summary>
        /// Deletes the asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<ReportInfoDTO> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken)
        {
            var report = await portal.Reports.GetByIdAsync(id, cancellationToken);

            await securityLogic.CheckUserHasClientAccessAsync(report.FlowchartTemplate.OmniClientId, userId, cancellationToken);

            securityLogic.CheckIsOwner(report.CreatedByUserId, userId);

            log.Add(LogLevel.Information, $"Deleting report: {report.Id} by user: {userId}");

            report = await portal.Reports.SoftDeleteAsync(report.Id, cancellationToken);

            await portal.SaveChangesAsync(cancellationToken);

            return mapper.Map<ReportInfoDTO>(report);
        }

        /// <summary>
        /// Downloads the asynchronous.
        /// </summary>
        /// <param name="reportId">The report identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<(Stream Stream, string FileName)> DownloadAsync(Guid reportId, Guid userId, CancellationToken cancellationToken)
        {
            log.Add(LogLevel.Information, $"Downloading Report document for user: {userId}");

            var report = await portal.Reports.GetByIdAsync(reportId, cancellationToken);

            await securityLogic.CheckUserHasClientAccessAsync(report.FlowchartTemplate.OmniClientId, userId, cancellationToken);

            var stream = await storage.DownloadAsync($"{reportId}{DocumentControllerLogic.ExcelFileExtension}", storageConfig.CurrentValue.ContainerName, cancellationToken);

            return (stream, $"{report.Name}{DocumentControllerLogic.ExcelFileExtension}");
        }

        /// <summary>
        /// Downloads the latest report asynchronous based on flowchart template Id.
        /// </summary>
        /// <param name="flowChartTemplateId">The flowchart template identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<(Stream Stream, string FileName)> DownloadByFlowChartTemplateId(Guid flowChartTemplateId, Guid userId, CancellationToken cancellationToken)
        {
            log.Add(LogLevel.Information, $"Downloading Report document for user: {userId}");

            var flowChartTemplateDetails = await portal.FlowchartTemplates.GetByIdAsync(flowChartTemplateId, cancellationToken);

            var reportDetails = await portalDbContext.Reports
                .Where(x => x.FlowchartTemplateId == flowChartTemplateId)
            .OrderByDescending(z => z.ModifiedDate)
                .Take(1).ToListAsync(cancellationToken: cancellationToken) ?? throw new KeyNotFoundException($"No report found for flowchart template name: {flowChartTemplateDetails.Name} and flowchart template Id: {flowChartTemplateId}");

            if (reportDetails.Count > 0)
            {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                string reportName = reportDetails.FirstOrDefault(x => x.FlowchartTemplateId == flowChartTemplateId).Name ?? flowChartTemplateDetails.Name;
#pragma warning restore CS8602 // Dereference of a possibly null reference.

#pragma warning disable CS8602 // Dereference of a possibly null reference.
                Guid reportId = reportDetails.FirstOrDefault(x => x.FlowchartTemplateId == flowChartTemplateId).Id;
#pragma warning restore CS8602 // Dereference of a possibly null reference.

                return await DownloadAsync(reportId, userId, cancellationToken);
            }
            else
            {
                throw new KeyNotFoundException($"No report found for flowchart template name: {flowChartTemplateDetails.Name} and flowchart template Id: {flowChartTemplateId}");
            }
        }

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<ReportInfoDTO> GetAsync(Guid id, Guid userId, CancellationToken cancellationToken)
        {
            var result = await portal.Reports.GetByIdAsync(id, cancellationToken);

            await securityLogic.CheckUserHasClientAccessAsync(result.FlowchartTemplate.OmniClientId, userId, cancellationToken);

            return mapper.Map<ReportInfoDTO>(result);
        }

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <param name="flowchartTemplateId">The flowchart template identifier.</param>
        /// <param name="search">The search.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<ReportInfoListDTO> GetAsync(Guid flowchartTemplateId, ReportSearchDTO search, Guid userId, CancellationToken cancellationToken)
        {
            var template = await portal.FlowchartTemplates.GetByIdAsync(flowchartTemplateId, cancellationToken);

            await securityLogic.CheckUserHasClientAccessAsync(template.OmniClientId, userId, cancellationToken);

            var (items, totalCount) = await portal.Reports.SearchAsync<ReportInfoDTO>(
                   search, cancellationToken, x => x.FlowchartTemplateId == flowchartTemplateId);

            return new ReportInfoListDTO
            {
                Items = items.OrderByDescending(x=> x.FlowchartTemplateVersion),
                TotalCount = totalCount,
            };
        }

        /// <summary>
        /// Publishes the asynchronous.
        /// </summary>
        /// <param name="reportPublish">The report publish.</param>
        /// <param name="user">The user.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<ReportInfoDTO> PublishAsync(ReportPublishDTO reportPublish, UserDetailsDTO user, CancellationToken cancellationToken)
        {
            log.Add(LogLevel.Information, $"Publishing report document for user: {user.Id}");

            var config = await portal.RunConfigurations.GetByIdAsync(reportPublish.RunConfigurationId, cancellationToken);

            await securityLogic.CheckUserHasClientAccessAsync(config.FlowchartTemplate.OmniClientId, user.Id, cancellationToken);

            using var file = new MemoryStream(reportPublish.FileContent);
            using var stream = await CleanupExcelDocumentAsync(file, cancellationToken);

            int runConfigCount = portalDbContext.Reports.Where(x => x.FlowchartTemplateId == config.FlowchartTemplateId).Count();
            int reportVersion = runConfigCount > 0 ? runConfigCount + 1 : 1;

            var report = await portal.Reports.AddAsync(
                new Report
                {
                    CreatedByUserId = user.Id,
                    ModifiedByUserId = user.Id,
                    Name = reportPublish.Name,
                    FileType = reportPublish.FileType,
                    FlowchartTemplateId = config.FlowchartTemplateId,
                    FlowchartTemplateVersion = reportVersion,
                    RunConfigurationId = config.Id,
                    Comments = reportPublish.Comments?? "Initial Version",
                }, cancellationToken);

            await storage.UploadAsync(stream, $"{report.Id}{DocumentControllerLogic.ExcelFileExtension}", storageConfig.CurrentValue.ContainerName, cancellationToken);

            using var dataStream = new MemoryStream();
            Json.Serialize(reportPublish.Data, dataStream);
            dataStream.Position = 0;

            await storage.UploadAsync(dataStream, $"{report.Id}{DataFileExtension}", storageConfig.CurrentValue.ContainerName, cancellationToken);

            using var templateStream = new MemoryStream();
            Json.Serialize(config.FlowchartTemplate, templateStream);
            templateStream.Position = 0;

            await storage.UploadAsync(templateStream, $"{report.Id}{FlowchartTemplateFileExtension}", storageConfig.CurrentValue.ContainerName, cancellationToken);

            using var settingsStream = new MemoryStream();
            Json.Serialize(reportPublish.RunConfigurationData, settingsStream);
            settingsStream.Position = 0;

            await storage.UploadAsync(settingsStream, $"{report.Id}{RunConfigFileExtension}", storageConfig.CurrentValue.ContainerName, cancellationToken);

            await portal.SaveChangesAsync(cancellationToken);

            var result = mapper.Map<ReportInfoDTO>(report);

            result.CreatedByUser = result.ModifiedByUser = user;
            result.RunConfiguration = mapper.Map<RunConfigurationInfoDTO>(config);

            return result;
        }

        /// <summary>
        /// Restores the flowchart asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="user">The user.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<FlowchartTemplateInfoDTO> RestoreFlowchartAsync(Guid id, UserDetailsDTO user, CancellationToken cancellationToken)
        {
            log.Add(LogLevel.Information, $"Restoring flowchart for user: {user.Id}");

            var report = await portal.Reports.GetByIdAsync(id, cancellationToken);

            await securityLogic.CheckUserHasClientAccessAsync(report.FlowchartTemplate.OmniClientId, user.Id, cancellationToken);
            securityLogic.CheckIsOwner(report.FlowchartTemplate.CreatedByUserId, user.Id);

            using var stream = await storage.DownloadAsync($"{report.Id}{FlowchartTemplateFileExtension}", storageConfig.CurrentValue.ContainerName, cancellationToken);

            var restored = Json.Deserialize<FlowchartTemplate>(stream)
                ?? throw new Exception($"Restore failed: deserialisation failed.");

            // things to restore
            report.FlowchartTemplate.FlowchartDefinition = restored.FlowchartDefinition;
            report.FlowchartTemplate.Version = restored.Version; // or should we use the latest version number and increase?

            await portal.SaveChangesAsync(cancellationToken);

            return mapper.Map<FlowchartTemplateInfoDTO>(report.FlowchartTemplate);
        }

        /// <summary>
        /// Cleans up the excel document asynchronous.
        /// </summary>
        /// <param name="file">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        private async Task<Stream> CleanupExcelDocumentAsync(Stream file, CancellationToken cancellationToken)
        {
            var output = new MemoryStream();
            await file.CopyToAsync(output, cancellationToken);
            output.Position = 0;

            using (var doc = SpreadsheetDocument.Open(output, true))
            {
                if (doc.WebExTaskpanesPart != null)
                {
                    doc.DeletePart(doc.WebExTaskpanesPart);
                }

                if (doc.CustomFilePropertiesPart != null)
                {
                    doc.DeletePart(doc.CustomFilePropertiesPart);
                }
            }

            output.Position = 0;

            return output;
        }
    }
}
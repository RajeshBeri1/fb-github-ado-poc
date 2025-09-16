using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DocumentFormat.OpenXml.Drawing;
using Lib.Common.Business.Interfaces;
using Lib.WebAPI.Business.Interfaces;
using Lib.WebAPI.DbModels;
using Lib.WebAPI.DTOs;
using Lib.WebAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Lib.WebAPI.Business
{
    /// <summary>
    /// FlowchartTemplatesHistortyControllerLogic
    /// </summary>
    public class FlowchartTemplatesHistortyControllerLogic : IFlowchartTemplatesHistortyControllerLogic
    {
        private const string DataFileExtension = ".data";
        private const string FlowchartTemplateFileExtension = ".template";
        private const string RunConfigFileExtension = ".settings";
        private readonly ILog<FlowchartTemplatesHistortyControllerLogic> log;
        private readonly IMapper mapper;
        private readonly IPortalUnitOfWork portal;
        private readonly SecurityLogic securityLogic;
        private readonly AzureBlobStorage storage;
        private readonly IOptionsMonitor<StorageConfig> storageConfig;
        private readonly IPortalDbContext portalDbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="FlowchartTemplatesHistortyControllerLogic" /> class.
        /// </summary>
        /// <param name="storage">The storage.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="log">The log.</param>
        /// <param name="securityLogic">The security logic.</param>
        /// <param name="portal">The portal.</param>
        /// <param name="storageConfig">The storage configuration.</param>
        /// <param name="portalDbContext">To Get and Set the data from the database</param>
        public FlowchartTemplatesHistortyControllerLogic(
            AzureBlobStorage storage,
            IMapper mapper,
            ILog<FlowchartTemplatesHistortyControllerLogic> log,
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
        /// Gets the asynchronous.
        /// </summary>
        /// <param name="search">The search.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<FlowchartTemplatesVersionHistortiesListDTO> GetAsync(FlowchartTemplatesVersionHistortySearchDTO search, Guid userId, CancellationToken cancellationToken)
        {
            List<FlowchartTemplatesVersionHistortiesDTO> histories = new List<FlowchartTemplatesVersionHistortiesDTO>();

            var template = await portal.FlowchartTemplates.GetByIdAsync(search.FlowchartTemplateId, cancellationToken);

            await securityLogic.CheckUserHasClientAccessAsync(template.OmniClientId, userId, cancellationToken);

            var items = await portalDbContext.FlowchartTemplatesVersionHistorties.Where(x=> x.FlowchartTemplateId == search.FlowchartTemplateId).ToListAsync(cancellationToken);

            foreach (var item in items )
            {
                FlowchartTemplatesVersionHistortiesDTO history = new()
                {
                    Id = item.Id,
                    FlowchartTemplateId = item.FlowchartTemplateId,
                    FlowchartDefinition = item.FlowchartDefinition,
                    Version = item.Version,
                    Name = item.Name,
                    CreatedDate = item.CreatedDate,
                    ModifiedDate = item.ModifiedDate,
                };
                UserInfoDTO userInfo = new()
                {
                    Id = item.CreatedByUser.Id,
                    Name = item.CreatedByUser.Name,
                    DisplayName = item.CreatedByUser.DisplayName,
                    Removed = item.CreatedByUser.Removed,
                };
                history.CreatedByUser = userInfo;
                history.ModifiedByUser = userInfo;
                history.Comments = item.Comments;
                histories.Add(history);
            }
            return new FlowchartTemplatesVersionHistortiesListDTO
            {
                Items = histories.OrderByDescending(x => x.Version),
                TotalCount = histories.Count,
            };
        }
    }
}

using AutoMapper;
using Lib.Common.Business;
using Lib.Common.Business.Interfaces;
using Lib.WebAPI.Business.Interfaces;
using Lib.WebAPI.DbModels;
using Lib.WebAPI.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Throw;

namespace Lib.WebAPI.Business
{
    /// <summary>
    /// FieldInfoServices
    /// </summary>
    public class FieldInfoServices : IFieldInfoServices
    {
        private readonly IPortalUnitOfWork portal;
        private readonly ILog<IFieldInfoServices> log;
        private readonly IMapper mapper;
        private readonly IPortalDbContext portalDbContext;


        /// <summary>
        /// Initializes a new instance of the <see cref="FieldInfoServices"/> class.
        /// The FieldInfoServices constructor
        /// </summary>
        /// <param name="portal">The portal.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="log">The log.</param>
        /// <param name="portalDbContext">The portalDbContext.</param>
        public FieldInfoServices(IPortalUnitOfWork portal, ILog<IFieldInfoServices> log, IMapper mapper, IPortalDbContext portalDbContext)
        {
            this.portal = portal;
            this.log = log;
            this.mapper = mapper;
            this.portalDbContext = portalDbContext;
        }

        /// <summary>
        /// Create the FieldInfoDto asynchronous.
        /// </summary>
        /// <param name="fieldInfoDto">The fieldInfo Dto.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<bool> CreateFieldInfoAsync(List<FieldInfoDto> fieldInfoDto, CancellationToken cancellationToken)
        {
            // Ensure the input list is not null
            fieldInfoDto.ThrowIfNull(nameof(fieldInfoDto));

            // Get all existing FieldInfo records from the database
            var existingFieldInfos = await portalDbContext.FieldInfos.ToListAsync(cancellationToken);

            // Filter out DTOs that already exist (by Name and Type, for example)
            var dataToInsert = existingFieldInfos.Count > 0 ? fieldInfoDto.Where(x => !existingFieldInfos.Any(y => y.Name == x.Name && y.Type == x.Type)).ToList() : fieldInfoDto;

            if (dataToInsert.Count > 0)
            {
                var fieldInfoEntities = mapper.Map<List<FieldInfo>>(dataToInsert);
                await portal.FieldInfos.AddRangeAsync(fieldInfoEntities, cancellationToken);
                await portal.SaveChangesAsync(cancellationToken);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Update the updateFieldInfoDto asynchronous.
        /// </summary>
        /// <param name="updateFieldInfoDto">The updateFieldInfo Dto.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task UpdateFieldInfoAsync(UpdateFieldInfoDto updateFieldInfoDto, CancellationToken cancellationToken)
        {
            updateFieldInfoDto.ThrowIfNull(nameof(updateFieldInfoDto));
            var fieldInfo = await portalDbContext.FieldInfos.FirstOrDefaultAsync(x => x.Id == updateFieldInfoDto.Id, cancellationToken: cancellationToken);
            fieldInfo.ThrowIfNull(nameof(fieldInfo));
            fieldInfo.Name = updateFieldInfoDto.Name;
            fieldInfo.Type = updateFieldInfoDto.Type;
            fieldInfo.IsMetric = updateFieldInfoDto.IsMetric;
            fieldInfo.IsCurrency = updateFieldInfoDto.IsCurrency;
            fieldInfo.IsCommon = updateFieldInfoDto.IsCommon;
            fieldInfo.ModuleType = updateFieldInfoDto.ModuleType;
            fieldInfo.Removed = updateFieldInfoDto.Removed;
            await portal.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Gets the Fieldinfo list asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="removed">The Removed.</param>
        public async Task<List<FieldInfo>> GetAllAsync(CancellationToken cancellationToken, bool removed = false)
        {
            var fieldInfo = await portal.FieldInfos.GetAllAsync(cancellationToken, removed);
            fieldInfo.ThrowIfNull(nameof(fieldInfo));
            return (List<FieldInfo>)fieldInfo;
        }


        /// <summary>
        /// Deletes the asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken)
        {
            var fieldInfos = await portal.FieldInfos.GetByIdAsync(id, cancellationToken);

            log.Add(LogLevel.Information, $"Deleting fieldInfos record: {fieldInfos.Id}");

            await portal.FieldInfos.SoftDeleteAsync(id, cancellationToken);

            await portal.SaveChangesAsync(cancellationToken);
        }
    }
}

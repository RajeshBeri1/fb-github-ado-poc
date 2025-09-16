using System.Collections.ObjectModel;
using AutoMapper;
using Lib.Athena.Consts;
using Lib.Common.Business;
using Lib.Common.Business.Interfaces;
using Lib.WebAPI.Business.Interfaces;
using Lib.WebAPI.DTOs;
using Lib.WebAPI.Enums;
using Lib.WebAPI.Models;
using Microsoft.Extensions.Logging;

namespace Lib.WebAPI.Business
{
    /// <summary>
    /// DataDictionaryControllerLogic
    /// </summary>
    public class DataDictionaryControllerLogic
    {
        private readonly ILog<DataDictionaryControllerLogic> log;
        private readonly IMapper mapper;
        private readonly IPortalUnitOfWork portal;
        private readonly SecurityLogic securityLogic;
        private readonly DataDictionaryUpdater dataDictionaryUpdater;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataDictionaryControllerLogic" />
        /// class.
        /// </summary>
        /// <param name="portal">The portal.</param>
        /// <param name="log">The log.</param>
        /// <param name="securityLogic">The security logic.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="dataDictionaryUpdater">The data dictionary updater.</param>
        public DataDictionaryControllerLogic(
            IPortalUnitOfWork portal,
            ILog<DataDictionaryControllerLogic> log,
            SecurityLogic securityLogic,
            IMapper mapper,
            DataDictionaryUpdater dataDictionaryUpdater)
        {
            this.portal = portal;
            this.log = log;
            this.securityLogic = securityLogic;
            this.mapper = mapper;
            this.dataDictionaryUpdater = dataDictionaryUpdater;
        }

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <param name="omniClientId">The omni client identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="displaySource">The display source.</param>
        public async Task<DataDictionaryDTO> GetAsync(Guid omniClientId, Guid userId, CancellationToken cancellationToken, DisplaySource displaySource = DisplaySource.PlanitClientAlias)
        {
            log.Add(LogLevel.Information, $"Getting data dictionary for user {userId}.");

            await securityLogic.CheckUserHasClientAccessAsync(omniClientId, userId, cancellationToken);

            var clientId = await securityLogic.GetClientIdByGuidAsync(omniClientId, cancellationToken);

            var items = await portal.DataDictionaryTables.GetAsync(cancellationToken, predicate: x => !DataDictionaryUpdater.PmdsNewTables.Contains(x.Name));
            var tables = mapper.Map<List<DataDictionaryTableDetailsDTO>>(items);

            var columnAliases = (await portal.ClientColumnAliases.GetAsync(cancellationToken, predicate: x => x.ClientId == clientId)).FirstOrDefault();
            var aliases = columnAliases == null ? null : Json.Deserialize<Collection<ColumnAlias>>(columnAliases.ColumnAliases);

            // Handle displaysource
            switch (displaySource)
            {
                case DisplaySource.PlanningSystem:
                    foreach (var table in tables)
                    {
                        foreach (var column in table.Columns)
                        {
                            column.DisplayName = column.Name; // Displays the field name
                        }
                    }
                    break;
                case DisplaySource.PlanitClientAlias:
                    foreach (var table in tables)
                        {
                            foreach (var column in table.Columns)
                            {
                                // Displays the alias name if available, otherwise fallback to field name
                                column.DisplayName = aliases?.FirstOrDefault(x =>
                                    column.Name.Equals(x.ColumnName, StringComparison.InvariantCultureIgnoreCase) &&
                                    table.Name.Equals(x.TableName, StringComparison.InvariantCultureIgnoreCase))?.Alias
                                    ?? column.Name;
                            }
                        }
                    break;
                default:
                    // Log or handle unexpected enum values
                    log.Add(LogLevel.Warning, $"Unexpected DataSourceEnum value: {displaySource}. Defaulting to FieldName.");
                    foreach (var table in tables)
                    {
                        foreach (var column in table.Columns)
                        {
                            column.DisplayName = column.Name; // Fallback to field name
                        }
                    }
                    break;
            }

            // Return the processed data dictionary DTO
            return new DataDictionaryDTO { Tables = tables };
        }

        /// <summary>
        /// Gets the broadcast calendar columns asynchronous.
        /// </summary>
        /// <param name="omniClientId">The omni client identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<ICollection<DataDictionaryColumnDetailsDTO>> GetBroadcastCalendarColumnsAsync(
            Guid omniClientId, Guid userId, CancellationToken cancellationToken)
        {
            log.Add(LogLevel.Information, $"Getting broadcast calendar columns for user {userId}.");

            await securityLogic.CheckUserHasClientAccessAsync(omniClientId, userId, cancellationToken);

            var clientId = await securityLogic.GetClientIdByGuidAsync(omniClientId, cancellationToken);

            var items = await portal.DataDictionaryTables.GetByNameAsync(AthenaConsts.RollPeriodTable, cancellationToken);
            var table = mapper.Map<DataDictionaryTableDetailsDTO>(items);

            var columnAliases = (await portal.ClientColumnAliases.GetAsync(cancellationToken, predicate: x => x.ClientId == clientId)).FirstOrDefault();
            var aliases = columnAliases == null ? null : Json.Deserialize<Collection<ColumnAlias>>(columnAliases.ColumnAliases);

            foreach (var column in table.Columns)
            {
                column.DisplayName = aliases?.FirstOrDefault(x =>
                    column.Name.Equals(x.ColumnName, StringComparison.InvariantCultureIgnoreCase) &&
                    table.Name.Equals(x.TableName, StringComparison.InvariantCultureIgnoreCase))?.Alias
                    ?? column.Name;
            }

            return table.Columns;
        }

        /// <summary>
        /// Gets the client calendar columns asynchronous.
        /// </summary>
        /// <param name="omniClientId">The omni client identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<ICollection<DataDictionaryColumnDetailsDTO>> GetClientCalendarColumnsAsync(
            Guid omniClientId, Guid userId, CancellationToken cancellationToken)
        {
            log.Add(LogLevel.Information, $"Getting client calendar columns for user {userId}.");

            await securityLogic.CheckUserHasClientAccessAsync(omniClientId, userId, cancellationToken);

            var clientId = await securityLogic.GetClientIdByGuidAsync(omniClientId, cancellationToken);

            var items = await portal.DataDictionaryTables.GetByNameAsync(AthenaConsts.ClientRollPeriodTable, cancellationToken);
            var table = mapper.Map<DataDictionaryTableDetailsDTO>(items);

            var columnAliases = (await portal.ClientColumnAliases.GetAsync(cancellationToken, predicate: x => x.ClientId == clientId)).FirstOrDefault();
            var aliases = columnAliases == null ? null : Json.Deserialize<Collection<ColumnAlias>>(columnAliases.ColumnAliases);

            foreach (var column in table.Columns)
            {
                column.DisplayName = aliases?.FirstOrDefault(x =>
                    column.Name.Equals(x.ColumnName, StringComparison.InvariantCultureIgnoreCase) &&
                    table.Name.Equals(x.TableName, StringComparison.InvariantCultureIgnoreCase))?.Alias
                    ?? column.Name;
            }

            return table.Columns;
        }

        /// <summary>
        /// Update the client details by ClientId.
        /// </summary>
        /// <param name="omniClientId">The omni client identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task UpdateClientDetailsByOmniClientIdAsync(
            Guid omniClientId, Guid userId, CancellationToken cancellationToken)
        {
            await securityLogic.CheckUserHasClientAccessAsync(omniClientId, userId, cancellationToken);
            string clientId = await securityLogic.GetClientIdByGuidAsync(omniClientId, cancellationToken);
            await dataDictionaryUpdater.UpdateClientDetailsByOmniClientIdAsync(omniClientId, clientId, cancellationToken);
        }

        /// <summary>
        /// Refresh the DataDictionary Tables data manually.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<string> RefreshDataDictionaryTablesAsync(CancellationToken cancellationToken)
        {
            return await dataDictionaryUpdater.RefreshDataDictionaryTablesAsync(cancellationToken);
        }
    }
}
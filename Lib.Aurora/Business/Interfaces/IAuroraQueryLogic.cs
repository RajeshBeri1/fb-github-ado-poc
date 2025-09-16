using Amazon.Athena.Model;
using Lib.Athena.Models;
using Lib.Aurora.Models;
using Lib.MediaopsToFlowChart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Aurora.Business.Interfaces
{
    public interface IAuroraQueryLogic
    {
        /// <summary>
        /// Gets the table column infos asynchronous.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<List<ColumnInfo>> GetTableColumnInfosAsync(string table, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the Client Column Aliases infos asynchronous.
        /// </summary>
        /// <param name="clientid">The clientid.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<List<Row>> GetDisplayNameFromClientColumnAliasesTable(Guid omniGuid, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the Client Column info using clientId and omniGuid asynchronous.
        /// </summary>
        /// <param name="clientid">The clientId.</param>
        /// <param name="omniGuid">The omniGuid.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<List<TemplatesClientColumnAliases>> GetClientColumnDetailsByClientIdAndOmniGuid(string clientId, Guid omniGuid, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the Client Column details using clientId and omniGuid asynchronous.
        /// </summary>
        /// <param name="clientid">The clientId.</param>
        /// <param name="omniGuid">The omniGuid.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<List<ClientColumnAliases>> GetClientColumnInfoByClientIdAndOmniGuid(string? clientId, Guid? omniGuid, CancellationToken cancellationToken);

        /// <summary>
        /// Queries the asynchronous.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<AthenaQueryResult> QueryAsync(string query, CancellationToken cancellationToken, bool isSplitQuery=false);
    }
}
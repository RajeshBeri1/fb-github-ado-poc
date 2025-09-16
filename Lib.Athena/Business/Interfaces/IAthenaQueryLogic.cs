using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.Athena.Model;
using Lib.Athena.Models;

namespace Lib.Athena.Business.Interfaces
{
    /// <summary>
    /// IAthenaQueryLogic
    /// </summary>
    public interface IAthenaQueryLogic
    {
        /// <summary>
        /// Gets the table column infos asynchronous.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<List<ColumnInfo>> GetTableColumnInfosAsync(string table, CancellationToken cancellationToken);

        /// <summary>
        /// Queries the asynchronous.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<AthenaQueryResult> QueryAsync(string query, CancellationToken cancellationToken);
    }
}
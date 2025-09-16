using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.MediaopsToFlowChart.Models
{
    public class MediaHierarchyColumnsDetailss
    {
        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>The client identifier.</value>
        public Guid? OmniClientId { get; set; }

        /// <summary>
        /// Gets or sets the name of the column.
        /// </summary>
        /// <value>The name of the column.</value>
        public string? ClientName { get; set; } = default!;

        /// <summary>
        /// Gets or sets the ClientId.
        /// </summary>
        /// <value>The ClientId.</value>
        public string? ClientId { get; set; } = default!;

        /// <summary>
        /// Gets or sets the name of the column.
        /// </summary>
        /// <value>The name of the column.</value>
        public List<MediaHierarchyColumnAndTableDetailss>? ColumnInfoss { get; set; } = default!;
    }

    /// <summary>
    /// MediaHierarchyColumnAndTableDetails
    /// </summary>
    public class MediaHierarchyColumnAndTableDetailss
    {
        /// <summary>
        /// Gets or sets the column name.
        /// </summary>
        /// <value>The column name.</value>
        public string? ColumnName { get; set; } = default!;

        /// <summary>
        /// Gets or sets the table identifier.
        /// </summary>
        /// <value>The table identifier.</value>
        public Guid? TableId { get; set; }
    }

    public class ClientColumnAliases
    {
        public string omniguid { get; set; }
        public string client_name { get; set; }
        public string client_id { get; set; }
        public string pmds_column_name { get; set; }
        public string columnalias { get; set; }
        public string tier { get; set; }
    }

    /// <summary>
    /// OmniClientIdWithClientId
    /// </summary>
    public class OmniClientIdWithClientId
    {
        /// <summary>
        /// Gets or sets the OmniClientId identifier.
        /// </summary>
        /// <value>The OmniClientId identifier.</value>
        public Guid? OmniClientId { get; set; }

        /// <summary>
        /// Gets or sets the ClientId.
        /// </summary>
        /// <value>The ClientId.</value>
        public string? ClientId { get; set; } = default!;

        /// <summary>
        /// Gets or sets the name of the column.
        /// </summary>
        /// <value>The name of the column.</value>
        public string? ClientName { get; set; } = default!;
    }
}

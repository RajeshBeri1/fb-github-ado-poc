using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.MediaopsToFlowChart.Models
{
    /// <summary>
    /// FlowchartDataDictionaryColumnDetails
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class FlowchartDataDictionaryColumnDetails
    {
        /// <summary>
        /// Gets or sets the TableId.
        /// </summary>
        /// <value>The TableId.</value>
        public Guid TableId { get; set; }

        /// <summary>
        /// Gets or sets the TableName.
        /// </summary>
        /// <value>The TableName.</value>
        public string? TableName { get; set; }

        /// <summary>
        /// Gets or sets the ColumnName.
        /// </summary>
        /// <value>The ColumnName.</value>
        public string? ColumnName { get; set; }

        /// <summary>
        /// Gets or sets the DisplayName.
        /// </summary>
        /// <value>The DisplayName.</value>
        public string? DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the ColumnType.
        /// </summary>
        /// <value>The ColumnType.</value>
        public string? ColumnType { get; set; }

        /// <summary>
        /// Gets or sets the IsCommon.
        /// </summary>
        /// <value>The IsCommon.</value>
        public bool IsCommon { get; set; }

        /// <summary>
        /// Gets or sets the IsCurrency.
        /// </summary>
        /// <value>The IsCurrency.</value>
        public bool IsCurrency { get; set; }

        /// <summary>
        /// Gets or sets the IsMetric.
        /// </summary>
        /// <value>The IsMetric.</value>
        public bool IsMetric { get; set; }


    }
}

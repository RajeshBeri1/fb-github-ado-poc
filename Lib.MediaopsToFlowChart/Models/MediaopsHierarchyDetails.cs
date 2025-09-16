using System.Diagnostics.CodeAnalysis;

namespace Lib.MediaopsToFlowChart.Models
{
    /// <summary>
    /// MediaopsHierarchyDetails
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MediaopsHierarchyDetails
    {
        /// <summary>
        /// Gets or sets the createdAt.
        /// </summary>
        /// <value>The createdAt.</value>
        public DateTime? createdAt { get; set; }

        /// <summary>
        /// Gets or sets the createdBy.
        /// </summary>
        /// <value>The createdBy.</value>
        public string? createdBy { get; set; }

        /// <summary>
        /// Gets or sets the updatedAt.
        /// </summary>
        /// <value>The updatedAt.</value>
        public DateTime? updatedAt { get; set; }

        /// <summary>
        /// Gets or sets the updatedBy.
        /// </summary>
        /// <value>The updatedBy.</value>
        public string? updatedBy { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        public int? id { get; set; }

        /// <summary>
        /// Gets or sets the omniguid.
        /// </summary>
        /// <value>The omniguid.</value>
        public string? omniguid { get; set; }

        /// <summary>
        /// Gets or sets the columns.
        /// </summary>
        /// <value>The columns.</value>
        public List<Column>? columns { get; set; }

        /// <summary>
        /// Gets or sets the minimumSetGuid.
        /// </summary>
        /// <value>The minimumSetGuid.</value>
        public string? minimumSetGuid { get; set; }

        /// <summary>
        /// Gets or sets the auxiliaryGuid.
        /// </summary>
        /// <value>The auxiliaryGuid.</value>
        public string? auxiliaryGuid { get; set; }

        /// <summary>
        /// Gets or sets the planIds.
        /// </summary>
        /// <value>The planIds.</value>
        public string? planIds { get; set; }

        /// <summary>
        /// Gets or sets the templateName.
        /// </summary>
        /// <value>The templateName.</value>
        public string? templateName { get; set; }

        /// <summary>
        /// Gets or sets the clientName.
        /// </summary>
        /// <value>The clientName.</value>
        public string? clientName { get; set; }

        /// <summary>
        /// Gets or sets the isActive.
        /// </summary>
        /// <value>The isActive.</value>
        public bool isActive { get; set; }

        /// <summary>
        /// Gets or sets the mediaOpsState.
        /// </summary>
        /// <value>The mediaOpsState.</value>
        public string? mediaOpsState { get; set; }

        /// <summary>
        /// Gets or sets the campaignStart.
        /// </summary>
        /// <value>The campaignStart.</value>
        public DateTime? campaignStart { get; set; }

        /// <summary>
        /// Gets or sets the campaignEnd.
        /// </summary>
        /// <value>The campaignEnd.</value>
        public DateTime? campaignEnd { get; set; }
    }

    public class Column
    {
        /// <summary>
        /// Gets or sets the createdAt.
        /// </summary>
        /// <value>The createdAt.</value>
        public DateTime? createdAt { get; set; }

        /// <summary>
        /// Gets or sets the createdBy.
        /// </summary>
        /// <value>The createdBy.</value>
        public string? createdBy { get; set; }

        /// <summary>
        /// Gets or sets the updatedAt.
        /// </summary>
        /// <value>The updatedAt.</value>
        public DateTime? updatedAt { get; set; }

        /// <summary>
        /// Gets or sets the updatedBy.
        /// </summary>
        /// <value>The updatedBy.</value>
        public string? updatedBy { get; set; }

        /// <summary>
        /// Gets or sets the templateGuid.
        /// </summary>
        /// <value>The templateGuid.</value>
        public string? templateGuid { get; set; }

        /// <summary>
        /// Gets or sets the tier.
        /// </summary>
        /// <value>The tier.</value>
        public string? tier { get; set; }

        /// <summary>
        /// Gets or sets the tierOrder.
        /// </summary>
        /// <value>The tierOrder.</value>
        public int tierOrder { get; set; }

        /// <summary>
        /// Gets or sets the columnName.
        /// </summary>
        /// <value>The columnName.</value>
        public string? columnName { get; set; }

        /// <summary>
        /// Gets or sets the columnalias.
        /// </summary>
        /// <value>The columnalias.</value>
        public string? columnalias { get; set; }

        /// <summary>
        /// Gets or sets the dataType.
        /// </summary>
        /// <value>The dataType.</value>
        public string? dataType { get; set; }

        /// <summary>
        /// Gets or sets the sourceSystem.
        /// </summary>
        /// <value>The sourceSystem.</value>
        public string? sourceSystem { get; set; }

        /// <summary>
        /// Gets or sets the sourceColumnName.
        /// </summary>
        /// <value>The sourceColumnName.</value>
        public string? sourceColumnName { get; set; }

        /// <summary>
        /// Gets or sets the isPartOfTierId.
        /// </summary>
        /// <value>The isPartOfTierId.</value>
        public bool isPartOfTierId { get; set; }

        /// <summary>
        /// Gets or sets the isAGroupByColumn.
        /// </summary>
        /// <value>The isAGroupByColumn.</value>
        public bool isAGroupByColumn { get; set; }

        /// <summary>
        /// Gets or sets the isMinSetColumn.
        /// </summary>
        /// <value>The isMinSetColumn.</value>
        public bool isMinSetColumn { get; set; }

        /// <summary>
        /// Gets or sets the isPartOfDisplayName.
        /// </summary>
        /// <value>The isPartOfDisplayName.</value>
        public bool isPartOfDisplayName { get; set; }

        /// <summary>
        /// Gets or sets the isAuxiliary.
        /// </summary>
        /// <value>The isAuxiliary.</value>
        public bool isAuxiliary { get; set; }
    }
}

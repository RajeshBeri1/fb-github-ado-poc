using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.MediaopsToFlowChart.DTO
{
    /// <summary>
    /// MediaopsColumnSearchDTO
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MediaopsColumnSearchDTO
    {
        /// <summary>
        /// Gets or sets the clientId.
        /// </summary>
        /// <value>The clientId.</value>
        [DefaultValue(typeof(string), null)]
        public string? ClientId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the createdAt.
        /// </summary>
        /// <value>The createdAt.</value>
        [DefaultValue(typeof(DateTime), null)]
        public DateTime? CreatedAt { get; set; } = null;

        /// <summary>
        /// Gets or sets the createdBy.
        /// </summary>
        /// <value>The createdBy.</value>z
        [DefaultValue(typeof(string), null)]
        public string? CreatedBy { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the endDate.
        /// </summary>
        /// <value>The endDate.</value>
        [DefaultValue(typeof(string), null)]
        public string? EndDate { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        public int? Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [DefaultValue(typeof(string), null)]
        public string? Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the startDate.
        /// </summary>
        /// <value>The startDate.</value>
        [DefaultValue(typeof(string), null)]
        public string? StartDate { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>The status.</value>
        [DefaultValue(typeof(string), "Active")]
        public string? Status { get; set; }

        /// <summary>
        /// Gets or sets the updatedAt.
        /// </summary>
        /// <value>The updatedAt.</value>
        [DefaultValue(typeof(DateTime), null)]
        public DateTime? UpdatedAt { get; set; } = null;

        /// <summary>
        /// Gets or sets the updatedBy.
        /// </summary>
        /// <value>The updatedBy.</value>
        [DefaultValue(typeof(string), null)]
        public string? UpdatedBy { get; set; } = string.Empty;
    }
}

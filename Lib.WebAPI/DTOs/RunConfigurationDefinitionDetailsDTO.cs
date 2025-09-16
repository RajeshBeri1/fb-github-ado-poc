using Lib.WebAPI.Models.Flowchart;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.WebAPI.DTOs
{
    /// <summary>
    /// RunConfigurationDefinitionDetailsDTO
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class RunConfigurationDefinitionDetailsDTO
    {
        /// <summary>
        /// Gets or sets the Id.
        /// </summary>
        /// <value>The Id.</value>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the media hierarchy levels.
        /// </summary>
        /// <value>The media hierarchy levels.</value>
        public ICollection<MediaHierarchyLevelBase>? MediaHierarchyLevels { get; set; }

        /// <summary>
        /// Gets or sets the run restrictions.
        /// </summary>
        /// <value>The run restrictions.</value>
        public ICollection<RunRestriction>? RunRestrictions { get; set; }

        /// <summary>
        /// Gets or sets the CreatedByUserId.
        /// </summary>
        /// <value>The CreatedByUserId.</value>
        public Guid CreatedByUserId { get; set; }
    }
}

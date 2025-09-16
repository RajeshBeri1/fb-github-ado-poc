using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Lib.WebAPI.Enumerations;
using Lib.WebAPI.Models.Data;

namespace Lib.WebAPI.DTOs
{
    /// <summary>
    /// ReportPublishDTO
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ReportPublishDTO
    {
        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>The data.</value>
        public Data Data { get; set; } = default!;

        /// <summary>
        /// Gets or sets the content of the file.
        /// </summary>
        /// <value>The content of the file.</value>
        public byte[] FileContent { get; set; } = default!;

        /// <summary>
        /// Gets or sets the type of the file.
        /// </summary>
        /// <value>The type of the file.</value>
        public FileType FileType { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; } = default!;

        /// <summary>
        /// Gets or sets the run configuration data.
        /// </summary>
        /// <value>The run configuration data.</value>
        public RunConfigurationData RunConfigurationData { get; set; } = default!;

        /// <summary>
        /// Gets or sets the run configuration identifier.
        /// </summary>
        /// <value>The run configuration identifier.</value>
        public Guid RunConfigurationId { get; set; }

        /// <summary>
        /// Gets or sets the report comments.
        /// </summary>
        /// <value>The report comments.</value>
        public string? Comments { get; set; }
    }
}
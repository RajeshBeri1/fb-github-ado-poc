using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.DbModels
{
    /// <summary>
    /// FieldInfo
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class FieldInfo : NamedDbModelBase
    {
        /// <summary>
        /// Gets or sets the type identifier.
        /// </summary>
        /// <value>The type identifier.</value>
        public string Type { get; set; } = default!;

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets the type identifier.
        /// </summary>
        /// <value>The Type identifier.</value>
        public bool IsMetric { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets the IsCurrency identifier.
        /// </summary>
        /// <value>The IsCurrency identifier.</value>
        public bool IsCurrency { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets the IsCommon identifier.
        /// </summary>
        /// <value>The IsCommon identifier.</value>
        public bool IsCommon { get; set; }

        /// <summary>
        /// Gets or sets the moduleType identifier.
        /// </summary>
        /// <value>The moduleType identifier.</value>
        public string? ModuleType { get; set; }

    }
}
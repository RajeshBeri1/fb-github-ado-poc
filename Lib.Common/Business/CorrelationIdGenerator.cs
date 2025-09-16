using System.Diagnostics.CodeAnalysis;
using Lib.Common.Business.Interfaces;
using Microsoft.Extensions.Primitives;

namespace Lib.Common.Business
{
    /// <summary>
    /// CorrelationIdGenerator
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CorrelationIdGenerator : ICorrelationIdGenerator
    {
        /// <summary>
        /// Gets or sets the correlation identifier.
        /// </summary>
        /// <value>The correlation identifier.</value>
        public StringValues CorrelationId { get; set; } = Guid.NewGuid().ToString();
    }
}
using Microsoft.Extensions.Primitives;

namespace Lib.Common.Business.Interfaces
{
    /// <summary>
    /// ICorrelationIdGenerator
    /// </summary>
    public interface ICorrelationIdGenerator
    {
        /// <summary>
        /// Gets or sets the correlation identifier.
        /// </summary>
        /// <value>The correlation identifier.</value>
        public StringValues CorrelationId { get; set; }
    }
}
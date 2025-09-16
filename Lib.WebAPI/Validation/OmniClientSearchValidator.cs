using System.Diagnostics.CodeAnalysis;
using Lib.WebAPI.DbModels;
using Lib.WebAPI.DTOs;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// OmniClientSearchValidator
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class OmniClientSearchValidator : NamedSearchValidator<OmniClientSearchDTO, OmniClient>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OmniClientSearchValidator" />
        /// class.
        /// </summary>
        public OmniClientSearchValidator()
            : base()
        {
        }
    }
}
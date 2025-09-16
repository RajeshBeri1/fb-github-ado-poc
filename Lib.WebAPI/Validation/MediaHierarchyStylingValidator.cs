using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// MediaHierarchyStyling
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MediaHierarchyStylingValidator : AbstractValidator<MediaHierarchyStyling>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MediaHierarchyStylingValidator"
        /// /> class.
        /// </summary>
        public MediaHierarchyStylingValidator()
        {
            // TODO
        }
    }
}
using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.DTOs
{
    /// <summary>
    /// MediaHierarchyColumnsDetailsResult
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MediaHierarchyColumnsDetailsResultDTO
    {
        public List<MediaHierarchyColumnsDetailsDTO> AllColumns { get; set; }
        public List<MediaHierarchyColumnsDetailsDTO> FilteredColumns { get; set; }
    }
}

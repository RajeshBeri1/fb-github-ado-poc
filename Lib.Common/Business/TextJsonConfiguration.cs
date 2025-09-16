using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
namespace Lib.Common.Business
{

    /// <summary>
    /// TextJsonConfiguration
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class TextJsonConfiguration
    {
        /// <summary>
        /// Configures the specified settings.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public static void Configure(JsonSerializerOptions settings)
        {
            settings.MaxDepth = 64;
            settings.WriteIndented = false;
            settings.Converters.Add(new DateTimeConverter());
            settings.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            settings.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        }
    }
}
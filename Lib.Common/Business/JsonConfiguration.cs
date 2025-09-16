using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Lib.Common.Business
{
    /// <summary>
    /// JsonConfiguration
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class JsonConfiguration
    {
        /// <summary>
        /// Configures the specified settings.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public static void Configure(JsonSerializerSettings settings)
        {
            settings.Formatting = Formatting.None;
            settings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            settings.DefaultValueHandling = DefaultValueHandling.Include;
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            settings.Converters.Add(new StringEnumConverter());
            settings.ContractResolver = new DefaultContractResolver();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class MediaHierarchyInflightOverlayMigrationExtensions
    {
        /// <summary>
        /// Migrate the single inflight overlay to the list of inflight overlays.
        /// </summary>
        /// <param name="setting">media hierarchy settings</param>
        public static void MigrateInflightOverlayToList(this MediaHierarchySetting setting)
        {
            if (setting.InflightOverlayTableId != null &&
                setting.InflightOverlayTableId.ToString() != "00000000-0000-0000-0000-000000000001")
            {
                setting.InflightOverlays = new List<MediaHierarchyInflightOverlay>()
                {
                    new()
                    {
                        ColumnName = setting.InflightOverlayColumnName,
                        TableId = setting.InflightOverlayTableId.Value,
                        Order = 0,
                        Styling = setting.InflightOverlayStyling,
                    },
                };
            }
            setting.InflightOverlayColumnName = null;
            setting.InflightOverlayTableId = null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Aurora.Models
{
    public class TemplatesClientColumnAliases
    {
        public int template_tracking_id { get; set; }
        public string omniguid { get; set; }
        public string client_name { get; set; }
        public string client_id { get; set; }
        public string source_sys { get; set; }
        public string pmds_column_name { get; set; }
        public string source_column_name { get; set; }
        public string columnalias { get; set; }
        public string tier { get; set; }
        public string data_type { get; set; }
        public string designation { get; set; }
        public string template_uid { get; set; }
        public string affiliated_uid { get; set; }
        public bool is_part_of_tier_id { get; set; }
        public bool is_a_group_by_column { get; set; }
        public bool is_part_of_display_name { get; set; }
        public bool is_client_default { get; set; }
        public DateTime last_refreshed_at { get; set; }
    }
}

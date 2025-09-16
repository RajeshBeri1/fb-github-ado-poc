using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.WebAPI.Configuration
{
    public class ZstdCompressionProviderOptions
    {
        public int Level { get; set; } = 3;
        public int BufferSize { get; set; } = 81920;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.WebAPI.Helpers
{
    public interface IQueryPartBuilder
    {
        void Build(StringBuilder query, QueryParameters parameters);
    }
}

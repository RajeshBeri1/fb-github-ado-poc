using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.WebAPI.Business.Interfaces
{
    /// <summary>
    /// ICacheLogic
    /// </summary>
    public interface ICacheLogic
    {
        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="factory">The factory.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        ValueTask<TValue> GetAsync<TValue>(string key, Func<CancellationToken, Task<TValue>> factory, CancellationToken cancellationToken);
    }
}
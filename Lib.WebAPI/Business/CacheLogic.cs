using Lib.WebAPI.Business.Interfaces;
using Lib.WebAPI.Models;
using ZiggyCreatures.Caching.Fusion;

namespace Lib.WebAPI.Business
{
    /// <summary>
    /// CacheLogic
    /// </summary>
    public class CacheLogic : ICacheLogic
    {
        private readonly IFusionCache cache;
        private readonly FusionCacheConfig config;

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheLogic" /> class.
        /// </summary>
        /// <param name="cache">The cache.</param>
        /// <param name="config">The configuration.</param>
        public CacheLogic(IFusionCache cache, FusionCacheConfig config)
        {
            this.cache = cache;
            this.config = config;
        }

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="factory">The factory.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async ValueTask<TValue> GetAsync<TValue>(string key, Func<CancellationToken, Task<TValue>> factory, CancellationToken cancellationToken)
        {
            if (!config.Enabled)
            {
                return await factory(cancellationToken);
            }

            var keyName = $"{typeof(TValue).Name}_{key}";
            return (await cache.GetOrSetAsync(keyName, (ct) => factory(ct)!, token: cancellationToken))!;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.WebAPI.Extensions
{
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Safely gets a value from dictionary or returns empty collection.
        /// Prevents null reference exceptions and improves readability.
        /// </summary>
        public static IReadOnlyList<T> GetValueOrEmpty<TKey, T>(this IReadOnlyDictionary<TKey, List<T>> dictionary, TKey key)
        {
            return dictionary.TryGetValue(key, out var value) && value.Any() ? value : Array.Empty<T>();
        }

        /// <summary>
        /// Creates a read-only wrapper around a dictionary.
        /// </summary>
        public static IReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
            where TKey : notnull
        {
            return dictionary ?? throw new ArgumentNullException(nameof(dictionary));
        }
    }
}

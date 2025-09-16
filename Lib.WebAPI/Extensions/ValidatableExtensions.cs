using System.Diagnostics.CodeAnalysis;
using Throw;

namespace Lib.WebAPI.Extensions
{
    /// <summary>
    /// ValidatableExtensions
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class ValidatableExtensions
    {
        private const string AllowedSqlCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789_";

        /// <summary>
        /// Ifs the not valid SQL.
        /// </summary>
        /// <param name="validatable">The validatable.</param>
        public static ref readonly Validatable<string> IfNotValidSql(this in Validatable<string> validatable)
        {
            if (validatable.Value.ToArray().Except(AllowedSqlCharacters.ToArray()).Any())
            {
                throw new ArgumentException($"SQL Security check hit! String contains not allowed characters.");
            }

            return ref validatable;
        }
    }
}
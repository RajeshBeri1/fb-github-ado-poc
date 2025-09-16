using System.Reflection;
using Lib.Athena.Enumerations;
using Lib.WebAPI.Business;
using Lib.WebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Lib.SampleData.Business
{
    /// <summary>
    /// SampleDataDbContext
    /// </summary>
    public class SampleDataDbContext : DbContext
    {
        /// <summary>
        /// Ensures the created asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task EnsureCreatedAsync(CancellationToken cancellationToken)
        {
            await Database.EnsureCreatedAsync(cancellationToken);

            var assembly = Assembly.GetExecutingAssembly();
            var files = assembly.GetManifestResourceNames().Where(x => x.Contains("datadictionary_"));

            foreach (var file in files)
            {
                var table = file.Split("_", 2)[1].Split(".")[0];
                await SeedTableAsync(table, cancellationToken);
            }
        }

        /// <summary>
        /// <para>
        /// Override this method to configure the database (and other options) to be used
        /// for this context. This method is called for each instance of the context that
        /// is created. The base implementation does nothing.
        /// </para>
        /// <para>
        /// In situations where an instance of <see
        /// cref="T:Microsoft.EntityFrameworkCore.DbContextOptions" /> may or may not have
        /// been passed to the constructor, you can use <see
        /// cref="P:Microsoft.EntityFrameworkCore.DbContextOptionsBuilder.IsConfigured" />
        /// to determine if the options have already been set, and skip some or all of the
        /// logic in <see
        /// cref="M:Microsoft.EntityFrameworkCore.DbContext.OnConfiguring(Microsoft.EntityFrameworkCore.DbContextOptionsBuilder)"
        /// />.
        /// </para>
        /// </summary>
        /// <param name="optionsBuilder">
        /// A builder used to create or modify options for this context. Databases (and
        /// other extensions) typically define extension methods on this object that allow
        /// you to configure the context.
        /// </param>
        /// <remarks>
        /// See <see href="https://aka.ms/efcore-docs-dbcontext">DbContext lifetime,
        /// configuration, and initialization</see> for more information.
        /// </remarks>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("DataSource=file::memory:?cache=shared");

            base.OnConfiguring(optionsBuilder);
        }

        /// <summary>
        /// Override this method to further configure the model that was discovered by
        /// convention from the entity types exposed in <see
        /// cref="T:Microsoft.EntityFrameworkCore.DbSet`1" /> properties on your derived
        /// context. The resulting model may be cached and re-used for subsequent
        /// instances of your derived context.
        /// </summary>
        /// <param name="modelBuilder">
        /// The builder being used to construct the model for this context. Databases (and
        /// other extensions) typically define extension methods on this object that allow
        /// you to configure aspects of the model that are specific to a given database.
        /// </param>
        /// <remarks>
        /// <para>
        /// If a model is explicitly set on the options for this context (via <see
        /// cref="M:Microsoft.EntityFrameworkCore.DbContextOptionsBuilder.UseModel(Microsoft.EntityFrameworkCore.Metadata.IModel)"
        /// />) then this method will not be run.
        /// </para>
        /// <para>
        /// See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and
        /// relationships</see> for more information.
        /// </para>
        /// </remarks>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        private string GetSqlType(ColumnType columnType)
        {
            return columnType switch
            {
                ColumnType.Boolean => "BIT",
                ColumnType.Date => "DATE",
                ColumnType.Decimal => "DECIMAL",
                ColumnType.LongInteger => "BIGINT",
                ColumnType.String => $"VARCHAR({int.MaxValue})",
                _ => throw new NotSupportedException($"Column type {columnType} is not supported."),
            };
        }

        private string GetValue(Dictionary<string, object?> data, string name)
        {
            if (!data.ContainsKey(name))
            {
                return "NULL";
            }

            var value = data[name];

            if (value == null)
            {
                return "NULL";
            }

            return value switch
            {
                DateTime dateTime => $"DATE({DataControllerLogic.SqlQuote(dateTime.ToString("yyyy-MM-dd"))})",
                string str => DataControllerLogic.SqlQuote(str).Replace("{", "{{").Replace("}", "}}"),
                _ => value.ToString()!.Replace(",", "."),
            };
        }

        private async Task SeedTableAsync(string table, CancellationToken cancellationToken)
        {
            var dataDictionary = AthenaSampleDataQueryLogic.GetData<List<DataDictionaryColumn>>($"datadictionary_{table}.json");

            var columns = string.Join(",", dataDictionary.Select(x => $"{Environment.NewLine}{x.Name} {GetSqlType(x.Type)} NULL"));

            var sql = $"CREATE TABLE {table} ({columns})";

            await Database.ExecuteSqlRawAsync(sql, cancellationToken);

            var data = AthenaSampleDataQueryLogic.GetData<List<Dictionary<string, object?>>>($"data_{table}.json");

            foreach (var item in data)
            {
                var cols = string.Join($",{Environment.NewLine}", dataDictionary.Select(x => x.Name));
                var values = string.Join($",{Environment.NewLine}", dataDictionary.Select(x => GetValue(item, x.Name)));

                var insert = $"INSERT INTO {table}({cols}) VALUES({values})";

                await Database.ExecuteSqlRawAsync(insert, cancellationToken);
            }
        }
    }
}
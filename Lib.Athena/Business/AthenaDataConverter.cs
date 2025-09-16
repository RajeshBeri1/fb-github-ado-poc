using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;
using Amazon.Athena;
using Amazon.Athena.Model;
using Lib.Athena.Enumerations;
using Lib.Athena.Models;
using Lib.Common.Business.Interfaces;
using Microsoft.Extensions.Logging;
using Throw;

namespace Lib.Athena.Business
{
    /// <summary>
    /// AthenaDataConverter
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class AthenaDataConverter
    {
        private readonly ILog<AthenaDataConverter> log;
        private readonly Regex regexForDatetime = new Regex(@"\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}.\d{3}");
        /// <summary>
        /// Initializes a new instance of the <see cref="AthenaDataConverter" /> class.
        /// </summary>
        /// <param name="log">The log.</param>
        public AthenaDataConverter(ILog<AthenaDataConverter> log)
        {
            this.log = log;
        }

        /// <summary>
        /// Gets the type of the column.
        /// </summary>
        /// <param name="columnInfo">The column information.</param>
        public static ColumnType GetColumnType(ColumnInfo columnInfo)
        {
            return columnInfo.Type switch
            {
                "boolean" => ColumnType.Boolean,
                "tinyint" or "smallint" or "int" or "integer" or "bigint" => ColumnType.LongInteger,
                "double" or "float" or "decimal" => ColumnType.Decimal,
                "char" or "varchar" or "string" => ColumnType.String,
                "date" or "timestamp" or "timestamp with time zone" => ColumnType.Date,
                "jsonb" or "json" => ColumnType.Jsonb,
                "binary" or "array" or "map" or "struct" => throw new NotSupportedException($"Athena data type {columnInfo.Type} is currently not supported."),
                _ => throw new NotSupportedException($"Athena data type {columnInfo.Type} is unknown."),
            };
        }

        /// <summary>
        /// Converts the specified athena query result.
        /// </summary>
        /// <param name="athenaQueryResult">The athena query result.</param>
        public ICollection<Dictionary<string, object?>> Convert(AthenaQueryResult athenaQueryResult)
        {
            var result = new List<Dictionary<string, object?>>();

            for (int r = 0; r < athenaQueryResult.Rows.Count; r++)
            {
                // detect first row without data -> SELECTs return all column names in
                // first row
                var firstRow = !athenaQueryResult.Rows[r].Data.Select(x => x.VarCharValue).Except(athenaQueryResult.ColumnInfo
                    .Select(x => x.Name)).Any();

                if (firstRow)
                {
                    continue;
                }

                var row = new Dictionary<string, object?>();
                var rowColumnCount = athenaQueryResult.Rows[r].Data.Count;
                for (int c = 0; c < athenaQueryResult.ColumnInfo.Count && rowColumnCount >= c + 1; c++)
                {
                    var info = athenaQueryResult.ColumnInfo[c];

                    if (!row.ContainsKey(info.Name))
                    {
                        try
                        {
                            row.Add(info.Name, Convert(athenaQueryResult.Rows[r].Data[c], info));
                        }
                        catch (Exception ex)
                        {
                            log.Add(LogLevel.Warning, $"Error while getting field {info.Name} value. {ex}");
                        }
                    }
                    else
                    {
                        if (row[info.Name] == null)
                        {
                            try
                            {
                                row[info.Name] = Convert(athenaQueryResult.Rows[r].Data[c], info);
                            }
                            catch (Exception)
                            {
                                // log.Add(LogLevel.Warning, $"Error while getting field {info.Name} value. {ex}");
                            }
                        }
                    }
                }

                result.Add(row);
            }

            return result;
        }

        /// <summary>
        /// Converts the specified column information.
        /// </summary>
        /// <param name="datum">The datum.</param>
        /// <param name="columnInfo">The column information.</param>
        public object? Convert(Datum datum, ColumnInfo columnInfo)
        {
            if (datum.VarCharValue == null)
            {
                return null;
            }

            if (columnInfo.Nullable == ColumnNullable.NULLABLE)
            {
                throw new NotImplementedException($"Null values needs to be implemented.");
            }

            var type = GetColumnType(columnInfo);

            return type switch
            {
                ColumnType.Boolean => ConvertBool(datum),
                ColumnType.LongInteger => ConvertLong(datum),
                ColumnType.Decimal => ConvertDecimal(datum),
                ColumnType.String => ConvertString(datum),
                ColumnType.Date => ConvertDate(datum),
                _ => throw new NotSupportedException($"Column type {type} is not supported."),
            };
        }

        private bool ConvertBool(Datum datum)
        {
            if (!bool.TryParse(datum.VarCharValue, out var result))
            {
                throw new InvalidDataException($"Failed to parse bool: {datum.VarCharValue}");
            }
            return result;
        }

        private DateTime ConvertDate(Datum datum)
        {
            if (regexForDatetime.IsMatch(datum.VarCharValue))
            {
                var finalDate = datum.VarCharValue.Substring(0, 19);
                if (!DateTime.TryParse(finalDate, out var result))
                {
                    throw new InvalidDataException($"Failed to parse date time: {datum.VarCharValue}");
                }
                return result;
            }
            else
            {
                if (!DateTime.TryParse(datum.VarCharValue, out var result))
                {
                    throw new InvalidDataException($"Failed to parse date time: {datum.VarCharValue}");
                }
                return result;
            }
        }

        private decimal ConvertDecimal(Datum datum)
        {
            if (!decimal.TryParse(datum.VarCharValue, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
            {
                throw new InvalidDataException($"Failed to parse decimal: {datum.VarCharValue}");
            }
            return result;
        }

        private long ConvertLong(Datum datum)
        {
            if (!long.TryParse(datum.VarCharValue, out var result))
            {
                throw new InvalidDataException($"Failed to parse long: {datum.VarCharValue}");
            }
            return result;
        }

        private string ConvertString(Datum datum)
        {
            return datum.VarCharValue;
        }
    }
}
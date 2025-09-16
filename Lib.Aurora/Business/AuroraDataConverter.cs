using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Lib.Aurora.Enumerations;
using Lib.Common.Business.Interfaces;
using Microsoft.Extensions.Logging;
using Npgsql;
using Throw;

namespace Lib.Aurora.Business;

/// <summary>
/// AthenaDataConverter
/// </summary>
[ExcludeFromCodeCoverage]
public class AuroraDataConverter
{
    private readonly ILog<AuroraDataConverter> log;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuroraDataConverter" /> class.
    /// </summary>
    /// <param name="log">The log.</param>
    public AuroraDataConverter(ILog<AuroraDataConverter> log)
    {
        this.log = log;
    }

    /// <summary>
    /// Gets the type of the column.
    /// </summary>
    /// <param name="columnInfo">The column information.</param>
    public static ColumnType GetColumnType(DataColumn columnInfo)
    {
        return columnInfo.DataType.Name switch
        {
            "boolean" => ColumnType.Boolean,
            "tinyint" or "smallint" or "int" or "integer" or "bigint" => ColumnType.LongInteger,
            "double" or "float" or "decimal" => ColumnType.Decimal,
            "char" or "varchar" or "string" => ColumnType.String,
            "date" or "timestamp" => ColumnType.Date,
            "jsonb" or "json" => ColumnType.Jsonb,
            "binary" or "array" or "map" or "struct" => throw new NotSupportedException($"Athena data type {columnInfo.DataType} is currently not supported."),
            _ => throw new NotSupportedException($"Athena data type {columnInfo.DataType} is unknown."),
        };
    }

    ///// <summary>
    ///// Converts the specified athena query result.
    ///// </summary>
    ///// <param name="athenaQueryResult">The athena query result.</param>
    //public ICollection<Dictionary<string, object?>> Convert(NpgsqlDataReader queryResult)
    //{
    //    var result = new List<Dictionary<string, object?>>();

    //    for (int r = 0; r < athenaQueryResult.Rows.Count; r++)
    //    {
    //        // detect first row without data -> SELECTs return all column names in
    //        // first row
    //        var firstRow = !athenaQueryResult.Rows[r].Data.Select(x => x.VarCharValue).Except(athenaQueryResult.ColumnInfo
    //            .Select(x => x.Name)).Any();

    //        if (firstRow)
    //        {
    //            continue;
    //        }

    //        var row = new Dictionary<string, object?>();

    //        for (int c = 0; c < athenaQueryResult.ColumnInfo.Count; c++)
    //        {
    //            var info = athenaQueryResult.ColumnInfo[c];

    //            if (row.ContainsKey(info.Name))
    //            {
    //                log.Add(LogLevel.Warning, $"Field {info.Name} already in data dictionary.");
    //            }
    //            else
    //            {
    //                row.Add(info.Name, Convert(athenaQueryResult.Rows[r].Data[c], info));
    //            }
    //        }

    //        result.Add(row);
    //    }

    //    return result;
    //}

    /// <summary>
    /// Converts the specified column information.
    /// </summary>
    /// <param name="datum">The datum.</param>
    /// <param name="columnInfo">The column information.</param>
    public object? Convert(NpgsqlDataReader datum, DataColumn columnInfo)
    {
        if (datum.GetValue(columnInfo.ColumnName) == null)
        {
            return null;
        }

        if (columnInfo.AllowDBNull)
        {
            throw new NotImplementedException($"Null values needs to be implemented.");
        }

        var type = GetColumnType(columnInfo);

        return type switch
        {
            ColumnType.Boolean => datum.GetBoolean(columnInfo.ColumnName),
            ColumnType.LongInteger => datum.GetInt64(columnInfo.ColumnName),
            ColumnType.Decimal => datum.GetDecimal(columnInfo.ColumnName),
            ColumnType.String => datum.GetString(columnInfo.ColumnName),
            ColumnType.Date => datum.GetDateTime(columnInfo.ColumnName),
            _ => throw new NotSupportedException($"Column type {type} is not supported."),
        };
    }

    private bool ConvertBool(string datum)
    {
        if (!bool.TryParse(datum, out var result))
        {
            throw new InvalidDataException($"Failed to parse bool: {datum}");
        }
        return result;
    }
}
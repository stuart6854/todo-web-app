using System.Data;
using System.Data.Common;
using System.Globalization;
using Domain.Extensions;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Infrastructure;

public class SanitizeCommandInterceptor : DbCommandInterceptor
{
    public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
    {
        ApplyCorrectCommand(command);
        return result;
    }

    public override InterceptionResult<object> ScalarExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<object> result)
    {
        ApplyCorrectCommand(command);
        return result;
    }

    public override InterceptionResult<int> NonQueryExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<int> result)
    {
        ApplyCorrectCommand(command);
        return result;
    }

    public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command, CommandEventData eventData,
        InterceptionResult<DbDataReader> result,
        CancellationToken cancellationToken = new CancellationToken())
    {
        ApplyCorrectCommand(command);
        return ValueTask.FromResult(result);
    }

    public override ValueTask<InterceptionResult<object>> ScalarExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<object> result,
        CancellationToken cancellationToken = new CancellationToken())
    {
        ApplyCorrectCommand(command);
        return ValueTask.FromResult(result);
    }

    public override ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<int> result,
        CancellationToken cancellationToken = new CancellationToken())
    {
        ApplyCorrectCommand(command);
        return ValueTask.FromResult(result);
    }

    private static void ApplyCorrectCommand(DbCommand command)
    {
        command.CommandText = command.CommandText.Sanitize();

        foreach (DbParameter parameter in command.Parameters)
        {
            switch (parameter.DbType)
            {
                case DbType.AnsiString:
                case DbType.AnsiStringFixedLength:
                case DbType.String:
                case DbType.StringFixedLength:
                case DbType.Xml:
                    if (!(parameter.Value is DBNull) && parameter.Value is string)
                    {
                        parameter.Value = Convert.ToString(parameter.Value, CultureInfo.InvariantCulture).Sanitize();
                    }

                    break;
            }
        }
    }
}
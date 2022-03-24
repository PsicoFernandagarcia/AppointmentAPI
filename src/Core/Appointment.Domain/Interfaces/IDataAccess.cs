using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Appointment.Domain.Interfaces
{
    public interface IDataAccess
    {
        [Obsolete]
        Result<Maybe<IEnumerable<T>>> GetRecords<T>(string sql, object parameters = null, CommandType commandType = CommandType.StoredProcedure);
        Task<Result<Maybe<IEnumerable<T>>>> GetRecordsAsync<T>(string sql, object parameters = null, CommandType commandType = CommandType.StoredProcedure);
        [Obsolete]
        Result<Maybe<T>> GetRecord<T>(string sql, object parameters = null, CommandType commandType = CommandType.StoredProcedure);
        Task<Result<Maybe<T>>> GetRecordAsync<T>(string sql, object parameters = null, CommandType commandType = CommandType.StoredProcedure);
        [Obsolete]
        Result Execute(string sql, object parameters = null, CommandType commandType = CommandType.StoredProcedure);
        Task<Result> ExecuteAsync(string sql, object parameters = null, CommandType commandType = CommandType.StoredProcedure);
        [Obsolete]
        Result ExecuteMany(params (string storeProcedureName, object parameters)[] operationsToExeute);
        Task<Result> ExecuteManyAsync(params (string storeProcedureName, object parameters)[] operationsToExeute);
    }
}

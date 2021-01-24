using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace PostgreSQL.ApplicationInsights.Interceptor
{
    public class PostgreSqlInterceptor : IDbCommandInterceptor, IDbTransactionInterceptor, IDbConnectionInterceptor
    {
        private readonly TelemetryClient _telemetryClient;

        private readonly ConcurrentDictionary<Guid, Lazy<IOperationHolder<RequestTelemetry>>> _operationHolders
            = new ConcurrentDictionary<Guid, Lazy<IOperationHolder<RequestTelemetry>>>();

        public PostgreSqlInterceptor(TelemetryClient telemetryClient)
        {
            _telemetryClient = telemetryClient;
        }


        public InterceptionResult<DbCommand> CommandCreating(CommandCorrelatedEventData eventData,
            InterceptionResult<DbCommand> result)
        {
            TrackEvent(nameof(CommandCreated), eventData.Connection, eventData.ConnectionId, eventData.StartTime);

            return result;
        }

        public DbCommand CommandCreated(CommandEndEventData eventData, DbCommand result)
        {
            TrackEvent(nameof(CommandCreated), eventData.Connection, eventData.ConnectionId);
            return result;
        }

        public InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData,
            InterceptionResult<DbDataReader> result)
        {
            TrackEvent(nameof(ReaderExecuting), eventData.Connection, command, eventData.ConnectionId);
            return result;
        }


        public InterceptionResult<object> ScalarExecuting(DbCommand command, CommandEventData eventData,
            InterceptionResult<object> result)
        {
            TrackEvent(nameof(ScalarExecuting), eventData.Connection, command, eventData.ConnectionId);
            return result;
        }

        public InterceptionResult<int> NonQueryExecuting(DbCommand command, CommandEventData eventData,
            InterceptionResult<int> result)
        {
            TrackEvent(nameof(NonQueryExecuting), eventData.Connection, command, eventData.ConnectionId);
            return result;
        }


        public ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command,
            CommandEventData eventData, InterceptionResult<DbDataReader> result,
            CancellationToken cancellationToken = new CancellationToken())
        {
            TrackEvent(nameof(ReaderExecutingAsync), eventData.Connection, command, eventData.ConnectionId);
            return ValueTask.FromResult(result);
        }

        public ValueTask<InterceptionResult<object>> ScalarExecutingAsync(DbCommand command, CommandEventData eventData,
            InterceptionResult<object> result,
            CancellationToken cancellationToken = new CancellationToken())
        {
            TrackEvent(nameof(ScalarExecutingAsync), eventData.Connection, command, eventData.ConnectionId);
            return ValueTask.FromResult(result);
        }

        public ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(DbCommand command, CommandEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = new CancellationToken())
        {
            TrackEvent(nameof(NonQueryExecutingAsync), eventData.Connection, command, eventData.ConnectionId);
            return ValueTask.FromResult(result);
        }

        public DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData, DbDataReader result)
        {
            TrackEvent(nameof(ReaderExecuted), eventData.Connection, command, eventData.ConnectionId, result);
            return result;
        }

        public object ScalarExecuted(DbCommand command, CommandExecutedEventData eventData, object result)
        {
            TrackEvent(nameof(NonQueryExecuted), eventData.Connection, command, eventData.ConnectionId, result);
            return result;
        }

        public int NonQueryExecuted(DbCommand command, CommandExecutedEventData eventData, int result)
        {
            TrackEvent(nameof(NonQueryExecuted), eventData.Connection, command, eventData.ConnectionId, result);
            return result;
        }

        public ValueTask<DbDataReader> ReaderExecutedAsync(DbCommand command, CommandExecutedEventData eventData,
            DbDataReader result,
            CancellationToken cancellationToken = new CancellationToken())
        {
            TrackEvent(nameof(ReaderExecutedAsync), eventData.Connection, command, eventData.ConnectionId, result);
            return ValueTask.FromResult(result);
        }

        public ValueTask<object> ScalarExecutedAsync(DbCommand command, CommandExecutedEventData eventData,
            object result,
            CancellationToken cancellationToken = new CancellationToken())
        {
            TrackEvent(nameof(ScalarExecutedAsync), eventData.Connection, command, eventData.ConnectionId, result);
            return ValueTask.FromResult(result);
        }

        public ValueTask<int> NonQueryExecutedAsync(DbCommand command, CommandExecutedEventData eventData, int result,
            CancellationToken cancellationToken = new CancellationToken())
        {
            TrackEvent(nameof(ReaderExecutedAsync), eventData.Connection, command, eventData.ConnectionId, result);

            return ValueTask.FromResult(result);
        }

        public void CommandFailed(DbCommand command, CommandErrorEventData eventData)
        {
            TrackEvent(nameof(DataReaderDisposing), eventData.Connection, eventData.ConnectionId, eventData.Duration);
        }

        public Task CommandFailedAsync(DbCommand command, CommandErrorEventData eventData,
            CancellationToken cancellationToken = new CancellationToken())
        {
            TrackEvent(nameof(DataReaderDisposing), eventData.Connection, eventData.ConnectionId, eventData.Duration);

            return Task.CompletedTask;
        }

        public InterceptionResult DataReaderDisposing(DbCommand command, DataReaderDisposingEventData eventData,
            InterceptionResult result)
        {
            TrackEvent(nameof(DataReaderDisposing), command.Connection, eventData.ConnectionId, eventData.Duration);

            return result;
        }


        public InterceptionResult<DbTransaction> TransactionStarting(DbConnection connection,
            TransactionStartingEventData eventData,
            InterceptionResult<DbTransaction> result)
        {
            TrackEvent(nameof(TransactionStarting), connection, eventData.ConnectionId, eventData.StartTime);

            return result;
        }

        public DbTransaction TransactionStarted(DbConnection connection, TransactionEndEventData eventData,
            DbTransaction result)
        {
            TrackEvent(nameof(TransactionStarted), connection, eventData.ConnectionId);

            return result;
        }

        public ValueTask<InterceptionResult<DbTransaction>> TransactionStartingAsync(DbConnection connection,
            TransactionStartingEventData eventData,
            InterceptionResult<DbTransaction> result, CancellationToken cancellationToken = new CancellationToken())
        {
            TrackEvent(nameof(TransactionStartingAsync), connection, eventData.ConnectionId, eventData.StartTime);
            return ValueTask.FromResult(result);
        }

        public ValueTask<DbTransaction> TransactionStartedAsync(DbConnection connection,
            TransactionEndEventData eventData,
            DbTransaction result,
            CancellationToken cancellationToken = new CancellationToken())
        {
            TrackEvent(nameof(TransactionStartedAsync), connection, eventData.ConnectionId);
            return ValueTask.FromResult(result);
        }

        public DbTransaction TransactionUsed(DbConnection connection, TransactionEventData eventData,
            DbTransaction result)
        {
            TrackEvent(nameof(TransactionUsed), connection, eventData.ConnectionId);
            return result;
        }

        public ValueTask<DbTransaction> TransactionUsedAsync(DbConnection connection, TransactionEventData eventData,
            DbTransaction result,
            CancellationToken cancellationToken = new CancellationToken())
        {
            TrackEvent(nameof(TransactionUsed), connection, eventData.ConnectionId);
            return ValueTask.FromResult(result);
        }

        public InterceptionResult TransactionCommitting(DbTransaction transaction, TransactionEventData eventData,
            InterceptionResult result)
        {
            TrackEvent(nameof(TransactionCommitting), transaction.Connection, eventData.ConnectionId);
            return result;
        }

        public void TransactionCommitted(DbTransaction transaction, TransactionEndEventData eventData)
        {
            TrackEvent(nameof(TransactionCommitted), transaction.Connection, eventData.ConnectionId);
        }

        public ValueTask<InterceptionResult> TransactionCommittingAsync(DbTransaction transaction,
            TransactionEventData eventData, InterceptionResult result,
            CancellationToken cancellationToken = new CancellationToken())
        {
            TrackEvent(nameof(TransactionCommittingAsync), transaction.Connection, eventData.ConnectionId);
            return ValueTask.FromResult(result);
        }

        public Task TransactionCommittedAsync(DbTransaction transaction, TransactionEndEventData eventData,
            CancellationToken cancellationToken = new CancellationToken())
        {
            TrackEvent(nameof(TransactionCommittedAsync), transaction.Connection, eventData.ConnectionId);
            return Task.CompletedTask;
        }

        public InterceptionResult TransactionRollingBack(DbTransaction transaction, TransactionEventData eventData,
            InterceptionResult result)
        {
            TrackEvent(nameof(TransactionCommitted), transaction.Connection, eventData.ConnectionId);
            return result;
        }

        public void TransactionRolledBack(DbTransaction transaction, TransactionEndEventData eventData)
        {
            TrackEvent(nameof(TransactionRolledBack), transaction.Connection, eventData.ConnectionId);
        }

        public ValueTask<InterceptionResult> TransactionRollingBackAsync(DbTransaction transaction,
            TransactionEventData eventData, InterceptionResult result,
            CancellationToken cancellationToken = new CancellationToken())
        {
            TrackEvent(nameof(TransactionRollingBackAsync), transaction.Connection, eventData.ConnectionId);
            return ValueTask.FromResult(result);
        }

        public Task TransactionRolledBackAsync(DbTransaction transaction, TransactionEndEventData eventData,
            CancellationToken cancellationToken = new CancellationToken())
        {
            TrackEvent(nameof(TransactionRolledBackAsync), transaction.Connection, eventData.ConnectionId);
            return Task.CompletedTask;
        }

        public InterceptionResult CreatingSavepoint(DbTransaction transaction, TransactionEventData eventData,
            InterceptionResult result)
        {
            return result;
        }

        public void CreatedSavepoint(DbTransaction transaction, TransactionEventData eventData)
        {
        }

        public ValueTask<InterceptionResult> CreatingSavepointAsync(DbTransaction transaction,
            TransactionEventData eventData, InterceptionResult result,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return ValueTask.FromResult(result);
        }

        public Task CreatedSavepointAsync(DbTransaction transaction, TransactionEventData eventData,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.CompletedTask;
        }

        public InterceptionResult RollingBackToSavepoint(DbTransaction transaction, TransactionEventData eventData,
            InterceptionResult result)
        {
            return result;
        }

        public void RolledBackToSavepoint(DbTransaction transaction, TransactionEventData eventData)
        {
        }

        public ValueTask<InterceptionResult> RollingBackToSavepointAsync(DbTransaction transaction,
            TransactionEventData eventData,
            InterceptionResult result, CancellationToken cancellationToken = new CancellationToken())
        {
            return ValueTask.FromResult(result);
        }

        public Task RolledBackToSavepointAsync(DbTransaction transaction, TransactionEventData eventData,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.CompletedTask;
        }

        public InterceptionResult ReleasingSavepoint(DbTransaction transaction, TransactionEventData eventData,
            InterceptionResult result)
        {
            return result;
        }

        public void ReleasedSavepoint(DbTransaction transaction, TransactionEventData eventData)
        {
        }

        public ValueTask<InterceptionResult> ReleasingSavepointAsync(DbTransaction transaction,
            TransactionEventData eventData, InterceptionResult result,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return ValueTask.FromResult(result);
        }

        public Task ReleasedSavepointAsync(DbTransaction transaction, TransactionEventData eventData,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.CompletedTask;
        }

        public void TransactionFailed(DbTransaction transaction, TransactionErrorEventData eventData)
        {
            TrackEvent(nameof(TransactionFailed), transaction.Connection, eventData.ConnectionId);
        }

        public Task TransactionFailedAsync(DbTransaction transaction, TransactionErrorEventData eventData,
            CancellationToken cancellationToken = new CancellationToken())
        {
            TrackEvent(nameof(TransactionFailedAsync), transaction.Connection, eventData.ConnectionId);
            return Task.CompletedTask;
        }

        public InterceptionResult ConnectionOpening(DbConnection connection, ConnectionEventData eventData,
            InterceptionResult result)
        {
            return result;
        }

        public ValueTask<InterceptionResult> ConnectionOpeningAsync(DbConnection connection,
            ConnectionEventData eventData,
            InterceptionResult result,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return ValueTask.FromResult(result);
        }

        public void ConnectionOpened(DbConnection connection, ConnectionEndEventData eventData)
        {
            TrackEvent(nameof(ConnectionOpened), connection, eventData.ConnectionId);
        }

        public Task ConnectionOpenedAsync(DbConnection connection, ConnectionEndEventData eventData,
            CancellationToken cancellationToken = new CancellationToken())
        {
            TrackEvent(nameof(ConnectionOpenedAsync), connection, eventData.ConnectionId);
            return Task.CompletedTask;
        }

        public InterceptionResult ConnectionClosing(DbConnection connection, ConnectionEventData eventData,
            InterceptionResult result)
        {
            TrackEvent(nameof(ConnectionClosing), connection, eventData.ConnectionId);
            return result;
        }

        public ValueTask<InterceptionResult> ConnectionClosingAsync(DbConnection connection,
            ConnectionEventData eventData,
            InterceptionResult result)
        {
            TrackEvent(nameof(ConnectionClosingAsync), connection, eventData.ConnectionId);
            return ValueTask.FromResult(result);
        }

        public void ConnectionClosed(DbConnection connection, ConnectionEndEventData eventData)
        {
            TrackEvent(nameof(ConnectionClosed), connection, eventData.ConnectionId);
            RemoveOperationHolder(eventData.ConnectionId);
        }

        public Task ConnectionClosedAsync(DbConnection connection, ConnectionEndEventData eventData)
        {
            TrackEvent(nameof(ConnectionClosedAsync), connection, eventData.ConnectionId);
            RemoveOperationHolder(eventData.ConnectionId);
            return Task.CompletedTask;
        }

        public void ConnectionFailed(DbConnection connection, ConnectionErrorEventData eventData)
        {
            TrackEvent(nameof(ConnectionFailed), connection, eventData.ConnectionId);
            RemoveOperationHolder(eventData.ConnectionId);
        }

        public Task ConnectionFailedAsync(DbConnection connection, ConnectionErrorEventData eventData,
            CancellationToken cancellationToken = new CancellationToken())
        {
            TrackEvent(nameof(ConnectionFailedAsync), connection, eventData.ConnectionId);
            RemoveOperationHolder(eventData.ConnectionId);
            return Task.CompletedTask;
        }


        private Lazy<IOperationHolder<RequestTelemetry>> StartOperation(Npgsql.NpgsqlConnection connection,
            Guid connectionId)
        {
            return _operationHolders.GetOrAdd(connectionId,
                x => new Lazy<IOperationHolder<RequestTelemetry>>(() =>
                {
                    var operationName = $"{connection.Database}";
                    var dependencyHolder = _telemetryClient.StartOperation<DependencyTelemetry>(operationName);

                    dependencyHolder.Telemetry.Target = $"{connection.Host}.{connection.Port}";

                    dependencyHolder.Telemetry.Type = "SQL";
                    dependencyHolder.Dispose();

                    var operationHolder = _telemetryClient.StartOperation<RequestTelemetry>(operationName + " request");
                    return operationHolder;
                }));
        }

        private void RemoveOperationHolder(Guid connectionId)
        {
            if (!_operationHolders.TryGetValue(connectionId, out var value)) return;

            value.Value.Dispose();
            _operationHolders.TryRemove(connectionId, out _);
        }

        private static string GetParameters(DbCommand command)
        {
            var parameters = (from DbParameter commandParameter in command.Parameters
                select commandParameter.ParameterName + ": " + commandParameter.Value);
            return string.Join(',', parameters);
        }

        private void TrackEventWithParameters(string name, DbConnection connection, Guid connectionId,
            Dictionary<string, string> parameters)
        {
            StartOperation(connection as Npgsql.NpgsqlConnection, connectionId);
            parameters.Add("ConnectionId", connectionId.ToString());
            var eventTelemetry = new EventTelemetry($"Database Event: {name}");
            foreach (var (key, value) in parameters)
            {
                eventTelemetry.Properties.Add(key, value);
            }
            
            _telemetryClient.TrackEvent(eventTelemetry);
        }

        private void TrackEvent(string name, DbConnection connection, Guid connectionId)
        {
            TrackEventWithParameters(name, connection, connectionId, new Dictionary<string, string>());
        }

        private void TrackEvent(string name, DbConnection connection, Guid connectionId, DateTimeOffset startTime)
        {
            TrackEventWithParameters(name, connection, connectionId, new Dictionary<string, string>
            {
                {"StartTime", startTime.ToString()}
            });
        }

        private void TrackEvent(string name, DbConnection connection, Guid connectionId, TimeSpan duration)
        {
            TrackEventWithParameters(name, connection, connectionId, new Dictionary<string, string>
            {
                {"Duration", duration.ToString()}
            });
        }

        private void TrackEvent(string name, DbConnection connection, DbCommand command, Guid connectionId)
        {
            TrackEventWithParameters(name, connection, connectionId, new Dictionary<string, string>
            {
                {"SQL", command.CommandText},
                {"Parameters", GetParameters(command)}
            });
        }

        private void TrackEvent(string name, DbConnection connection, DbCommand command, Guid connectionId,
            object result)
        {
            TrackEventWithParameters(name, connection, connectionId, new Dictionary<string, string>
            {
                {"SQL", command.CommandText},
                {"Parameters", GetParameters(command)},
                {"Result", result?.ToString()}
            });
        }

        private void TrackEvent(string name, DbConnection connection, DbCommand command, Guid connectionId,
            DbDataReader result)
        {
            TrackEventWithParameters(name, connection, connectionId, new Dictionary<string, string>
            {
                {"SQL", command.CommandText},
                {"Parameters", GetParameters(command)},
                {"HasRows", result.HasRows.ToString()}
            });
        }
    }
}
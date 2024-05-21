using EventTracker.Interfaces;
using EventTracker.Models;
using System.Data.SqlClient;
using System.Text.Json;

namespace EventTracker.Database;

internal class EventRepository
{
    private readonly DatabaseHelper _dbHelper;

    public EventRepository(ConnectionString connectionString)
    {
        _dbHelper = new DatabaseHelper(connectionString);
    }

    internal async Task<bool> CheckDatabase()
    {
        var query = @$"
            SELECT CASE
                WHEN EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'{_dbHelper.ConnectionString.ApplicationName}')
                THEN 'true'
                ELSE 'false'
            END";

        return await _dbHelper.ExecuteSqlCommandWithTransaction(
            query,
            async command => Convert.ToBoolean(await command.ExecuteScalarAsync()));
    }

    internal async Task CreateDatabase()
    {
        var query = @$"
            CREATE TABLE Events.dbo.{_dbHelper.ConnectionString.ApplicationName} (
                Id          INT             PRIMARY KEY IDENTITY(1, 1),
                StreamID    INT             NOT NULL,
                Event       NVARCHAR(MAX)   NOT NULL,
                Timestamp   DATETIME2       NOT NULL
            );";

        await _dbHelper.ExecuteSqlCommandWithTransaction(
            query,
            async command => await command.ExecuteScalarAsync());
    }

    internal async Task AddEvent(IEvent @event, string json)
    {
        var query = @$"
            INSERT INTO Events.dbo.{_dbHelper.ConnectionString.ApplicationName} (StreamId, Event, Timestamp)
            VALUES (@streamId, @event, @timestamp)";

        List<SqlParameter> parameters = new List<SqlParameter>
        {
            new SqlParameter("streamId", @event.StreamId),
            new SqlParameter("event", json),
            new SqlParameter("timestamp", @event.Timestamp),
        };

        await _dbHelper.ExecuteSqlCommandWithTransaction(
            query,
            async command => await command.ExecuteScalarAsync(),
            parameters);
    }

    internal async Task<List<IEvent>> GetEvents(int streamId, JsonSerializerOptions options)
    {
        var query = @$"SELECT StreamID, Event FROM Events.dbo.Test WHERE StreamID = @streamId ORDER BY Timestamp";

        List<SqlParameter> parameters = new List<SqlParameter>
        {
            new SqlParameter("streamId", streamId)
        };

        return await _dbHelper.ExecuteSqlCommandWithTransaction(query, async command =>
        {
            using (SqlDataReader reader = await command.ExecuteReaderAsync())
            {
                var result = new List<IEvent>();

                while (await reader.ReadAsync())
                {
                    string json = reader["Event"].ToString();
                    result.Add(JsonSerializer.Deserialize<IEvent>(json, options));
                }

                return result;
            }
        }, parameters);
    }
}

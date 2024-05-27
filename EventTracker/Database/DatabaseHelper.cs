using EventTracker.Models;
using System.Data.SqlClient;

namespace EventTracker.Database;

public delegate Task<T> SqlCommandExecutorAsync<T>(SqlCommand command);

internal class DatabaseHelper
{
    public ConnectionString ConnectionString;

    public DatabaseHelper(ConnectionString connectionString)
    {
        ConnectionString = connectionString;
    }

    public async Task<T> ExecuteSqlCommandWithTransaction<T>(string sqlCommandText, SqlCommandExecutorAsync<T> executor, List<SqlParameter> parameters = null)
    {
        T result = default(T);

        using (SqlConnection connection = new SqlConnection(ConnectionString.ToString()))
        {
            await connection.OpenAsync();

            SqlTransaction transaction = (SqlTransaction)await connection.BeginTransactionAsync();

            try
            {
                using (SqlCommand command = new SqlCommand(sqlCommandText, connection, transaction))
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters.ToArray());
                    }

                    result = await executor(command);
                }

                await transaction.CommitAsync();
            }
            catch (SqlException ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"SQL error: {ex.Message}");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"General error: {ex.Message}");
            }
        }

        return result;
    }

}

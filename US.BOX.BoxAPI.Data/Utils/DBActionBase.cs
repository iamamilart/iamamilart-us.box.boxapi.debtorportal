using System.Data;
using System.Data.SqlClient;


namespace US.BOX.BoxAPI.Data.Utils
{
    public abstract class DBActionBase<T> : IDisposable
    {
        protected SqlConnection _connection;


        protected async Task OpenConnectionAsync(CancellationToken cancellationToken = default)
        {
            if (_connection.State != ConnectionState.Open)
            {
                await _connection.OpenAsync(cancellationToken);
            }
        }

        protected async Task<SqlCommand> CreateCommandAsync(string commandText, CommandType commandType, string connectionString, CancellationToken cancellationToken = default)
        {
            _connection = new SqlConnection(connectionString);
            await OpenConnectionAsync(cancellationToken);
            var command = new SqlCommand(commandText, _connection)
            {
                CommandType = commandType
            };
            return command;
        }

        // Abstract async method to enforce implementation in derived classes
      //  public abstract Task<T> ExecuteAsync(string connectionString);
        public abstract Task<T> ExecuteAsync(string connectionString, CancellationToken cancellationToken = default);

        public void Dispose()
        {
            _connection?.Close();
            _connection?.Dispose();
        }
    }
}

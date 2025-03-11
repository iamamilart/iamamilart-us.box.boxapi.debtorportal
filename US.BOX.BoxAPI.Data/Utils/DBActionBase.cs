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

        public static object GetSafeValue(SqlDataReader reader, string columnName)
        {
            int index = reader.GetOrdinal(columnName);
            Type columnType = reader.GetFieldType(index);

            if (reader.IsDBNull(index))
            {
                return GetDefaultValue(columnType); // Return default value based on type
            }

            // Handle based on column type
            if (columnType == typeof(string))
                return reader.GetString(index);
            if (columnType == typeof(int))
                return reader.GetInt32(index);
            if (columnType == typeof(long))

                return reader.GetInt64(index);
            if (columnType == typeof(bool))
                return reader.GetBoolean(index);
            if (columnType == typeof(DateTime))
                return reader.GetDateTime(index);
            if (columnType == typeof(decimal))
                return reader.GetDecimal(index);
            if (columnType == typeof(double))
                return reader.GetDouble(index);
            if (columnType == typeof(Guid))
                return reader.GetGuid(index);

            // Default case: return object
            return reader.GetValue(index);
        }

        private static object GetDefaultValue(Type type)
        {
            if (type == typeof(string)) return string.Empty;
            if (type == typeof(int)) return 0;
            if (type == typeof(long)) return 0L;
            if (type == typeof(bool)) return false;
            if (type == typeof(DateTime)) return DateTime.MinValue;
            if (type == typeof(decimal)) return 0m;
            if (type == typeof(double)) return 0.0;
            if (type == typeof(Guid)) return Guid.Empty;

            return Activator.CreateInstance(type); // Default for other reference types
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

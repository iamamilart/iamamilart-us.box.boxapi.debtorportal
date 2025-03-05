using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using US.BOX.BoxAPI.Data.Utils;

namespace US.BOX.BoxAPI.Data.Actions
{
    public class CheckPortalUserAction(string username, string password) : DBActionBase<int>
    {
        public override async Task<int> ExecuteAsync(string connectionString , CancellationToken cancellationToken = default)  // Task<int> is expected here
        {
            const string storedProcedure = "[boxapi].[CheckPortalUser]";

            using (var command = await CreateCommandAsync(storedProcedure, CommandType.StoredProcedure , connectionString, cancellationToken))
            {
                command.Parameters.Add(new SqlParameter("@caseno", SqlDbType.Int)).Value = username;
                command.Parameters.Add(new SqlParameter("@password", SqlDbType.VarChar)).Value = password;


                try
                {
                     var res =  await command.ExecuteScalarAsync(cancellationToken);
                    cancellationToken.ThrowIfCancellationRequested(); // Check cancellation between rows

                    return Convert.ToInt32(res);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error executing stored procedure {storedProcedure} for caseNumber {username}", ex);
                }
            }
        }
    }
}

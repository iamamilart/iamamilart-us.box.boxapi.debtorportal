using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using US.BOX.BoxAPI.Data.Utils;

namespace US.BOX.BoxAPI.Data.Actions
{
    public class GetDebtorPortalSettingAction(int caseNumber) : DBActionBase<Dictionary<string, string>>
    {
        public override async Task<Dictionary<string, string>> ExecuteAsync(string connectionString, CancellationToken cancellationToken = default)
        {
            const string storedProcedure = "[boxapi].[GetDebtorPortalSettings]";
            var result = new Dictionary<string, string>();

            try
            {
                using var command = await CreateCommandAsync(storedProcedure, CommandType.StoredProcedure, connectionString, cancellationToken);
                command.Parameters.Add(new SqlParameter("@caseNumber", SqlDbType.Int)).Value = caseNumber;

                using var dataReader = await command.ExecuteReaderAsync(cancellationToken);
                while (await dataReader.ReadAsync(cancellationToken))
                {
                    cancellationToken.ThrowIfCancellationRequested(); // Check cancellation between rows

                    var key = dataReader["key"].ToString();
                    var value = dataReader["value"].ToString();
                    result[key] = value;
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error executing stored procedure {storedProcedure} for caseNumber {caseNumber}", ex);
            }
        }
    }
}

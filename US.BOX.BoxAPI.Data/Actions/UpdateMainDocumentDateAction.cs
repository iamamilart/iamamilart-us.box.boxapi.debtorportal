using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using US.BOX.BoxAPI.Data.Models;
using US.BOX.BoxAPI.Data.Utils;
using US.BOX.DataAccess.DBFramework;

namespace US.BOX.BoxAPI.Data.Actions
{
    public class UpdateMainDocumentDateAction(int entityNo, string activityName, string workflowType) :DBActionBase<bool>
    {
        public override async Task<bool> ExecuteAsync(string connectionString, CancellationToken cancellationToken = default)  // Task<int> is expected here
        {
            const string storedProcedure = "[dpapi].[UpdateMainDocumentDate]";

            using (var command = await CreateCommandAsync(storedProcedure, CommandType.StoredProcedure, connectionString, cancellationToken))
            {
                command.Parameters.Add(DataAcessUtils.CreateParam("@EntityNo", DbType.Int32, entityNo));
                command.Parameters.Add(DataAcessUtils.CreateParam("@ActivityName", DbType.String, activityName));
                command.Parameters.Add(DataAcessUtils.CreateParam("@WorkflowType", DbType.String, workflowType));

                try
                {
                    var res = await command.ExecuteScalarAsync(cancellationToken);
                    cancellationToken.ThrowIfCancellationRequested(); // Check cancellation between rows

                    return Convert.ToBoolean(res);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error executing stored procedure {storedProcedure} for caseNumber {entityNo}", ex);
                }
            }
        }
    }
}

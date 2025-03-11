using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using US.BOX.BoxAPI.Data.Models;
using US.BOX.BoxAPI.Data.Utils;
using US.BOX.DataAccess.DBFramework;

namespace US.BOX.BoxAPI.Data.Actions
{
    public class PrivacyPolicyUpdateAction(int caseNo, bool state) : DBActionBase<bool>
    {
        public override async Task<bool> ExecuteAsync(string connectionString, CancellationToken cancellationToken = default)  // Task<int> is expected here
        {
            const string storedProcedure = "[dpapi].[UpdateprivacyStatus]";

            using (var command = await CreateCommandAsync(storedProcedure, CommandType.StoredProcedure, connectionString, cancellationToken))
            {
               // command.Parameters.Add(DataAcessUtils.CreateParam("@Caseno", DbType.Int32, caseNo));
                //command.Parameters.Add(DataAcessUtils.CreateParam("@ReadStatus", DbType.Boolean, state));
              
                try
                {
                    var res = await command.ExecuteNonQueryAsync(cancellationToken);
                    cancellationToken.ThrowIfCancellationRequested(); // Check cancellation between rows

                    return Convert.ToBoolean(res);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error executing stored procedure {storedProcedure} for caseNumber {caseNo}", ex);
                }
            }
        }
    }
}

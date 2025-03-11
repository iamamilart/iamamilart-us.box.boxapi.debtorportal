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
    public  class AddDPCardPaymentAction(int caseno, string kid, string paymentRef, decimal amount) : DBActionBase<bool>
    {
        public override async Task<bool> ExecuteAsync(string connectionString, CancellationToken cancellationToken = default)  // Task<int> is expected here
        {
            const string storedProcedure = "[dpapi].[AddDPCardPayment]";

            using (var command = await CreateCommandAsync(storedProcedure, CommandType.StoredProcedure, connectionString, cancellationToken))
            {
                command.Parameters.Add(DataAcessUtils.CreateParam("@CaseId", DbType.Int32, caseno));
                command.Parameters.Add(DataAcessUtils.CreateParam("@Amount", DbType.Decimal, amount));
                command.Parameters.Add(DataAcessUtils.CreateParam("@TransID", DbType.String, paymentRef));
                command.Parameters.Add(DataAcessUtils.CreateParam("@KID", DbType.String, kid));

                try
                {
                    var res = await command.ExecuteScalarAsync(cancellationToken);
                    cancellationToken.ThrowIfCancellationRequested(); // Check cancellation between rows

                    return Convert.ToBoolean(res);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error executing stored procedure {storedProcedure} for caseNumber {caseno}", ex);
                }
            }
        }
    }
}

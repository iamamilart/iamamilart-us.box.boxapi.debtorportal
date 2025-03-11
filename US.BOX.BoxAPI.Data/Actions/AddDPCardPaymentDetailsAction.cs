using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using US.BOX.BoxAPI.Data.Models;
using US.BOX.BoxAPI.Data.Utils;
using US.BOX.DataAccess.DBFramework;

namespace US.BOX.BoxAPI.Data.Actions
{
    public class AddDPCardPaymentDetailsAction(int status, ProcessResponse obj): DBActionBase<bool>
    {
        public override async Task<bool> ExecuteAsync(string connectionString, CancellationToken cancellationToken = default)  // Task<int> is expected here
        {
            const string storedProcedure = "[dpapi].[AddDPCardPaymentDetail]";

            using (var command = await CreateCommandAsync(storedProcedure, CommandType.StoredProcedure, connectionString, cancellationToken))
            {
                command.Parameters.Add(DataAcessUtils.CreateParam("@Status", DbType.Int32, status));
                command.Parameters.Add(DataAcessUtils.CreateParam("@Executiontime", DbType.DateTime, obj.ExecutionTime));
                command.Parameters.Add(DataAcessUtils.CreateParam("@TransID", DbType.String, obj.TransactionId));
                command.Parameters.Add(DataAcessUtils.CreateParam("@AuthorizeCode", DbType.String, obj.AuthorizationId));
                command.Parameters.Add(DataAcessUtils.CreateParam("@Merchant", DbType.String, obj.MerchantId));
                command.Parameters.Add(DataAcessUtils.CreateParam("@Opration", DbType.String, obj.Operation));
                command.Parameters.Add(DataAcessUtils.CreateParam("@ResponseCode", DbType.String, obj.ResponseCode));
                command.Parameters.Add(DataAcessUtils.CreateParam("@Batchnumber", DbType.String, obj.BatchNumber));
                command.Parameters.Add(DataAcessUtils.CreateParam("@DetailResponce", DbType.String, obj.DetailResponce));

                try
                {
                    var res = await command.ExecuteScalarAsync(cancellationToken);
                    cancellationToken.ThrowIfCancellationRequested(); 

                    return Convert.ToBoolean(res);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error executing stored procedure {storedProcedure} for obj.TransactionId {obj.TransactionId}", ex);
                }
            }
        }
    }
}

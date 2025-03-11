
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using US.BOX.BoxAPI.Data.Models;
using US.BOX.BoxAPI.Data.Utils;
using US.BOX.DataAccess.DBFramework;
using Transaction = US.BOX.BoxAPI.Data.Models.Transaction;

namespace US.BOX.BoxAPI.Data.Actions
{
    public class GetCardPaymentDataAction(string transID) : DBActionBase<Transaction>
    {
        public override async Task<Transaction> ExecuteAsync(string connectionString, CancellationToken cancellationToken = default)  // Task<int> is expected here
        {
            Transaction importcardPaymnet = null;
            const string storedProcedure = "dpapi.GetCardPaymentForImport";

            using var command = await CreateCommandAsync(storedProcedure, CommandType.StoredProcedure, connectionString, cancellationToken);
            command.Parameters.Add(DataAcessUtils.CreateParam("@transID", DbType.String, transID));

            using var dataReader = await command.ExecuteReaderAsync(cancellationToken);
            while (await dataReader.ReadAsync(cancellationToken))
            {
                cancellationToken.ThrowIfCancellationRequested();

                importcardPaymnet = new Transaction();
                importcardPaymnet.PID = (string)GetSafeValue(dataReader,"PID");
                importcardPaymnet.CID = (string)GetSafeValue(dataReader,"CID");
                importcardPaymnet.TType = (string)GetSafeValue(dataReader,"TType");
                importcardPaymnet.TDate = (DateTime)GetSafeValue(dataReader,"TDate");
                importcardPaymnet.TAmount = (Double)GetSafeValue(dataReader,"TAmount");
                importcardPaymnet.TInfo = (string)GetSafeValue(dataReader,"TInfo");
                importcardPaymnet.TKid = (string)GetSafeValue(dataReader,"TKid");
                importcardPaymnet.TStatus = (string)GetSafeValue(dataReader,"TStatus");
                importcardPaymnet.TNumber = (string)GetSafeValue(dataReader,"TNumber");
            }
            return importcardPaymnet;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using US.BOX.BoxAPI.Data.Models;
using US.BOX.BoxAPI.Data.Utils;

namespace US.BOX.BoxAPI.Data.Actions
{
    internal class GetPartPaymentAction(int caseNumber) : DBActionBase<List<PartPayment>>
    {
        public override async Task<List<PartPayment>> ExecuteAsync(string connectionString, CancellationToken cancellationToken = default)
        {
            List<PartPayment> list = new List<PartPayment>();
            string storedProcedure = "dbo.USP_GetExistingCasePartPayment";
            //TODO - Need to create boxapi SP
            try
            {
                using var command = await CreateCommandAsync(storedProcedure, CommandType.StoredProcedure, connectionString,cancellationToken);
                command.Parameters.Add(new SqlParameter("@caseNumber", SqlDbType.Int)).Value = caseNumber;

                using var dataReader = await command.ExecuteReaderAsync(cancellationToken);
                while (await dataReader.ReadAsync(cancellationToken))
                {
                    cancellationToken.ThrowIfCancellationRequested(); // Check cancellation between rows

                    PartPayment partPaymentSchema = new PartPayment();
                    partPaymentSchema.CapitalAmount = Convert.ToInt32(dataReader["Amount"]);
                    partPaymentSchema.Date = Convert.ToDateTime(dataReader["DueDate"]);
                    partPaymentSchema.CapitalAmount = Convert.ToDecimal(dataReader["Amount"]);
                    partPaymentSchema.InstallmentFee = Convert.ToDecimal(dataReader["InstalmentFee"]);
                    partPaymentSchema.InstallmentAmount = Convert.ToDecimal(dataReader["InstalmentAmount"]);
                    partPaymentSchema.Outstanding = Convert.ToDecimal(dataReader["Balance"]);
                    partPaymentSchema.Interest = Convert.ToDecimal(dataReader["InterestAmount"]);
                    if (dataReader["PaymentID"] != DBNull.Value)
                    {
                        partPaymentSchema.Paid = true;
                    }

                    list.Add(partPaymentSchema);
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error executing stored procedure {storedProcedure} for caseNumber {caseNumber}", ex);
            }
        }
    }
}
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
    public class RegisterClaimOfPaymentAction(DataClaimOfPayment _claimOfPayment) : DBActionBase<int>
    {
        public override async Task<int> ExecuteAsync(string connectionString, CancellationToken cancellationToken = default)  // Task<int> is expected here
        {
            int noteId = 0;
            const string storedProcedureName = "[dpapi].[USP_AddClaimOfPayment]";
            using (var command = await CreateCommandAsync(storedProcedureName, CommandType.StoredProcedure, connectionString, cancellationToken))
            {
                command.Parameters.Add(new SqlParameter("@CaseNo", _claimOfPayment.CaseId));
                command.Parameters.Add(new SqlParameter("@Paidamount", _claimOfPayment.PaidAmount));
                command.Parameters.Add(new SqlParameter("@Paiddate", _claimOfPayment.PaidDate));
                command.Parameters.Add(new SqlParameter("@Fromaccount", _claimOfPayment.FromAccount));
                command.Parameters.Add(new SqlParameter("@Toaccount", _claimOfPayment.ToAccount));
                command.Parameters.Add(new SqlParameter("@Kid", _claimOfPayment.Kid));
                command.Parameters.Add(new SqlParameter("@Casenote", _claimOfPayment.CaseNote));
                command.Parameters.Add(new SqlParameter("@NoteCommunicationType", _claimOfPayment.CommunicationType));
                command.Parameters.Add(new SqlParameter("@Cardnumber", _claimOfPayment.CardNumber));
                command.Parameters.Add(new SqlParameter("@PayerName", _claimOfPayment.PayerName));
                command.Parameters.Add(new SqlParameter("@PaymentMethod", _claimOfPayment.PaymentMethod));
                command.Parameters.Add(new SqlParameter("@SMSnumber", _claimOfPayment.SmsNumber));
                command.Parameters.Add(new SqlParameter("@Email", _claimOfPayment.Email));
                command.Parameters.Add(new SqlParameter("@Isprincipal", _claimOfPayment.IsPrincipal));
                command.Parameters.Add(new SqlParameter("@Invoices", getInvoiceDataTable(_claimOfPayment)));

                using var dataReader = await command.ExecuteReaderAsync(cancellationToken);
                while (await dataReader.ReadAsync(cancellationToken))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (DBNull.Value != dataReader["Noteid"])
                    {
                        noteId = Convert.ToInt32(dataReader["Noteid"] ?? 0);
                    }
                }

            }
            return noteId;
        }

        private DataTable getInvoiceDataTable(DataClaimOfPayment cop)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("InvoiceNo");
            dt.Columns.Add("KID");
            dt.Columns.Add("Amount");
            dt.Columns.Add("PaidAmount");
            dt.Columns.Add("Balance");

            cop.InvoiceList.ForEach(x =>
            {
                dt.Rows.Add(x.InvoiceNo, x.Kid, x.Amount, x.PaidAmount, x.Balance);
            });
            return dt;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using US.BOX.BoxAPI.Data.Models;
using US.BOX.BoxAPI.Data.Utils;

namespace US.BOX.BoxAPI.Data.Actions
{
    public class RegisterDebtorObjectionAction(DebtorObjection _debtorObjection) : DBActionBase<int>
    {
        public override async Task<int> ExecuteAsync(string connectionString, CancellationToken cancellationToken = default)  // Task<int> is expected here
        {
            int noteId = 0;
            const string storedProcedureName = "[dpapi].[USP_AddDebtorObjection]";
            using (var command = await CreateCommandAsync(storedProcedureName, CommandType.StoredProcedure, connectionString, cancellationToken))
            {
                command.Parameters.Add(new SqlParameter("@Caseno", _debtorObjection.CaseId));
                command.Parameters.Add(new SqlParameter("@Notetext", _debtorObjection.CaseNote));
                command.Parameters.Add(new SqlParameter("@communicationType", _debtorObjection.CommunicationType));
                command.Parameters.Add(new SqlParameter("@ObjectionReasonId", _debtorObjection.ObjectionReasonId));
                command.Parameters.Add(new SqlParameter("@SMSnumber", _debtorObjection.SmsNumber));
                command.Parameters.Add(new SqlParameter("@Email", _debtorObjection.Email));
                command.Parameters.Add(new SqlParameter("@Invoices", getInvoiceDataTable(_debtorObjection)));

                using var dataReader = await command.ExecuteReaderAsync(cancellationToken);
                while (await dataReader.ReadAsync(cancellationToken))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (DBNull.Value != dataReader["NoteId"])
                    {
                        noteId = Convert.ToInt32(dataReader["NoteId"] ?? 0);
                    }
                }

            }
            return noteId;
        }

        private DataTable getInvoiceDataTable(DebtorObjection debtor)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("InvoiceNo");
            dt.Columns.Add("KID");
            dt.Columns.Add("Amount");
            dt.Columns.Add("PaidAmount");
            dt.Columns.Add("Balance");

            debtor.Invoices.ForEach(x =>
            {
                dt.Rows.Add(x.InvoiceNo, x.Kid, x.Amount, x.PaidAmount, x.Balance);
            });
            return dt;
        }
    }
}
    

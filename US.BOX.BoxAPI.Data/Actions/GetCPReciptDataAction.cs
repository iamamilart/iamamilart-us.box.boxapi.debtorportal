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
    public class GetCPReciptDataAction(string caseNo, string KID) : DBActionBase<CardPaymentReceipt>
    {
        public override async Task<CardPaymentReceipt> ExecuteAsync(string connectionString, CancellationToken cancellationToken = default)  // Task<int> is expected here
        {
            CardPaymentReceipt ReciptData = new CardPaymentReceipt();
            const string storedProcedure = "dpapi.DP_GetCardPaymentReceipt";

            using (var command = await CreateCommandAsync(storedProcedure, CommandType.StoredProcedure, connectionString, cancellationToken))
            {
                command.Parameters.Add(DataAcessUtils.CreateParam("@KID", DbType.String, KID));
                command.Parameters.Add(DataAcessUtils.CreateParam("@caseNumber", DbType.Int32, caseNo));

                ReciptData.ReciptPaymentDetail = new List<ReciptPaymentDetail>();
                List<ReciptPaymentDetail> details = new List<ReciptPaymentDetail>();

                try
                {
                    using var dataReader = await command.ExecuteReaderAsync(cancellationToken);
                    while (await dataReader.ReadAsync(cancellationToken))
                    {
                        cancellationToken.ThrowIfCancellationRequested(); // Check cancellation between rows

                        ReciptData.PaymentCheck = (string)GetSafeValue(dataReader,"PaymentRef");

                        DebtorCase caseobj = new DebtorCase();
                        if (ReciptData.PaymentCheck == "")
                        {
                            ReciptPaymentDetail pt1 = new ReciptPaymentDetail();
                            pt1.ItemText = (string)GetSafeValue(dataReader,"ItemText");
                            pt1.ItemAmount = (Decimal)GetSafeValue(dataReader,"ItemAmount");
                            details.Add(pt1);
                        }

                        else
                        {
                            ReciptData.CreditorName = (string)GetSafeValue(dataReader,"credior");
                            ReciptData.InvoiceDate = (string)GetSafeValue(dataReader,"InvoiceDate");
                            ReciptData.DebtorName = (string)GetSafeValue(dataReader,"debtor");
                            ReciptData.PaymentRef = (string)GetSafeValue(dataReader,"PaymentRef");
                            ReciptData.PaidAmount = (Decimal)GetSafeValue(dataReader,"Amount");
                            ReciptData.PaymentDate = (string)GetSafeValue(dataReader,"RegDate");
                            ReciptData.CaseNumber = (string)GetSafeValue(dataReader,"ExCaseNo");
                        }
                    }
                    ReciptData.ReciptPaymentDetail = details;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error executing stored procedure {storedProcedure} for caseNumber {caseNo}", ex);
                }
            }
            return ReciptData;
        }
    }
}

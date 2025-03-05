
using System.Data.SqlClient;
using System.Data;
using System.Text;
using US.BOX.BoxAPI.Data.Models;
using US.BOX.BoxAPI.Data.Utils;

namespace US.BOX.BoxAPI.Data.Actions
{
    public class GetPortalCaseAction(int caseNumber) : DBActionBase<PortalCase>
    {

        public override async Task<PortalCase> ExecuteAsync(string connectionString, CancellationToken cancellationToken = default)
        {
            PortalCase pcase = null;
            string storedProcedure = "[boxapi].[GetPortalCase]";

            Dictionary<string, string> returnList = new Dictionary<string, string>();

            using (var command = await CreateCommandAsync(storedProcedure, CommandType.StoredProcedure, connectionString,cancellationToken))
            {
                command.Parameters.Add(new SqlParameter("@caseno", SqlDbType.Int)).Value = caseNumber;


                try
                {
                    using var dataReader = await command.ExecuteReaderAsync(cancellationToken);
                    pcase = new PortalCase();
                    pcase.InvoiceInfo = new List<PortalInvoice>();
                    pcase.History = new List<Activity>();
                    pcase.Status = -1;
                    while (await dataReader.ReadAsync(cancellationToken))
                    {
                        cancellationToken.ThrowIfCancellationRequested(); // Check cancellation between rows
                        pcase.ArNo = Convert.ToInt32(dataReader["arNo"]);
                        pcase.Balance = Convert.ToDecimal(dataReader["balance"]);
                        pcase.Balance2 = Convert.ToDecimal(dataReader["week2"]);
                        pcase.Balance4 = Convert.ToDecimal(dataReader["week4"]);
                        pcase.Balance6 = Convert.ToDecimal(dataReader["week6"]);
                        pcase.Paid = Convert.ToDecimal(dataReader["paid"]);
                        pcase.Cost = Convert.ToDecimal(dataReader["cost"]);
                        pcase.Interest = Convert.ToDecimal(dataReader["interest"]);
                        pcase.MainAmount = Convert.ToDecimal(dataReader["mainamount"]);
                        pcase.ClientAccount = dataReader.GetStringValue("account");
                        pcase.CaseKID = dataReader.GetString("casekid");
                        pcase.DebtorSSNO = dataReader.GetStringValue("debtorSsno");
                        pcase.DebtorNo = dataReader.GetString("debtorNo");
                        pcase.DebtorName = dataReader["debtorName"].ToString();
                        pcase.DebtorFirstName = dataReader["debtorFirstName"].ToString();
                        pcase.DebtorLastName = dataReader["debtorLastName"].ToString();
                        pcase.DebtorBirthDate = dataReader.GetDateTimeNull("debBorn");
                        pcase.DebType = dataReader["debType"].ToString();
                        pcase.ExternalCaseNumber = Convert.ToInt32(dataReader["caseNo"]);
                        pcase.BOXCaseNumber = Convert.ToInt32(dataReader["boxCaseNo"]);
                        pcase.CaseType = dataReader["caseType"].ToString();
                        pcase.CreditorID = dataReader["creditorId"].ToString();
                        pcase.CreditorName = dataReader["creditorName"].ToString();
                        pcase.CaseCost = Convert.ToDecimal(dataReader["CaseCost"]);
                        pcase.IsPartPaymentBreach = dataReader.GetBooleanValue("HasPartpaymentBreached");

                        pcase.Status = Convert.ToInt32(dataReader["status"]);
                        pcase.MinInstallment = Convert.ToInt32(dataReader["minInstallments"]);
                        pcase.MaxInstallment = Convert.ToInt32(dataReader["maxInstallments"]);
                        pcase.MaxPossibleFee = Convert.ToDouble(dataReader["maxPossibleFee"]);
                        pcase.DelayedTill = dataReader.GetDateTimeNull("DelayedTill");
                        pcase.WorkflowStatus = dataReader["WorkflowStatus"].ToString();
                        pcase.StateWorkflow = dataReader["StateWorkflow"].ToString();
                        pcase.IsPartPayment = dataReader.GetInt32Value("partpayment") > 0 ? true : false;
                        pcase.IsAllowedToDelay = Convert.ToBoolean(dataReader["isAllowedToDelay"]);
                        pcase.IsPartPaymentPossible = Convert.ToBoolean(dataReader["isPartPaymentPossible"]);
                        pcase.ExceededAmount = dataReader.GetDecimalValue("exceededAmount");
                        pcase.HasOpenObjection = Convert.ToBoolean(dataReader["hasOpenObjection"]);
                        pcase.ObjectionDate = dataReader.GetDateTimeNull("objectionDate");
                        pcase.InvoiceNumber = Convert.ToString(dataReader["invoiceNumber"]);
                        pcase.DelayDueDate = dataReader.GetDateTimeNull("delayDueDate");
                        pcase.DelayUpto = dataReader.GetDateTimeNull("delayDueDate");
                        pcase.DelayFee = Convert.ToDecimal(dataReader["delayFee"]);
                        // column added Note:- remove null checking
                        pcase.DebtorObjectionNote = dataReader["ObjectionReason"] != null ? dataReader["ObjectionReason"].ToString() : String.Empty;
                        pcase.LastCreditorPortalNote = dataReader["LastCreditorPortalNote"] != null ? dataReader["LastCreditorPortalNote"].ToString() : String.Empty;
                        pcase.IsPrivacyPolicyRead = Convert.ToBoolean(dataReader["IsprivacyPolicyRead"]);


                    }

                    dataReader.NextResult();

                    while (await dataReader.ReadAsync())
                    {
                        PortalInvoice pinvoice = new PortalInvoice();
                        pinvoice.Amount = Convert.ToDecimal(dataReader["amount"]);
                        pinvoice.InvoiceId = Convert.ToInt32(dataReader["invoiceId"]);
                        pinvoice.InvoiceNumber = dataReader.GetString("invoiceno");
                        pinvoice.Date = dataReader.GetDateTimeNull("invoiceDate");
                        pinvoice.Text = dataReader.GetString("invoiceText");
                        pinvoice.DocumentId = dataReader.GetString("documentId");
                        pinvoice.DueDate = dataReader.GetDateTimeNull("dueDate");

                        pcase.InvoiceInfo.Add(pinvoice);
                    }

                    dataReader.NextResult();

                    while (await dataReader.ReadAsync())
                    {
                        Activity pActivity = new Activity();

                        pActivity.ExecutionId = Convert.ToInt32(dataReader["historyId"]);
                        pActivity.Text = dataReader.GetString("historyText");
                        pActivity.Description = dataReader.GetString("historyDescription");
                        pActivity.ExeutedDate = Convert.ToDateTime(dataReader.GetDateTimeNull("orderDate"));
                        pActivity.DocumentId = dataReader.GetString("documentId");
                        pActivity.DocumentType = dataReader.GetInt32("documentType");
                        pcase.History.Add(pActivity);
                    }

                    if (dataReader.NextResult())
                    {
                        Contact debContact = new Contact();
                        while (await dataReader.ReadAsync())
                        {
                            debContact.Address = dataReader.GetStringValue("Addr1") + " " + dataReader.GetStringValue("Addr2");
                            debContact.Email = dataReader.GetStringValue("Email");
                            debContact.Fax = dataReader.GetStringValue("Fax");
                            debContact.Mobile = dataReader.GetStringValue("TelMobile");
                            debContact.PostalCode = dataReader.GetStringValue("ZipCode");
                            debContact.PostalPlace = dataReader.GetStringValue("ZipName");
                            debContact.TelHome = dataReader.GetStringValue("TelHome");
                            debContact.TelWork = dataReader.GetStringValue("TelWork");


                        }
                        pcase.DebtorContact = debContact;
                    }
                    return pcase;

                }
                catch (Exception ex)
                {
                    throw new Exception($"Error executing stored procedure {storedProcedure} for caseNumber {caseNumber}", ex);
                }


            }
        }
    }
}

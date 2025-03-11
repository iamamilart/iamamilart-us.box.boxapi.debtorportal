
//using System.Data.SqlClient;
using System.Data;
using System.Text;
using US.BOX.BoxAPI.Data.Models;
using US.BOX.BoxAPI.Data.Utils;
using US.BOX.DataAccess.DBFramework;

namespace US.BOX.BoxAPI.Data.Actions
{
    public class GetPortalCaseAction(int caseNumber) : DBActionBase<PortalCase>
    {

        public override async Task<PortalCase> ExecuteAsync(string connectionString, CancellationToken cancellationToken = default)
        {
            PortalCase pcase = null;
            string storedProcedure = "[dpapi].[GetPortalCase]";

            Dictionary<string, string> returnList = new Dictionary<string, string>();

 
            using (var command = await CreateCommandAsync(storedProcedure, CommandType.StoredProcedure, connectionString,cancellationToken))
            {
               // command.Parameters.Add(new SqlParameter("@caseno", SqlDbType.Int)).Value = caseNumber;
                command.Parameters.Add(DataAcessUtils.CreateParam("@caseno", DbType.Int32, caseNumber));

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
                        pcase.ArNo =(Int32) GetSafeValue(dataReader, "arNo");// Convert.ToInt32(dataReader["arNo"]);
                        pcase.Balance = (Decimal)GetSafeValue(dataReader, "balance");// Convert.ToDecimal(dataReader["balance"]);
                        pcase.Balance2 = (Decimal)GetSafeValue(dataReader, "week2");// Convert.ToDecimal(dataReader["week2"]);
                        pcase.Balance4 = (Decimal)GetSafeValue(dataReader, "week4");// Convert.ToDecimal(dataReader["week4"]);
                        pcase.Balance6 = (Decimal)GetSafeValue(dataReader, "week6");// Convert.ToDecimal(dataReader["week6"]);
                        pcase.Paid = (Decimal)GetSafeValue(dataReader, "paid");// Convert.ToDecimal(dataReader["paid"]);
                        pcase.Cost = (Decimal)GetSafeValue(dataReader, "cost");// Convert.ToDecimal(dataReader["cost"]);
                        pcase.Interest = (Decimal)GetSafeValue(dataReader, "interest");// Convert.ToDecimal(dataReader["interest"]);
                        pcase.MainAmount = (Decimal)GetSafeValue(dataReader, "mainamount");// Convert.ToDecimal(dataReader["mainamount"]);
                        pcase.ClientAccount = (string)GetSafeValue(dataReader, "account");// dataReader.GetStringValue("account");
                        pcase.CaseKID = (string)GetSafeValue(dataReader, "casekid");// dataReader.GetStringValue("casekid");
                        pcase.DebtorSSNO = (string)GetSafeValue(dataReader, "debtorSsno");// dataReader.GetStringValue("debtorSsno");
                        pcase.DebtorNo = (string)GetSafeValue(dataReader, "debtorNo");// dataReader.GetString("debtorNo");
                        pcase.DebtorName = (string)GetSafeValue(dataReader, "debtorName");// dataReader["debtorName"].ToString();
                        pcase.DebtorFirstName = (string)GetSafeValue(dataReader, "debtorFirstName");// dataReader["debtorFirstName"].ToString();
                        pcase.DebtorLastName = (string)GetSafeValue(dataReader, "debtorLastName");// dataReader["debtorLastName"].ToString();
                        pcase.DebtorBirthDate = (DateTime)GetSafeValue(dataReader, "debBorn");// dataReader.GetDateTimeNull("debBorn");
                        pcase.DebType = (string)GetSafeValue(dataReader, "debType");// dataReader["debType"].ToString();
                        pcase.ExternalCaseNumber = (Int32)GetSafeValue(dataReader, "caseNo");// Convert.ToInt32(dataReader["caseNo"]);
                        pcase.BOXCaseNumber = (Int32)GetSafeValue(dataReader, "boxCaseNo");// Convert.ToInt32(dataReader["boxCaseNo"]);
                        pcase.CaseType = (string)GetSafeValue(dataReader, "caseType");// dataReader["caseType"].ToString();
                        pcase.CreditorID = (string)GetSafeValue(dataReader, "creditorId");// dataReader["creditorId"].ToString();
                        pcase.CreditorName = (String)GetSafeValue(dataReader, "creditorName");// dataReader["creditorName"].ToString();
                        pcase.CaseCost = (Decimal)GetSafeValue(dataReader, "CaseCost");// Convert.ToDecimal(dataReader["CaseCost"]);
                        pcase.IsPartPaymentBreach = (Boolean)GetSafeValue(dataReader, "HasPartpaymentBreached");// dataReader.GetBooleanValue("HasPartpaymentBreached");

                        pcase.Status = (Int32)GetSafeValue(dataReader, "status");// Convert.ToInt32(dataReader["status"]);
                        pcase.MinInstallment = (Int32)GetSafeValue(dataReader, "minInstallments");// Convert.ToInt32(dataReader["minInstallments"]);
                        pcase.MaxInstallment = (Int32)GetSafeValue(dataReader, "maxInstallments");// Convert.ToInt32(dataReader["maxInstallments"]);
                        pcase.MaxPossibleFee = (Double)GetSafeValue(dataReader, "maxPossibleFee");// Convert.ToDouble(dataReader["maxPossibleFee"]);
                        pcase.DelayedTill = (DateTime)GetSafeValue(dataReader, "DelayedTill");// dataReader.GetDateTimeNull("DelayedTill");
                        pcase.WorkflowStatus = (string)GetSafeValue(dataReader, "WorkflowStatus");// dataReader["WorkflowStatus"].ToString();
                        pcase.StateWorkflow = (string)GetSafeValue(dataReader, "StateWorkflow");// dataReader["StateWorkflow"].ToString();
                        pcase.IsPartPayment = (Int32)GetSafeValue(dataReader, "partpayment") > 0 ? true : false;// dataReader.GetInt32Value("partpayment") > 0 ? true : false;
                        pcase.IsAllowedToDelay = (Boolean)GetSafeValue(dataReader, "isAllowedToDelay");//Convert.ToBoolean(dataReader["isAllowedToDelay"]);
                        pcase.IsPartPaymentPossible = (Boolean)GetSafeValue(dataReader, "isPartPaymentPossible");// Convert.ToBoolean(dataReader["isPartPaymentPossible"]);
                        pcase.ExceededAmount = (Decimal)GetSafeValue(dataReader, "exceededAmount");// dataReader.GetDecimalValue("exceededAmount");
                        pcase.HasOpenObjection = (Boolean)GetSafeValue(dataReader, "hasOpenObjection");// Convert.ToBoolean(dataReader["hasOpenObjection"]);
                        pcase.ObjectionDate = (DateTime)GetSafeValue(dataReader, "objectionDate");// dataReader.GetDateTimeNull("objectionDate");
                        pcase.InvoiceNumber = (string)GetSafeValue(dataReader, "invoiceNumber");//Convert.ToString(dataReader["invoiceNumber"]);
                        pcase.DelayDueDate = (DateTime)GetSafeValue(dataReader, "delayDueDate");// dataReader.GetDateTimeNull("delayDueDate");
                        pcase.DelayUpto = (DateTime)GetSafeValue(dataReader, "delayDueDate");// dataReader.GetDateTimeNull("delayDueDate");
                        pcase.DelayFee = (Decimal)GetSafeValue(dataReader, "delayFee");//Convert.ToDecimal(dataReader["delayFee"]);
                        // column added Note:- remove null checking
                        pcase.DebtorObjectionNote = (string)GetSafeValue(dataReader, "ObjectionReason");
                                                                                                 //dataReader["ObjectionReason"] != null ? dataReader["ObjectionReason"].ToString() : String.Empty;
                        pcase.LastCreditorPortalNote = (string)GetSafeValue(dataReader, "LastCreditorPortalNote");
                                                                                                    //dataReader["LastCreditorPortalNote"] != null ? dataReader["LastCreditorPortalNote"].ToString() : String.Empty;
                        pcase.IsPrivacyPolicyRead = (Boolean)GetSafeValue(dataReader, "IsprivacyPolicyRead");// Convert.ToBoolean(dataReader["IsprivacyPolicyRead"]);


                    }

                    dataReader.NextResult();

                    while (await dataReader.ReadAsync())
                    {
                        PortalInvoice pinvoice = new PortalInvoice();
                        pinvoice.Amount = (Decimal)GetSafeValue(dataReader, "amount");// Convert.ToDecimal(dataReader["amount"]);
                        pinvoice.InvoiceId = (Int32)GetSafeValue(dataReader, "invoiceId");// Convert.ToInt32(dataReader["invoiceId"]);
                        pinvoice.InvoiceNumber = (string)GetSafeValue(dataReader, "invoiceno");// dataReader.GetString("invoiceno");
                        pinvoice.Date = (DateTime)GetSafeValue(dataReader, "invoiceDate");// dataReader.GetDateTimeNull("invoiceDate");
                        pinvoice.Text = (string)GetSafeValue(dataReader, "invoiceText");// dataReader.GetString("invoiceText");
                        pinvoice.DocumentId = (string)GetSafeValue(dataReader, "documentId");// dataReader.GetString("documentId");
                        pinvoice.DueDate = (DateTime)GetSafeValue(dataReader, "dueDate");// dataReader.GetDateTimeNull("dueDate");

                        pcase.InvoiceInfo.Add(pinvoice);
                    }

                    dataReader.NextResult();

                    while (await dataReader.ReadAsync())
                    {
                        Activity pActivity = new Activity();

                        pActivity.ExecutionId = (Int32)GetSafeValue(dataReader, "historyId");// Convert.ToInt32(dataReader["historyId"]);
                        pActivity.Text = (string)GetSafeValue(dataReader, "historyText");// dataReader.GetString("historyText");
                        pActivity.Description = (string)GetSafeValue(dataReader, "historyDescription");// dataReader.GetString("historyDescription");
                        pActivity.ExeutedDate = (DateTime)GetSafeValue(dataReader, "orderDate");// Convert.ToDateTime(dataReader.GetDateTimeNull("orderDate"));
                        pActivity.DocumentId = (string)GetSafeValue(dataReader, "documentId");// dataReader.GetString("documentId");
                        pActivity.DocumentType = (Int32)GetSafeValue(dataReader, "documentType");// dataReader.GetInt32("documentType");
                        pcase.History.Add(pActivity);
                    }

                    if (dataReader.NextResult())
                    {
                        Contact debContact = new Contact();
                        while (await dataReader.ReadAsync())
                        {
                            debContact.Address = (string)GetSafeValue(dataReader, "Addr1");// dataReader.GetStringValue("Addr1") + " " + dataReader.GetStringValue("Addr2");
                            debContact.Email = (string)GetSafeValue(dataReader, "Email");// dataReader.GetStringValue("Email");
                            debContact.Fax = (string)GetSafeValue(dataReader, "Fax");// dataReader.GetStringValue("Fax");
                            debContact.Mobile = (string)GetSafeValue(dataReader, "TelMobile");// dataReader.GetStringValue("TelMobile");
                            debContact.PostalCode = (string)GetSafeValue(dataReader, "ZipCode");// dataReader.GetStringValue("ZipCode");
                            debContact.PostalPlace = (string)GetSafeValue(dataReader, "ZipName");// dataReader.GetStringValue("ZipName");
                            debContact.TelHome = (string)GetSafeValue(dataReader, "TelHome");// dataReader.GetStringValue("TelHome");
                            debContact.TelWork = (string)GetSafeValue(dataReader, "TelWork");//dataReader.GetStringValue("TelWork");


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

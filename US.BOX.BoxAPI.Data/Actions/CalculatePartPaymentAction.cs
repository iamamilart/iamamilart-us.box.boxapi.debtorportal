using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using US.BOX.BoxAPI.Data.Models;
using US.BOX.BoxAPI.Data.Utils;
using US.BOX.DataAccess.DBFramework;

namespace US.BOX.BoxAPI.Data.Actions
{
    public class CalculatePartPaymentAction(DataPartPaymentRequest _partPayment) : DBActionBase<CalculatePartPayment>
    {
        public override async Task<CalculatePartPayment> ExecuteAsync(string connectionString, CancellationToken cancellationToken = default)  // Task<int> is expected here
        {
            CalculatePartPayment calculatePartPayment = new CalculatePartPayment();
            List<PartPaymentSchema> partPaymentSchema = new List<PartPaymentSchema>();
            decimal forwardCharge = 0;
            decimal caseCost = 0;

            const string storedProcedure = "[dpapi].[CalculatePartPayment]";

            using (var command = await CreateCommandAsync(storedProcedure, CommandType.StoredProcedure, connectionString, cancellationToken))
            {
              
                command.Parameters.Add(DataAcessUtils.CreateParam("@caseno", DbType.String, _partPayment.CaseId));
                command.Parameters.Add(DataAcessUtils.CreateParam("@CaseNote", DbType.String,
                                                              _partPayment.CaseNote != null
                                                                  ? _partPayment.CaseNote.Trim()
                                                                  : string.Empty));
                command.Parameters.Add(DataAcessUtils.CreateParam("@Termamount", DbType.Decimal,
                                                              _partPayment.InstallmentAmount));
                command.Parameters.Add(DataAcessUtils.CreateParam("@Duedate", DbType.Date,
                                                              _partPayment.PartPaymentDate));
                command.Parameters.Add(DataAcessUtils.CreateParam("@FirstInstallmentDate", DbType.Date,
                                                              _partPayment.FirstInstallmentDate));
                command.Parameters.Add(DataAcessUtils.CreateParam("@Noofinstallments", DbType.Int32,
                                                              _partPayment.NumberOfInstallment));
                command.Parameters.Add(DataAcessUtils.CreateParam("@Increment", DbType.Int32, _partPayment.IncrementTimePeriod));
                command.Parameters.Add(DataAcessUtils.CreateParam("@Timepersepective", DbType.Int32,
                                                              _partPayment.SelectedPeriod));
                command.Parameters.Add(DataAcessUtils.CreateParam("@FowardCharge", DbType.Double,
                                                              _partPayment.ForwardCharge));
                command.Parameters.Add(DataAcessUtils.CreateParam("@Agreedfinalsettlement", DbType.Boolean,
                                                              _partPayment.HasPaymentAgreement));
                command.Parameters.Add(DataAcessUtils.CreateParam("@Casebalance", DbType.Decimal,
                                                              _partPayment.RemainingBalance));
                command.Parameters.Add(DataAcessUtils.CreateParam("@Addcasecost", DbType.Boolean,
                                                              _partPayment.IsCaseCostAdded));
                command.Parameters.Add(DataAcessUtils.CreateParam("@ComProfile", DbType.Int32,
                                                              _partPayment.CommunicationProfile));
                command.Parameters.Add(DataAcessUtils.CreateParam("@Writedb", DbType.Int32, _partPayment.WriteToDB));
                command.Parameters.Add(DataAcessUtils.CreateParam("@Smsno", DbType.String, _partPayment.SmsNumber));
                command.Parameters.Add(DataAcessUtils.CreateParam("@email", DbType.String, _partPayment.Email));

                try
                {
                    if (_partPayment.TermAmountNoOfInstallments > 0)
                    {
                        command.Parameters.Add(DataAcessUtils.CreateParam("@Termamountnoofinstallments", DbType.Int32,
                                                                      _partPayment.TermAmountNoOfInstallments));
                    }
                    if (_partPayment.WriteToDB == 1)
                    {
                        await command.ExecuteNonQueryAsync();
                    }
                    else
                    {
                        using var dataReader = await command.ExecuteReaderAsync(cancellationToken);
                        while (await dataReader.ReadAsync(cancellationToken))
                        {
                            cancellationToken.ThrowIfCancellationRequested(); // Check cancellation between rows

                            //var key = dataReader["key"].ToString();
                            //var value = dataReader["value"].ToString();
                            //result[key] = value;
                        
                            PartPaymentSchema schema = new PartPaymentSchema();

                            if (!Convert.ToBoolean(dataReader["IsErroroccured"]))
                            {
                                if (dataReader["paymentno"] != DBNull.Value)
                                {
                                    schema.PartPaymentNo = Convert.ToInt32(dataReader["paymentno"]);
                                }
                                if (dataReader["duedate"] != DBNull.Value)
                                {
                                    schema.DueDate = Convert.ToDateTime(dataReader["duedate"]);
                                }
                                if (dataReader["partpayment"] != DBNull.Value)
                                {
                                    schema.PartPayment = Convert.ToDecimal(dataReader["partpayment"]);
                                }
                                if (dataReader["termamount"] != DBNull.Value)
                                {
                                    schema.InstallmentAmount = Convert.ToDecimal(dataReader["termamount"]);
                                }
                                if (dataReader["termfee"] != DBNull.Value)
                                {
                                    schema.InstallmentFee = Convert.ToDecimal(dataReader["termfee"]);
                                }
                                if (dataReader["remainingbalance"] != DBNull.Value)
                                {
                                    schema.RemainingBalance = Convert.ToDecimal(dataReader["remainingbalance"]);
                                }
                                if (dataReader["interestpart"] != DBNull.Value)
                                {
                                    schema.Interest = Convert.ToDecimal(dataReader["interestpart"]);
                                }
                                if (dataReader["ForwardChargePortion"] != DBNull.Value)
                                {
                                    forwardCharge = Convert.ToDecimal(dataReader["ForwardChargePortion"]);
                                }
                                if (dataReader["CaseCost"] != DBNull.Value)
                                {
                                    caseCost = Convert.ToDecimal(dataReader["CaseCost"]);
                                }
                                if (dataReader["IsDCFAdded"] != DBNull.Value)
                                {
                                    calculatePartPayment.IsDCFAdded = Convert.ToBoolean(dataReader["IsDCFAdded"]);
                                }
                            }
                            else
                            {
                                if (dataReader["ErrorNumber"] != DBNull.Value)
                                {
                                    calculatePartPayment.ErrorCode = Convert.ToInt32(dataReader["ErrorNumber"]);
                                }
                                if (dataReader["ErrorMessage"] != DBNull.Value)
                                {
                                    calculatePartPayment.ErrorMessage = dataReader["ErrorMessage"].ToString();
                                }
                                if (dataReader["IsErroroccured"] != DBNull.Value)
                                {
                                    calculatePartPayment.IsErrorOccured = Convert.ToBoolean(dataReader["IsErroroccured"]);
                                }
                            }

                            partPaymentSchema.Add(schema);
                        }
                        calculatePartPayment.TotalAmount = partPaymentSchema.Sum(x => x.InstallmentAmount);
                        calculatePartPayment.FirstInstallmentDate = partPaymentSchema[0].DueDate;
                        calculatePartPayment.LastInstallmentDate = partPaymentSchema[partPaymentSchema.Count - 1].DueDate;
                        calculatePartPayment.CaseCost = caseCost;
                        calculatePartPayment.ForwardCharge = forwardCharge;
                        calculatePartPayment.FirstInstallmentAmount = partPaymentSchema[0].InstallmentAmount;
                        calculatePartPayment.LastInstallmentAmount = partPaymentSchema[partPaymentSchema.Count - 1].InstallmentAmount;
                        calculatePartPayment.SchemaList = partPaymentSchema;
                    }
                    return calculatePartPayment;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error executing stored procedure {storedProcedure} for caseNumber {_partPayment.CaseId}", ex);
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using US.BOX.BoxAPI.Data.Models;
using US.BOX.BoxAPI.Data.Utils;
using US.BOX.DataAccess.DBFramework;

namespace US.BOX.BoxAPI.Data.Actions
{
    public class RegisterPaymentAgreementAction(USPPaymentAgreement _paymentAgreement) : DBActionBase<int>
    {
        public override async Task<int> ExecuteAsync(string connectionString, CancellationToken cancellationToken = default)  // Task<int> is expected here
        {
            const string storedProcedureName = "dpapi.USP_AddPaymentAgreement";
            using (var command = await CreateCommandAsync(storedProcedureName, CommandType.StoredProcedure, connectionString, cancellationToken))
            {
                command.Parameters.Add(DataAcessUtils.CreateParam("@PayDate", DbType.DateTime, _paymentAgreement.PayDate));
                command.Parameters.Add(DataAcessUtils.CreateParam("@MainAmount", DbType.Decimal, _paymentAgreement.MainAmount));
                command.Parameters.Add(DataAcessUtils.CreateParam("@InterestMainFee", DbType.Decimal, _paymentAgreement.InterestMainAmount));
                command.Parameters.Add(DataAcessUtils.CreateParam("@InterestCostFee", DbType.Decimal, _paymentAgreement.InterestCostAmount));
                command.Parameters.Add(DataAcessUtils.CreateParam("@CollectionFee", DbType.Decimal, _paymentAgreement.CollectionFee));
                command.Parameters.Add(DataAcessUtils.CreateParam("@CourtFee ", DbType.Decimal, _paymentAgreement.CourtFee));
                command.Parameters.Add(DataAcessUtils.CreateParam("@CostFee ", DbType.Decimal, _paymentAgreement.CostFee));
                command.Parameters.Add(DataAcessUtils.CreateParam("@OtherFee ", DbType.Decimal, _paymentAgreement.OtherFee));
                command.Parameters.Add(DataAcessUtils.CreateParam("@Total ", DbType.Decimal, _paymentAgreement.TotalAmount));
                command.Parameters.Add(DataAcessUtils.CreateParam("@IntMainAmount ", DbType.Decimal, _paymentAgreement.InterestMainAmount));
                command.Parameters.Add(DataAcessUtils.CreateParam("@IntCollectionAmount ", DbType.Decimal, _paymentAgreement.InterestCollectionAmount));
                command.Parameters.Add(DataAcessUtils.CreateParam("@IntCostAmount ", DbType.Decimal, _paymentAgreement.InterestCostAmount));
                command.Parameters.Add(DataAcessUtils.CreateParam("@IntCourtAmount ", DbType.Decimal, _paymentAgreement.InterestCourtAmount));
                command.Parameters.Add(DataAcessUtils.CreateParam("@IntOtherAmount ", DbType.Decimal, _paymentAgreement.InterestOtherAmount));
                command.Parameters.Add(DataAcessUtils.CreateParam("@CaseNo", DbType.Int32, _paymentAgreement.CaseNo));
                command.Parameters.Add(DataAcessUtils.CreateParam("@Type", DbType.String, _paymentAgreement.AgreementType));
                command.Parameters.Add(DataAcessUtils.CreateParam("@IsSendSMSNotification", DbType.Boolean, _paymentAgreement.IsSendSMSNotification));
                command.Parameters.Add(DataAcessUtils.CreateParam("@InterestDate", DbType.DateTime, _paymentAgreement.InterestDate));
                command.Parameters.Add(DataAcessUtils.CreateParam("@InterestMainBalance", DbType.Decimal, _paymentAgreement.InterestMainBalance));
                command.Parameters.Add(DataAcessUtils.CreateParam("@InterestCostBalance", DbType.Decimal, _paymentAgreement.InterestCostBalance));
                try
                {
                    if (_paymentAgreement.ID > 0)
                    {
                        command.Parameters.Add(DataAcessUtils.CreateParam("@Id", DbType.Int32, _paymentAgreement.ID));
                    }

                    SqlParameter outPutIDParameter = new SqlParameter
                    {
                        ParameterName = "@Agreementid",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(outPutIDParameter);
                    SqlParameter outPutErrorParameter = new SqlParameter
                    {
                        ParameterName = "@ErrorText",
                        SqlDbType = SqlDbType.NVarChar,
                        Direction = ParameterDirection.Output,
                        Size = 100
                    };
                    command.Parameters.Add(outPutErrorParameter);
                    await command.ExecuteNonQueryAsync();
                    //if (Int32.TryParse(outPutIDParameter.Value.ToString(), out int Agreementid))
                    //{


                    //    return new RegisterPaymentAgreementResponse() { AgreementId = Agreementid, Error = outPutErrorParameter.Value.ToString() };

                    //}
                    //else
                    //{

                    //    return new RegisterPaymentAgreementResponse() { AgreementId = Agreementid, Error = (outPutErrorParameter.Value == DBNull.Value) ? string.Empty : outPutErrorParameter.Value.ToString() };
                    //}
                    return Convert.ToInt32(outPutIDParameter.Value.ToString());
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error executing stored procedure {storedProcedureName} for caseNumber {_paymentAgreement.CaseNo}", ex);
                }
            }
        }
    }
}

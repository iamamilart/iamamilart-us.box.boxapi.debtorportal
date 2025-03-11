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
    public class GetPortalCasesByDebtorAction(string ssno, int caseNumber) : DBActionBase<List<DebtorCase>>
    {
        public override async Task<List<DebtorCase>> ExecuteAsync(string connectionString, CancellationToken cancellationToken = default)
        {
            const string storedProcedure = "[dpapi].[GetPortalCasesByDebtor]";
            var result = new List<DebtorCase>();

            try
            {
                using var command = await CreateCommandAsync(storedProcedure, CommandType.StoredProcedure, connectionString, cancellationToken);
                //command.Parameters.Add(new SqlParameter("@caseNumber", SqlDbType.Int)).Value = caseNumber;
                command.Parameters.Add(DataAcessUtils.CreateParam("@ssno", DbType.Int32, ssno));
                command.Parameters.Add(DataAcessUtils.CreateParam("@caseNumber", DbType.Int32, caseNumber));

                using var dataReader = await command.ExecuteReaderAsync(cancellationToken);
                while (await dataReader.ReadAsync(cancellationToken))
                {
                    cancellationToken.ThrowIfCancellationRequested(); // Check cancellation between rows

                    DebtorCase caseobj = new DebtorCase();
                    caseobj.Balance = dataReader.GetDecimal("balance");
                    caseobj.MainAmount = dataReader.GetDecimal("mainAmount");
                    caseobj.CaseNumber = dataReader.GetInt32("caseno");
                    caseobj.creditorID = dataReader.GetString("creditorId");
                    caseobj.CreditorName = dataReader.GetString("creditorName");
                    caseobj.DebtorName = dataReader.GetString("debtorName");
                    caseobj.DebtorNumber = dataReader.GetString("debtorNo");
                    caseobj.ExtCaseNumber = Convert.ToInt32(dataReader.GetString("exCaseNo"));
                    caseobj.Status = dataReader.GetString("status");
                    caseobj.Password = dataReader.GetString("password");
                    caseobj.Date = dataReader.GetDateTime("regdate");
                    result.Add(caseobj);
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error executing stored procedure {storedProcedure} for caseNumber {caseNumber}", ex);
            }
        }
    }
}

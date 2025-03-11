using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using US.BOX.BoxAPI.Data.Models;
using US.BOX.BoxAPI.Data.Utils;
using US.BOX.DataAccess.DBFramework;

namespace US.BOX.BoxAPI.Data.Actions
{
    public class GetCasesByDebtorsAction(List<int> EntRoleidList, int caseno) : DBActionBase<List<DebtorCase>>
    {
        public override async Task<List<DebtorCase>> ExecuteAsync(string connectionString, CancellationToken cancellationToken = default)
        {
            List<DebtorCase> returnedCases = new List<DebtorCase>();
            const string storedProcedure = "dpapi.DP_GetCasesByDebtors";
            try
            {
                using var command = await CreateCommandAsync(storedProcedure, CommandType.StoredProcedure, connectionString, cancellationToken);
               // command.Parameters.Add(DataAcessUtils.CreateParam("@EntRoleIDList", SqlDbType.Structured, EntRoleidList);
                command.Parameters.Add(DataAcessUtils.CreateParam("@caseNumber", DbType.Int32, caseno));
                SqlParameter param = new SqlParameter("@EntRoleIDList", SqlDbType.Structured);
                param.Direction = ParameterDirection.Input;
                param.Value = GetIDTable(EntRoleidList);
                command.Parameters.Add(param);

                using var dataReader = await command.ExecuteReaderAsync(cancellationToken);
                while (await dataReader.ReadAsync(cancellationToken))
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    DebtorCase debtorCase = new DebtorCase();
                    debtorCase.Balance = (Decimal)GetSafeValue(dataReader,"balance");
                    debtorCase.MainAmount = (Decimal)GetSafeValue(dataReader,"mainAmount");
                    debtorCase.CaseNumber = (Int32)GetSafeValue(dataReader,"caseno");
                    debtorCase.creditorID = (string)GetSafeValue(dataReader,"creditorId");
                    debtorCase.CreditorName = (string)GetSafeValue(dataReader,"creditorName");
                    debtorCase.DebtorName = (string)GetSafeValue(dataReader,"debtorName");
                    debtorCase.DebtorNumber = (string)GetSafeValue(dataReader,"debtorNo");
                    debtorCase.ExtCaseNumber = (Int32)GetSafeValue(dataReader,"exCaseNo");
                    debtorCase.Status = (string)GetSafeValue(dataReader,"status");
                    debtorCase.Password = (string)GetSafeValue(dataReader,"password");
                    debtorCase.Date = (DateTime)GetSafeValue(dataReader,"regdate");
                    debtorCase.PhoneNumber = (string)GetSafeValue(dataReader,"debtorMobile");
                    debtorCase.Email = (string)GetSafeValue(dataReader,"debtorEmail");
                    debtorCase.DateOfBirth = (DateTime)GetSafeValue(dataReader,"birthDate");
                    returnedCases.Add(debtorCase);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetCasesByDebtorsAction", ex);
            }
            return returnedCases;
        }
        private DataTable GetIDTable(List<int> _entRoleIdList)
        {
            DataTable table = new DataTable();
            table.Columns.Add("ID", typeof(string));

            foreach (int id in _entRoleIdList)
            {
                var row = table.NewRow();
                row["ID"] = id;
                table.Rows.Add(row);
            }
            return table;
        }
    }
}

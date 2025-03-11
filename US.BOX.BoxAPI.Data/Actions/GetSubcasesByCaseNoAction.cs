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
    public class GetSubcasesByCaseNoAction(int CaseNo):DBActionBase<List<SubCase>>
    {
        public override async Task<List<SubCase>> ExecuteAsync(string connectionString, CancellationToken cancellationToken = default)
        {
            List<SubCase> subCases = new List<SubCase>();
            string storedProcedure = "[dpapi].[GetSubcasesByCaseNo]";
            try
            {
                using var command = await CreateCommandAsync(storedProcedure, CommandType.StoredProcedure, connectionString, cancellationToken);
                command.Parameters.Add(DataAcessUtils.CreateParam("@CaseNo", DbType.Int32, CaseNo));

                using var dataReader = await command.ExecuteReaderAsync(cancellationToken);
  
                while (await dataReader.ReadAsync(cancellationToken))
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    SubCase subCase = new SubCase();
                    if (DBNull.Value != dataReader["SubCaseNumber"] && !string.IsNullOrWhiteSpace(dataReader["SubCaseNumber"].ToString()))
                    {
                        subCase.SubCaseNumber = Convert.ToInt32(dataReader["SubCaseNumber"]);
                    }
                    if (DBNull.Value != dataReader["InvoiceRef"])
                    {
                        subCase.InvoiceRef = Convert.ToString(dataReader["InvoiceRef"]);
                    }
                    if (DBNull.Value != dataReader["MainAmount"])
                    {
                        subCase.MainAmount = Convert.ToDecimal(dataReader["MainAmount"]);
                    }
                    if (DBNull.Value != dataReader["Balance"])
                    {
                        subCase.Balance = Convert.ToDecimal(dataReader["Balance"]);
                    }
                    if (DBNull.Value != dataReader["KID"])
                    {
                        subCase.KID = Convert.ToString(dataReader["KID"]);
                    }
                    subCase.ExCaseNo = Convert.ToString(dataReader["ExCaseNo"]);
                    subCases.Add(subCase);

                }


            }
            catch (Exception ex)
            {
                throw new Exception($"Error executing stored procedure {storedProcedure} for caseNumber {CaseNo}", ex);
            }
            return subCases;
        }
    }
}


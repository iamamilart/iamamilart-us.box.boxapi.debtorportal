using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using US.BOX.BoxAPI.Data.Utils;
using US.BOX.DataAccess.DBFramework;

namespace US.BOX.BoxAPI.Data.Actions
{
    public class GetCaseMetaDataAction(int caseNo):DBActionBase<string>
    {
        public override async Task<string> ExecuteAsync(string connectionString, CancellationToken cancellationToken = default)
        {
            string CaseType = string.Empty;
            string storedProcedure = "dpapi.GetCaseMetaData";
            try
            {
                using var command = await CreateCommandAsync(storedProcedure, CommandType.StoredProcedure, connectionString, cancellationToken);
                command.Parameters.Add(DataAcessUtils.CreateParam("@CaseNo", DbType.Int32, caseNo));

                using var dataReader = await command.ExecuteReaderAsync(cancellationToken);
                while (await dataReader.ReadAsync(cancellationToken))
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    CaseType = Convert.ToString(dataReader["CaseType"].ToString() ?? "");
                }


            }
            catch (Exception ex)
            {
                throw new Exception($"Error executing stored procedure {storedProcedure} for caseNumber {caseNo}", ex);
            }
            return CaseType;
        }
    }
}


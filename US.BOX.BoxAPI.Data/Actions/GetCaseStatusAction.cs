using System;
using System.Data.Common;
//using System.Data.SqlClient;
using System.Data;
using US.BOX.DataAccess.DBFramework;
using US.BOX.BoxAPI.Data.Utils;

namespace US.BOX.BoxAPI.Data.Actions
{
    public class GetCaseStatusAction : DBActionBase<string>
    {
        private readonly int _caseNumber;
        public GetCaseStatusAction(int caseNumber)
        {
            this._caseNumber = caseNumber;
        }

        public override async Task<string> ExecuteAsync(string connectionString, CancellationToken cancellationToken = default)
        {
            string caseStatus = string.Empty;
            string storedProcedure = "dpapi.GetCaseStatusByCaseNumber";
            try
            {
                using var command = await CreateCommandAsync(storedProcedure, CommandType.StoredProcedure, connectionString, cancellationToken);
                command.Parameters.Add(DataAcessUtils.CreateParam("@CaseNumber", DbType.Int32, this._caseNumber));

                using var dataReader = await command.ExecuteReaderAsync(cancellationToken);
                while (await dataReader.ReadAsync(cancellationToken))
                {
                    cancellationToken.ThrowIfCancellationRequested(); 

                   // caseStatus = Convert.ToString(dataReader["CaseStatus"].ToString() ?? "" );
                    caseStatus = (string)GetSafeValue(dataReader, "CaseStatus");
                }
               

            }
            catch(Exception ex)
            {
                throw new Exception($"Error executing stored procedure {storedProcedure} for caseNumber {_caseNumber}", ex);
            }
            return caseStatus;
        }
    }
}

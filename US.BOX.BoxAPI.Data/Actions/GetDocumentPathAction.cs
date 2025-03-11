using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using US.BOX.BoxAPI.Data.Utils;
using US.BOX.DataAccess.DBFramework;

namespace US.BOX.BoxAPI.Data.Actions
{
    public class GetDocumentPathAction(int CaseNo, string RefNo, string DocType,int ActivityCode) : DBActionBase<string>
    {
        public override async Task<string> ExecuteAsync(string connectionString, CancellationToken cancellationToken = default)
        {
            string documentPath = "";
            string storedProcedure = "[dpapi].[GetDocumentPath]";// "documentservice.GetDocumentPath";
            try
            {
                using var command = await CreateCommandAsync(storedProcedure, CommandType.StoredProcedure, connectionString, cancellationToken);
                command.Parameters.Add(DataAcessUtils.CreateParam("@CaseNo", DbType.Int32, CaseNo));
                command.Parameters.Add(DataAcessUtils.CreateParam("@RefNo", DbType.String, RefNo));
                command.Parameters.Add(DataAcessUtils.CreateParam("@DocType", DbType.String, DocType));

                command.Parameters.Add(DataAcessUtils.CreateParam("@ActivityCode", DbType.Int32, ActivityCode));

                using var dataReader = await command.ExecuteReaderAsync(cancellationToken);
                while (await dataReader.ReadAsync(cancellationToken))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    documentPath=Convert.ToString(dataReader["DocPath"]);
                }

            }
            catch (Exception ex)
            {
                throw new Exception($"Error executing stored procedure {storedProcedure} for lookupId {CaseNo}", ex);
            }
            return documentPath;
        }
    }
}

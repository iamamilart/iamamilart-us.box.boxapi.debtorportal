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
    public class GetDocumentPathV4Action(int lookupId, string system, string lookupType) : DBActionBase<List<string>>
    {
        public override async Task<List<string>> ExecuteAsync(string connectionString, CancellationToken cancellationToken = default)
        {
            List<string> documentPaths = new List<string>();
            string storedProcedure = "[dpapi].[GetDocumentPathV4]";
            try
            {
                using var command = await CreateCommandAsync(storedProcedure, CommandType.StoredProcedure, connectionString, cancellationToken);
                command.Parameters.Add(DataAcessUtils.CreateParam("@LookupId", DbType.Int32, lookupId));
                command.Parameters.Add(DataAcessUtils.CreateParam("@System", DbType.String, system));
                command.Parameters.Add(DataAcessUtils.CreateParam("@LookupType", DbType.String, lookupType));

                using var dataReader = await command.ExecuteReaderAsync(cancellationToken);
                while (await dataReader.ReadAsync(cancellationToken))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    documentPaths.Add(Convert.ToString(dataReader["DocumentPath"]));
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error executing stored procedure {storedProcedure} for lookupId {lookupId}", ex);
            }
            return documentPaths;
        }
    }
}

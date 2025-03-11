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
    public class GetDocumentPathByDocIdAction(string documentId) : DBActionBase<KeyValuePair<string, string>>
    {
      

        public override async Task<KeyValuePair<string, string>> ExecuteAsync(string connectionString, CancellationToken cancellationToken = default)
        {
            KeyValuePair<string, string> docData = new KeyValuePair<string, string>();
            string storedProcedure = "dpapi.GetDocumentPathByDocId";
            try
            {
                using var command = await CreateCommandAsync(storedProcedure, CommandType.StoredProcedure, connectionString, cancellationToken);
                command.Parameters.Add(DataAcessUtils.CreateParam("@DocumentId", DbType.String, documentId));

                using var dataReader = await command.ExecuteReaderAsync(cancellationToken);
                while (await dataReader.ReadAsync(cancellationToken))
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    docData = new KeyValuePair<string, string>(Convert.ToString(dataReader["docType"].ToString() ??""), Convert.ToString(dataReader["docPath"].ToString() ?? ""));
                }


            }
            catch (Exception ex)
            {
                throw new Exception($"Error executing stored procedure {storedProcedure} for documentId  {documentId}", ex);
            }
            return docData;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using US.BOX.BoxAPI.Data.Models;
using US.BOX.BoxAPI.Data.Utils;

namespace US.BOX.BoxAPI.Data.Actions
{
    public class GetEntityDocumentDetailsAction(int entityId, string entityType, bool addedToHistory) : DBActionBase<List<EntityDocumentDetails>>
    {
        public override async Task<List<EntityDocumentDetails>> ExecuteAsync(string connectionString, CancellationToken cancellationToken = default)
        {
            List<EntityDocumentDetails> entityDocument = new List<EntityDocumentDetails>();
            string storedProcedure = @"[dpapi].[GetEntityDocumentDetails]";
            try
            {
                using var command = await CreateCommandAsync(storedProcedure, CommandType.StoredProcedure, connectionString, cancellationToken);
              
                command.Parameters.Add(new SqlParameter("@EntityId", entityId));
                command.Parameters.Add(new SqlParameter("@EntityType", entityType));
                command.Parameters.Add(new SqlParameter("@AddToHistory", addedToHistory));

                using var dataReader = await command.ExecuteReaderAsync(cancellationToken);
                string errorMessage = null;
                while (await dataReader.ReadAsync(cancellationToken))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    errorMessage = Convert.ToString(dataReader["ErrorMessage"]);
                    if (string.IsNullOrEmpty(errorMessage))
                    {
                        EntityDocumentDetails document = new EntityDocumentDetails();
                        if (DBNull.Value != dataReader["DocumentId"])
                        {
                            document.DocumentId = Convert.ToInt32(dataReader["DocumentId"]);
                        }
                        if (DBNull.Value != dataReader["DocumentPath"])
                        {
                            document.DocumentName = Path.GetFileName(Convert.ToString(dataReader["DocumentPath"]));
                            document.DocumentType = Path.GetExtension(Convert.ToString(dataReader["DocumentPath"]));
                        }
                        if (DBNull.Value != dataReader["DocumentPath"])
                        {
                            document.DocumentPath = Convert.ToString(dataReader["DocumentPath"]);
                        }
                        if (DBNull.Value != dataReader["DocumentCategory"])
                        {
                            document.DocumentCategory = Convert.ToString(dataReader["DocumentCategory"]);
                        }
                        if (DBNull.Value != dataReader["StorageType"])
                        {
                            document.StorageType = Convert.ToString(dataReader["StorageType"]);
                        }
                        if (DBNull.Value != dataReader["UploadedDate"])
                        {
                            document.UploadedDate = Convert.ToDateTime(dataReader["UploadedDate"]);
                        }
                        if (DBNull.Value != dataReader["UploadedBy"])
                        {
                            document.UploadedBy = Convert.ToString(dataReader["UploadedBy"]);
                        }
                        if (DBNull.Value != dataReader["EntityType"])
                        {
                            document.EntityType = Convert.ToString(dataReader["EntityType"]);
                        }
                        entityDocument.Add(document);
                    }
                }


            }
            catch (Exception ex)
            {
                throw new Exception($"Error executing stored procedure {storedProcedure} for entityID  {entityId}", ex);
            }
            return entityDocument;
        }
    }
}

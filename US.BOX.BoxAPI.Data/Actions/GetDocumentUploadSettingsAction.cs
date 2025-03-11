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
    public class GetDocumentUploadSettingsAction(int entityId, string entityType) : DBActionBase<UploadSettings>
    {
        public override async Task<UploadSettings> ExecuteAsync(string connectionString, CancellationToken cancellationToken = default)
        {
            const string storedProcedure = "[dpapi].[GetDocumentUploadSettings]";
            UploadSettings uploadSettings = new UploadSettings();

            try
            {
                using var command = await CreateCommandAsync(storedProcedure, CommandType.StoredProcedure, connectionString, cancellationToken);
                command.Parameters.Add(new SqlParameter("@EntityId", entityId));
                command.Parameters.Add(new SqlParameter("@EntityType", entityType));

                using var dataReader = await command.ExecuteReaderAsync(cancellationToken);
                while (await dataReader.ReadAsync(cancellationToken))
                {
                    cancellationToken.ThrowIfCancellationRequested(); // Check cancellation between rows

                    string errorMessage = Convert.ToString(dataReader["ErrorMessage"]);
                    if (string.IsNullOrEmpty(errorMessage))
                    {
                        uploadSettings.MaxFileUploadSize = (Int32)GetSafeValue(dataReader,"MaxFileUploadSize");
                        uploadSettings.MaxFileUploadCount = (Int32)GetSafeValue(dataReader,"MaxFileUploadCount");
                        uploadSettings.StorageType = (string)GetSafeValue(dataReader,"StorageType");
                        uploadSettings.BlobContainerName = (string)GetSafeValue(dataReader,"BlobContainerName");
                        uploadSettings.AllowedFileTypes = (string)GetSafeValue(dataReader,"AllowedFileTypes");
                    }
                    else
                    {
                        uploadSettings.ErrorMessage = errorMessage;
                    }
                }

                return uploadSettings;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error executing stored procedure {storedProcedure} for caseNumber {entityId}", ex);
            }
        }
    }
}

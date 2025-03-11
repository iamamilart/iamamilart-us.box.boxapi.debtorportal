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
    public class AddUploadedDocumentsAction(FileUploadRequest fileUploadRequest, string storageType): DBActionBase<List<UploadDocumentResponse>>
    {
        private static DataTable CreateTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("DocumentPath", typeof(string));
            dt.Columns.Add("DocumentName", typeof(string));
            dt.Columns.Add("DocumentType", typeof(string));
            dt.Columns.Add("StorageType", typeof(string));
            return dt;
        }

        public override async Task<List<UploadDocumentResponse>> ExecuteAsync(string connectionString, CancellationToken cancellationToken = default)
        {
            List<UploadDocumentResponse> uploadDocumentResponses = new List<UploadDocumentResponse>();

            DataTable dtPorperties = CreateTable();
            int id = 1;
            foreach (var uploadedFile in fileUploadRequest.UploadedDocuments)
            {
                if (storageType == "BlobStorage")
                {
                    Uri uri = new Uri(uploadedFile.DocumentPath);
                    string documentPath = "az:" + uri.LocalPath.TrimStart('/');
                    dtPorperties.Rows.Add(id, documentPath, Path.GetFileName(uploadedFile.DocumentPath), fileUploadRequest.DocumentType
                        , "BlobStorage");
                }
                else
                {
                    dtPorperties.Rows.Add(id, uploadedFile.DocumentPath, Path.GetFileName(uploadedFile.DocumentPath), fileUploadRequest.DocumentType
                        , "LocalStorage");
                }
                id++;
            }

            string storedProcedure = "[dpapi].[AddUploadedDocuments]";
            try
            {
                using var command = await CreateCommandAsync(storedProcedure, CommandType.StoredProcedure, connectionString, cancellationToken);
                //DataTable dt = CreateTable();


                 command.Parameters.Add(DataAcessUtils.CreateParam("@entityId", System.Data.DbType.Int32, fileUploadRequest.EntityId));
                command.Parameters.Add(DataAcessUtils.CreateParam("@EntityType", System.Data.DbType.String, fileUploadRequest.EntityType));
                command.Parameters.Add(DataAcessUtils.CreateParam("@NoteText", System.Data.DbType.String, fileUploadRequest.NoteText));
                command.Parameters.Add(DataAcessUtils.CreateParam("@AddToHistory", System.Data.DbType.Boolean, fileUploadRequest.AddToHistory));
                var parameter = new SqlParameter("@DocumentDetailsList", SqlDbType.Structured) { Value = dtPorperties };
                command.Parameters.Add(parameter);

                using var dataReader = await command.ExecuteReaderAsync(cancellationToken);
                while (await dataReader.ReadAsync(cancellationToken))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    UploadDocumentResponse uploadDocumentResponse = new UploadDocumentResponse();
                    if (DBNull.Value != dataReader["DocumentId"])
                    {
                        uploadDocumentResponse.DocumentId = Convert.ToInt32(dataReader["DocumentId"]);
                    }
                    uploadDocumentResponse.Message = "Success";
                    uploadDocumentResponse.IsSuccess = true;
                    uploadDocumentResponses.Add(uploadDocumentResponse);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error executing stored procedure {storedProcedure}", ex);
            }
            return uploadDocumentResponses;
        }
    }
}

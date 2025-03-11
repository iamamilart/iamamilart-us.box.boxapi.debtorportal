using Azure.Messaging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using US.BOX.BoxAPI.Data.Utils;
using US.BOX.DataAccess.DBFramework;

namespace US.BOX.BoxAPI.Data.Actions
{
    public class UpdateCommunicationQueueAction(string entityId, string entityType, string templateName,
                                         string templateCategory, int pendingexecId, int isPreview, int activityId, string activity, int invGrpNo, string activityversion) : DBActionBase<int>
    {
        public override async Task<int> ExecuteAsync(string connectionString, CancellationToken cancellationToken = default)
        {
            int response = -1;
            const string storedProcedure = "[dpapi].[UpdateCommunicationQueue]";
            string messageContent = string.Empty;
            string msgAddressOrNo = string.Empty;
            string emailAttachment = string.Empty;
            int domainTextCJId = -1;

            using (var command = await CreateCommandAsync(storedProcedure, CommandType.StoredProcedure, connectionString, cancellationToken))
            {
                
                command.Parameters.Add(DataAcessUtils.CreateParam("@EntityId", DbType.String, entityId));
                command.Parameters.Add(DataAcessUtils.CreateParam("@EntityType", DbType.String, entityType));
                command.Parameters.Add(DataAcessUtils.CreateParam("@TemplateName", DbType.String, templateName));
                command.Parameters.Add(DataAcessUtils.CreateParam("@TemplateCategory", DbType.String, templateCategory));
                command.Parameters.Add(DataAcessUtils.CreateParam("@PendingExecuteID", DbType.Int32, pendingexecId));
                command.Parameters.Add(DataAcessUtils.CreateParam("@Preview", DbType.Int32, isPreview));
                command.Parameters.Add(DataAcessUtils.CreateParam("@ActivityId", DbType.Int32, activityId));
                command.Parameters.Add(DataAcessUtils.CreateParam("@ActivityName", DbType.String, activity));
                command.Parameters.Add(DataAcessUtils.CreateParam("@IGroupNo", DbType.Int32, invGrpNo));
                if (!string.IsNullOrEmpty(messageContent))
                {
                    command.Parameters.Add(DataAcessUtils.CreateParam("@MessageContent", DbType.String, messageContent));
                }
                if (!string.IsNullOrEmpty(msgAddressOrNo))
                {
                    command.Parameters.Add(DataAcessUtils.CreateParam("@AddressOrNumber", DbType.String, msgAddressOrNo));
                }
                if (!string.IsNullOrEmpty(emailAttachment))
                {
                    command.Parameters.Add(DataAcessUtils.CreateParam("@EmailAttachment", DbType.String, emailAttachment));
                }
                if (domainTextCJId > 0)
                {
                    command.Parameters.Add(DataAcessUtils.CreateParam("@domainTextCJId", DbType.Int32, domainTextCJId));
                }

                command.Parameters.Add(DataAcessUtils.CreateParam("@ActivityVersion", DbType.String, activityversion));

                try
                {
                    DbParameter paraQueId = new SqlParameter();
                    paraQueId.DbType = DbType.Int32;
                    paraQueId.ParameterName = "@QueueId";
                    paraQueId.Direction = ParameterDirection.Output;

                    command.Parameters.Add(paraQueId);
                    await command.ExecuteNonQueryAsync(cancellationToken);
                    cancellationToken.ThrowIfCancellationRequested(); // Check cancellation between rows
                    response = Convert.ToInt32(paraQueId.Value);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error executing stored procedure {storedProcedure} for entityId {entityId}", ex);
                }
            }
            return response;
        }

       
    }
}

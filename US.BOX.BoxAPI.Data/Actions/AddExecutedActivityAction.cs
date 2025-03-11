using Microsoft.VisualBasic;
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
    public  class AddExecutedActivityAction(int rowId, int queueId, string activitiyName, string routineName, int entityId,
                                 string errorText,
                                 string entityType, string user, string param, DateTime dueDate, int invGrpNo, string attachmentPath, string activityversion, bool isSuccess) : DBActionBase<int>

    {
        public override async Task<int> ExecuteAsync(string connectionString, CancellationToken cancellationToken = default)
        {
            string storedProcedure = GetSpName(entityType);
            using (var command = await CreateCommandAsync(storedProcedure, CommandType.StoredProcedure, connectionString, cancellationToken))
            {
                command.Parameters.Add(DataAcessUtils.CreateParam("@RowId", DbType.Int32, rowId));
                command.Parameters.Add(DataAcessUtils.CreateParam("@QueueId", DbType.Int32, queueId));
                command.Parameters.Add(DataAcessUtils.CreateParam("@ActivityName", DbType.String, activitiyName));
                command.Parameters.Add(DataAcessUtils.CreateParam("@RoutineName", DbType.String, routineName));
                command.Parameters.Add(DataAcessUtils.CreateParam("@EntityId", DbType.Int32, entityId));
                command.Parameters.Add(DataAcessUtils.CreateParam("@ErrorText", DbType.String, errorText));
                command.Parameters.Add(DataAcessUtils.CreateParam("@EntityType", DbType.String, entityType));
                command.Parameters.Add(DataAcessUtils.CreateParam("@User", DbType.String, user));
                command.Parameters.Add(DataAcessUtils.CreateParam("@Param", DbType.String, param));
                command.Parameters.Add(DataAcessUtils.CreateParam("@DueDate", DbType.DateTime, dueDate));
                command.Parameters.Add(DataAcessUtils.CreateParam("@InvGrpNo", DbType.Int32, invGrpNo));
                command.Parameters.Add(DataAcessUtils.CreateParam("@AttachmentPath", DbType.String, attachmentPath));
                if (dueDate != DateTime.MinValue)
                {
                    command.Parameters.Add(DataAcessUtils.CreateParam("@DueDate", DbType.DateTime, dueDate));
                }
                if (!string.IsNullOrEmpty(attachmentPath))
                {
                    command.Parameters.Add(DataAcessUtils.CreateParam("@Attachmentpath", DbType.String, attachmentPath));
                }
                command.Parameters.Add(DataAcessUtils.CreateParam("@ActivityVersion", DbType.String, activityversion));

                try
                {
                    var res = await command.ExecuteNonQueryAsync(cancellationToken);
                    cancellationToken.ThrowIfCancellationRequested(); // Check cancellation between rows
                    return res;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error executing stored procedure {storedProcedure} for rowId {rowId}", ex);
                }
            }
        }

        private string GetSpName(string entityType)
        {
            switch (entityType.Trim())
            {
                case "SubCase":
                    return "[dpapi].[USP_AddExecutedActivitySubCase]";
                case "Case":
                    return "[dpapi].[USP_AddExecutedActivityCase]";
                case "Debtor":
                case "AR":
                    return "[dpapi].[USP_AddExecutedActivityDebtor]";
                case "Creditor":
                    return "[dpapi].[USP_AddExecutedActivityCreditor]";
                case "Bureau":
                    return "[dpapi].[USP_AddExecutedActivityBureau]";
                case "Payment":
                    return "[dpapi].[USP_AddExecutedActivityPayment]";
            }
            return string.Empty;
        }

    }
}

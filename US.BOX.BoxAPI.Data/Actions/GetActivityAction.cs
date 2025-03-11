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
    public class GetActivityAction(string activityCode, int entityId, string entityType) : DBActionBase<ActivityExecutionDetail>
    {
        public override async Task<ActivityExecutionDetail> ExecuteAsync(string connectionString, CancellationToken cancellationToken = default)
        {
            ActivityExecutionDetail activity = new ActivityExecutionDetail();
            string storedProcedure = "[dpapi].[GetActivity]";
            try
            {
                using var command = await CreateCommandAsync(storedProcedure, CommandType.StoredProcedure, connectionString, cancellationToken);
                command.Parameters.Add(DataAcessUtils.CreateParam("@ActivityCode", DbType.String, activityCode));
                command.Parameters.Add(DataAcessUtils.CreateParam("@EntityId", DbType.Int32, entityId));
                command.Parameters.Add(DataAcessUtils.CreateParam("@EntityType", DbType.String, entityType));

                
                using var dataReader = await command.ExecuteReaderAsync(cancellationToken);
                while (await dataReader.ReadAsync(cancellationToken))
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    activity.Id =(Int32) GetSafeValue(dataReader,"ID");
                    activity.Name =(string) GetSafeValue(dataReader,"Name");
                    activity.Type = (string)GetSafeValue(dataReader,"Type");
                    activity.Settings = (string)GetSafeValue(dataReader,"Settings");
                    activity.Reference = (string)GetSafeValue(dataReader,"Referance");
                    activity.WorkFlowType = (string)GetSafeValue(dataReader,"EntityType");
                    activity.CaseNo = (Int32)GetSafeValue(dataReader,"CaseNo");
                    activity.CategoryName = (string)GetSafeValue(dataReader,"CategoryName");
                    activity.MessageType = (string)GetSafeValue(dataReader,"MessageType");
                    activity.defaultTemplateId = (Int32)GetSafeValue(dataReader,"DefaultTemplateId");
                    activity.ActivityVersion= (string)GetSafeValue(dataReader, "ActivityVersion");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error executing stored procedure {storedProcedure} for activityCode {activityCode}", ex);
            }
            return activity;
        }
    }
    
}

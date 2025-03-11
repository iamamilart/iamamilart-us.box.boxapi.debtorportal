using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using US.BOX.BoxAPI.Data.Models;
using US.BOX.BoxAPI.Data.Utils;
using US.BOX.DataAccess.DBFramework;

namespace US.BOX.BoxAPI.Data.Actions
{
    public class GetActivityServiceSettingsAction(string activityCode, string entityType) : DBActionBase<ActivityServiceSetting>
    {
        public override async Task<ActivityServiceSetting> ExecuteAsync(string connectionString, CancellationToken cancellationToken = default)
        {
            ActivityServiceSetting activityServiceSetting = new ActivityServiceSetting();
            string storedProcedure = @"[dpapi].[GetActivityServiceSettings]";

            using (var command = await CreateCommandAsync(storedProcedure, CommandType.StoredProcedure, connectionString, cancellationToken))
            {
                command.Parameters.Add(DataAcessUtils.CreateParam("@activityCode", DbType.String, activityCode));
                command.Parameters.Add(DataAcessUtils.CreateParam("@entityType", DbType.String, entityType));

                try
                {
                    using var dataReader = await command.ExecuteReaderAsync(cancellationToken);

                    while (await dataReader.ReadAsync(cancellationToken))
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                      
                        activityServiceSetting.ActivityServiceBasePath = (string)GetSafeValue(dataReader,"ActivityServiceURL");
                        activityServiceSetting.BureaueCode = (string)GetSafeValue(dataReader,"BureaueCode");
                        activityServiceSetting.ActivityType = (string)GetSafeValue(dataReader,"ActivityType");
                        activityServiceSetting.ActivityVersion = (string)GetSafeValue(dataReader,"ActivityVersion");
                        activityServiceSetting.CloudStorageUsed = (bool)GetSafeValue(dataReader,"CloudStorageUsed");
                        activityServiceSetting.TemplateCategoryId = (Int32)GetSafeValue(dataReader,"TemplateCategoryId");
                        activityServiceSetting.AttachmentActivityCode = (Int32)GetSafeValue(dataReader,"AttachmentActivityCode");
                        activityServiceSetting.AttachmentActivityCategoryId = (Int32)GetSafeValue(dataReader,"AttachmentActivityCategoryId");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error executing stored procedure {storedProcedure} for activityCode {activityCode}", ex);
                }

            }
            return activityServiceSetting;
        }



    }
}

using Azure.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using US.BOX.BoxAPI.Data.Models;
using US.BOX.BoxAPI.Data.Utils;
using US.BOX.DataAccess.DBFramework;

namespace US.BOX.BoxAPI.Data.Actions
{
    public class CreateDPortalCaseNoteAction(DPCaseNote _request) : DBActionBase<int>
    {
        public override async Task<int> ExecuteAsync(string connectionString, CancellationToken cancellationToken = default)  // Task<int> is expected here
        {
            const string storedProcedure = "dpapi.CreateDPortalCaseNote";

            using (var command = await CreateCommandAsync(storedProcedure, CommandType.StoredProcedure, connectionString, cancellationToken))
            {
                command.Parameters.Add(DataAcessUtils.CreateParam("@CaseNumber", DbType.Int32, _request.ReferenceNumber));
                command.Parameters.Add(DataAcessUtils.CreateParam("@Type", DbType.String, _request.Type));
                command.Parameters.Add(DataAcessUtils.CreateParam("@NoteText", DbType.String, _request.NoteText));
                command.Parameters.Add(DataAcessUtils.CreateParam("@User", DbType.String, _request.CreatedUser));
                command.Parameters.Add(DataAcessUtils.CreateParam("@ActivityName", DbType.String, _request.ActivityName));
                command.Parameters.Add(DataAcessUtils.CreateParam("@Language", DbType.String, _request.Lan));

                try
                {

                    DbParameter para1 = new SqlParameter();
                    para1.DbType = DbType.Int32;
                    para1.ParameterName = "@OutPutId";
                    para1.Direction = ParameterDirection.Output;
                    command.Parameters.Add(para1);
                    await command.ExecuteNonQueryAsync(cancellationToken);
                    cancellationToken.ThrowIfCancellationRequested();
                    if (para1!=null)
                    {
                        return Convert.ToInt32(para1.Value);
                    }
                    else
                    {
                        return -1;
                    }
                    

                }
                catch (Exception ex)
                {
                    return 0;
                    //throw new Exception($"Error executing stored procedure {storedProcedure} for caseNumber {_request.ReferenceNumber}", ex);
                }
            }
        }
    }

}

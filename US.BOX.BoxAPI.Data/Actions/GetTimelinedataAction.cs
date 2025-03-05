using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using US.BOX.BoxAPI.Data.Models;
using US.BOX.BoxAPI.Data.Utils;

namespace US.BOX.BoxAPI.Data.Actions
{
    internal class GetTimelinedataAction(int caseNumber) : DBActionBase<List<Timeline>>
    {
        public override async Task<List<Timeline>> ExecuteAsync(string connectionString, CancellationToken cancellationToken = default)
        {
            List<Timeline> list = new List<Timeline>();
            string storedProcedure = "boxapi.GetTimelineData";
            try
            {
                using var command = await CreateCommandAsync(storedProcedure, CommandType.StoredProcedure, connectionString);
                command.Parameters.Add(new SqlParameter("@CaseNumber", SqlDbType.Int)).Value = caseNumber;

                using var dataReader = await command.ExecuteReaderAsync(cancellationToken);
                while (await dataReader.ReadAsync(cancellationToken))
                {
                    cancellationToken.ThrowIfCancellationRequested(); // Check cancellation between rows
                    Timeline time = new Timeline();
                    time.TimeLineDate = Convert.ToDateTime(dataReader["TimeLineDate"]);
                    time.Balance = Convert.ToDouble(dataReader["Balance"]);
                    time.WorkflowState = Convert.ToString(dataReader["WorkflowState"]);

                    list.Add(time);
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error executing stored procedure {storedProcedure} for caseNumber {caseNumber}", ex);
            }
        }
    }
}
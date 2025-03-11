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
    internal class GetNewPaymentKidAction(int caseno) : DBActionBase<string>
    {
        public override async Task<string> ExecuteAsync(string connectionString, CancellationToken cancellationToken = default)
        {
            string generateKid = "";
            string storedProcedure = "[dpapi].[GenerateMobilePayKID]";
            try
            {
                using var command = await CreateCommandAsync(storedProcedure, CommandType.StoredProcedure, connectionString, cancellationToken);
                //command.Parameters.Add(new SqlParameter("@caseNumber", SqlDbType.Int)).Value = caseNumber;
               // command.Parameters.Add(DataAcessUtils.CreateParam("@caseNumber", DbType.Int32, caseNumber));
                command.Parameters.Add(DataAcessUtils.CreateParam("@Itemid", DbType.Int32, caseno));
                command.Parameters.Add("@Kid", SqlDbType.VarChar, 100);
                command.Parameters["@Kid"].Direction = ParameterDirection.Output;

                await command.ExecuteScalarAsync(cancellationToken);
                 generateKid=command.Parameters["@Kid"].Value.ToString();
                
            }
            catch (Exception ex)
            {
                throw new Exception($"Error executing stored procedure {storedProcedure} for caseNumber {caseno}", ex);
            }
            return generateKid;
        }
    }
}

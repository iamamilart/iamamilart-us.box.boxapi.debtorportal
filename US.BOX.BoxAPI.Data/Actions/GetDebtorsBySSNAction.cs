using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using US.BOX.BoxAPI.Data.Utils;
using US.BOX.DataAccess.DBFramework;

namespace US.BOX.BoxAPI.Data.Actions
{
    public class GetDebtorsBySSNAction(string personNo):DBActionBase<List<int>>
    {
        public override async Task<List<int>> ExecuteAsync(string connectionString, CancellationToken cancellationToken = default)
        {
            List<int> debtorIds = new List<int>();
            string storedProcedure = "[dpapi].[USP_GetDebtorsBySSN]";
            try
            {
                using var command = await CreateCommandAsync(storedProcedure, CommandType.StoredProcedure, connectionString, cancellationToken);
                command.Parameters.Add(DataAcessUtils.CreateParam("@SSN", DbType.String, personNo));

                using var dataReader = await command.ExecuteReaderAsync(cancellationToken);
                while (await dataReader.ReadAsync(cancellationToken))
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    int EntRoleID = (Int32)GetSafeValue(dataReader,"EntRoleID");
                    debtorIds.Add(EntRoleID);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetDebtorsBySSNAction", ex);
            }
            return debtorIds;
        }
    }
    
    
}

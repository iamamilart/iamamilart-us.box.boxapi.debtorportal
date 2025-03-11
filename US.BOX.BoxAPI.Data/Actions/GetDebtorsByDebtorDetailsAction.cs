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
    public class GetDebtorsByDebtorDetailsAction(string dateOfBirth, string phoneNumber):DBActionBase<List<int>>
    {
        public override async Task<List<int>> ExecuteAsync(string connectionString, CancellationToken cancellationToken = default)
        {
            List<int> debtorIds = new List<int>();
            string storedProcedure = "[dpapi].[GetDebtorsByDebtorDetails]";
            try
            {
                using var command = await CreateCommandAsync(storedProcedure, CommandType.StoredProcedure, connectionString, cancellationToken);
                command.Parameters.Add(DataAcessUtils.CreateParam("@dateOfBirth", DbType.String, dateOfBirth));
                command.Parameters.Add(DataAcessUtils.CreateParam("@phoneNumber", DbType.String, phoneNumber));

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
                throw new Exception("Error in GetDebtorsByDebtorDetailsAction", ex);
            }
            return debtorIds;
        }
    }
}

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
    public class UpdateContactAction(int caseNo, Contact contact) : DBActionBase<bool>
    {
        public override async Task<bool> ExecuteAsync(string connectionString, CancellationToken cancellationToken = default)  // Task<int> is expected here
        {
            const string storedProcedure = "[dpapi].[UpdateAddress]";

            using (var command = await CreateCommandAsync(storedProcedure, CommandType.StoredProcedure, connectionString, cancellationToken))
            {
                command.Parameters.Add(DataAcessUtils.CreateParam("@caseno", DbType.Int32, caseNo));
                command.Parameters.Add(DataAcessUtils.CreateParam("@email", DbType.String, contact.Email));
                command.Parameters.Add(DataAcessUtils.CreateParam("@mobile", DbType.String, contact.Mobile));
                command.Parameters.Add(DataAcessUtils.CreateParam("@home", DbType.String, contact.TelHome));
                command.Parameters.Add(DataAcessUtils.CreateParam("@work", DbType.String, contact.TelWork));
                command.Parameters.Add(DataAcessUtils.CreateParam("@fax", DbType.String, contact.Fax));
                command.Parameters.Add(DataAcessUtils.CreateParam("@postalcode", DbType.String, contact.PostalCode));
                command.Parameters.Add(DataAcessUtils.CreateParam("@address", DbType.String, contact.Address));

                try
                {
                    var res = await command.ExecuteNonQueryAsync(cancellationToken);
                    cancellationToken.ThrowIfCancellationRequested(); // Check cancellation between rows

                    return Convert.ToBoolean(res);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error executing stored procedure {storedProcedure} for caseNumber {caseNo}", ex);
                }
            }
        }
    }
}

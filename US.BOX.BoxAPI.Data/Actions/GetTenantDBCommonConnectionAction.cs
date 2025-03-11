using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using US.BOX.BoxAPI.Data.Models;
using US.BOX.BoxAPI.Data.Utils;

namespace US.BOX.BoxAPI.Data.Actions
{
    public class GetTenantDBCommonConnectionAction(string TenantId): DBActionBase<TenentDetail>
    {
        public override async Task<TenentDetail> ExecuteAsync(string connectionString, CancellationToken cancellationToken = default)
        {
            TenentDetail tenent = new TenentDetail();
            string storedProcedure = "[dbo].[GetTenantDBCommonConnection]";
            try
            {
                using var command = await CreateCommandAsync(storedProcedure, CommandType.StoredProcedure, connectionString, cancellationToken);
                command.Parameters.Add(new SqlParameter("@TenantId", TenantId));

                using var dataReader = await command.ExecuteReaderAsync(cancellationToken);
                while (await dataReader.ReadAsync(cancellationToken))
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    tenent.TenentServer=(string) GetSafeValue(dataReader,"TenantServer");
                    tenent.TenentDatabase = (string)GetSafeValue(dataReader, "TenantDatabase");
                    tenent.TenentCommonLogin = (string)GetSafeValue(dataReader, "TenantCommonLogin");
                    tenent.TenentCommonPassword = (string)GetSafeValue(dataReader, "TenantCommonPassword");
                    tenent.IsOnPremiseDB = (bool)GetSafeValue(dataReader, "IsOnPremiseDB");
                    tenent.BlobConnection = (string)GetSafeValue(dataReader, "BlobConnection");
                    tenent.ClientID = (Guid)GetSafeValue(dataReader, "ClientId");
               
                }


            }
            catch (Exception ex)
            {
                throw new Exception($"Error executing stored procedure {storedProcedure} for tenent ID {tenent}", ex);
            }
            return tenent;
        }
    }
}

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
    public  class GetSystemCategoriesAction(string category, int arNo) : DBActionBase<List<SystemCategory>>
    {
        public override async Task<List<SystemCategory>> ExecuteAsync(string connectionString, CancellationToken cancellationToken = default)  // Task<int> is expected here
        {
            const string storedProcedure = "[dpapi].[GetSystemCategories]";
            var items = new List<SystemCategory>();
            using (var command = await CreateCommandAsync(storedProcedure, CommandType.StoredProcedure, connectionString, cancellationToken))
            {
                command.Parameters.Add(DataAcessUtils.CreateParam("@Category", DbType.String, category));
                //command.Parameters.Add(DataAcessUtils.CreateParam("@ArNo", DbType.Int32, arNo));

                try
                {
                    using var dataReader = await command.ExecuteReaderAsync(cancellationToken);
                   
                    while (await dataReader.ReadAsync(cancellationToken))
                    {
                        cancellationToken.ThrowIfCancellationRequested(); // Check cancellation between rows
                        var item = new SystemCategory();
                        item.Id = Convert.ToInt32(dataReader["Id"] ?? 0);
                        item.Value = Convert.ToString(dataReader["Value"].ToString() ?? "");
                        item.EnText = Convert.ToString(dataReader["EnText"].ToString() ?? "");
                        item.NoText = Convert.ToString(dataReader["NoText"].ToString() ?? "");

                        items.Add(item);
                    }

                    return items;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error executing stored procedure {storedProcedure} for arNo {arNo}", ex);
                }
            }
        }
    }
}

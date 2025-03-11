using Azure;
using System;
using System.Threading.Tasks;
using System.Linq;
using US.BOX.BoxAPI.Data.Models;
using Azure.Data.Tables;

namespace US.BOX.BoxAPI.Service.Utils
{
    public class TableStorageUtility
    {
        public static async Task<DocumentEntity> RetrieveDocumentEntityUsingPointQuery(TableClient table, string partitionKey, string rowKey)
        {
            try
            {
                Pageable<DocumentEntity> queryResultsFilter = table.Query<DocumentEntity>(filter: $"PartitionKey eq '{partitionKey}' and RowKey eq '{rowKey}'");
                return queryResultsFilter.FirstOrDefault();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }

        public static async Task<TableClient> GetTableAsync(string tableName, string clientId)
        {
            TableServiceClient tableServiceClient = TableStorageCommon.CreateTableServiceClient(clientId);

            TableClient table = tableServiceClient.GetTableClient(tableName);

            try
            {
                await tableServiceClient.CreateTableAsync(tableName);
                Console.WriteLine("Created Table named: {0}", tableName);
            }
            catch
            {
                Console.WriteLine("Table {0} already exists", tableName);
            }

            return table;
        }

        //public static async Task<Azure.Data.Tables.TableEntity> AddRecordToInvoiceAudit(TableClient table, TableEntity invoiceAudit)
        //{
        //    try
        //    {
        //        var result = await table.UpsertEntityAsync(invoiceAudit);

        //        Console.WriteLine("Request Charge of InsertOrMerge Operation: " + result);

        //        return invoiceAudit;
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e.Message);
        //        throw;
        //    }
        //}
    }
}

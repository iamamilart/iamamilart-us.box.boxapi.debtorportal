using Azure.Data.Tables;
using Azure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace US.BOX.BoxAPI.Service.Utils
{
    public class TableStorageCommon
    {
        public static TableServiceClient CreateTableServiceClient(string clientId)
        {
            TableServiceClient tableServiceClient;
            try
            {
                string accountName = Environment.GetEnvironmentVariable("AzureStorageBlobOptions:AccountName");
                string accountEndPoint = string.Format("https://{0}.table.core.windows.net/", accountName);

                tableServiceClient = new TableServiceClient(new Uri(accountEndPoint), new DefaultAzureCredential(new DefaultAzureCredentialOptions { ManagedIdentityClientId = clientId }));

            }
            catch (FormatException ex)
            {
                throw ex;
            }
            catch (ArgumentException e)
            {
                 throw e;
            }

            return tableServiceClient;
        }
    }
}

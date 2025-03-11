using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace US.BOX.BoxAPI.Data.Models
{
    public static class DPAPIServiceConfig
    {
        public static string CompanyCode { get; set; }
        public static string StorageContainer { get; set; }
        public static string DocumentTableName { get; set; }
        public static string DocumentTableConnection { get; set; }
        public static string DocumentGenerationAPIBaseURL { get; set; }
        public static string ServiceBusQueName { get; set; }
        public static string SBfullyQualifiedNamespace { get; set; }
        public static string RequestStorageContainer { get; set; }
        public static string EncriptionKey { get; set; }
        public static string AzureStorageConnection { get; set; }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace US.BOX.BoxAPI.Data.Models
{
    public static class DPAPIConfig
    {
        public static string ClientID { get; set; }
        public static string ConnectionString {  get; set; }
        public static string CatalogDBConnectionString { get; set; }
        public static string AllowedHosts { get; set; } = string.Empty;    
        public static string AuthSecret { get; set; }
        public static string AuthValidIssuer { get; set; }
        public static string AuthValidAudience { get; set; }
        public static string EncryptionKey { get; set; }
        public static string GenerateGDPRActivityCode { get; set; }
        public static string InvoiceActivityCode { get; set; }
        public static string AzureBlobStorageConnectionString { get; set; }
        public static string AzureBlobStorageContainerName { get; set; }
        public static string AzureBlobStorageAccountName { get; set; }
        public static string CPReciptTemplate {  get; set; }
        public static string ReciptFolderpath { get; set; }
        public static string TenentID { get; set; }

    }
}

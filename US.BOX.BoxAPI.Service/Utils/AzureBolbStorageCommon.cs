using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using Azure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using US.BOX.BoxAPI.Data.Models;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace US.BOX.BoxAPI.Service.Utils
{
    public class AzureBolbStorageCommon
    {
        public static async Task<string> UploadFileAsync(IFormFile file, string storagename, string containerName, string folderPath, string filePath, string clientId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(storagename))
                    throw new ArgumentException("Storage name cannot be null or empty.", nameof(storagename));

                if (string.IsNullOrWhiteSpace(containerName))
                    throw new ArgumentException("Container name cannot be null or empty.", nameof(containerName));

                if (string.IsNullOrWhiteSpace(folderPath))
                    throw new ArgumentException("Folder path cannot be null or empty.", nameof(folderPath));

                storagename = storagename.Trim();
                containerName = containerName.Trim();
                folderPath = folderPath.Trim().TrimEnd('/');

                string containerEndPoint = string.Format("https://{0}.blob.core.windows.net/{1}", storagename, containerName);
                BlobContainerClient containerClient = new BlobContainerClient(
                    new Uri(containerEndPoint),
                    new DefaultAzureCredential(new DefaultAzureCredentialOptions { ManagedIdentityClientId = clientId })
                );

                await containerClient.CreateIfNotExistsAsync(PublicAccessType.None);

                string fileName = Path.GetFileName(filePath);
                string blobName = $"{folderPath}/{fileName}"; // Include folder structure in blob name

                BlobClient blobClient = containerClient.GetBlobClient(blobName);

                using (var stream = file.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream, overwrite: true);
                }

                return blobClient.Uri.ToString();
            }
            catch (Exception)
            {
                //throw;
                return "";
            }
        }

        //public static async Task<bool> DeleteFileAsync(string fileName, string containerName,string storagename,string folderName, string clientId)
        //{
        //    try
        //    {
        //        string containerEndPoint = string.Format("https://{0}.blob.core.windows.net/{1}", storagename, folderName);
        //        var blobServiceClient = new BlobContainerClient(new Uri(containerEndPoint),
        //                                                                          new DefaultAzureCredential(new DefaultAzureCredentialOptions { ManagedIdentityClientId = clientId }));

        //        var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

        //        var blobClient = containerClient.GetBlobClient(fileName);

        //        var response = await blobClient.DeleteIfExistsAsync();

        //        return response.Value; // Returns true if the file was deleted, false if not found
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception($"File deletion failed: {ex.Message}");
        //    }
        //}

        public static async Task<byte[]> DownloadFileAsync(string fileName, string containerName, string clientId)
        {
            try
            {
                // Create a BlobServiceClient object which will be used to create a container client
                string blobAccountName = DPAPIConfig.AzureBlobStorageAccountName;
                string accountEndPoint = string.Format("https://{0}.blob.core.windows.net/", blobAccountName);

                BlobServiceClient blobServiceClient = new BlobServiceClient(new Uri(accountEndPoint), new DefaultAzureCredential(new DefaultAzureCredentialOptions { ManagedIdentityClientId = clientId }));
                ///https://strgunicornboxdev.blob.core.windows.net/dpuploadclaims/ClaimOfPaymentReceipts/204/20250224T232245/CP_1212.html
                // Create the container and return a container client object
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

                // Get a reference to a blob
                BlobClient blobClient = containerClient.GetBlobClient(fileName);

                if (!await blobClient.ExistsAsync())
                    throw new FileNotFoundException($"File '{fileName}' not found in Blob Storage.");

                BlobDownloadInfo download = await blobClient.DownloadAsync();

                using (MemoryStream ms = new MemoryStream())
                {
                    await download.Content.CopyToAsync(ms);
                    return ms.ToArray(); // Return the file as a byte array
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"File download failed: {ex.Message}");
            }
        }

        public static async Task<string> DownloadFromBlobStorage(string fileName, string containerName, string clientId)
        {
            try
            {
                // Create a BlobServiceClient object which will be used to create a container client
                string blobAccountName = DPAPIConfig.AzureBlobStorageAccountName;
                string accountEndPoint = string.Format("https://{0}.blob.core.windows.net/", blobAccountName);
            //https://strgunicornboxdev.blob.core.windows.net/dpuploadclaims/CPRecipt.html
                BlobServiceClient blobServiceClient = new BlobServiceClient(new Uri(accountEndPoint), new DefaultAzureCredential(new DefaultAzureCredentialOptions { ManagedIdentityClientId = clientId }));
                // Create the container and return a container client object
                
                
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

                // Get a reference to a blob
                BlobClient blobClient = containerClient.GetBlobClient(fileName);

                // Download the blob's contents and save it to a file
                BlobDownloadInfo download = blobClient.Download();

                using (var streamReader = new StreamReader(download.Content))
                {
                    while (!streamReader.EndOfStream)
                    {
                        var file = streamReader.ReadToEnd();
                        return file;
                    }

                    return string.Empty;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static async Task<bool> FileExistsAsync(string containerName, string blobName)
        {
            try
            {
                var credential = new DefaultAzureCredential();
                var blobServiceClient = new BlobServiceClient(new Uri("https://<your-storage-account-name>.blob.core.windows.net"), credential);
                var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

                var blobClient = containerClient.GetBlobClient(blobName);

                return await blobClient.ExistsAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }
    }
}

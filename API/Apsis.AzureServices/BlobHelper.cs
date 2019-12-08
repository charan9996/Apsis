using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Apsis.AzureServices
{
    public class BlobHelper : IBlobHelper
    {
        readonly CloudBlobClient cloudBlobClient;

        public BlobHelper(IConfiguration configuration)
        {
            // Retrieve the connection string forAzure storage account
            string storageConnectionString = configuration["ConnectionStrings:StorageConnectionString"];
            CloudStorageAccount.TryParse(storageConnectionString, out CloudStorageAccount storageAccount);

            // Create the CloudBlobClient that represents the Blob storage endpoint for the storage account.
            cloudBlobClient = storageAccount.CreateCloudBlobClient();
        }

        /// <summary>
        /// Uploads a file to blob storage.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task<string> UploadFileToBlobAsync(string blobContainerName, string fileName, Stream fileStream)
        {
            string fileUrl = "";
            try
            {
                // Create a container.
                var blobContainer = cloudBlobClient.GetContainerReference(blobContainerName);
                await blobContainer.CreateIfNotExistsAsync();

                // Set the permissions so the blobs are public.
                var permissions = new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob };
                await blobContainer.SetPermissionsAsync(permissions);

                // Get a reference to the blob address, then upload the file to the blob.
                var cloudBlockBlob = blobContainer.GetBlockBlobReference(fileName);
                await cloudBlockBlob.UploadFromStreamAsync(fileStream);
                fileUrl = cloudBlockBlob.Uri.ToString();

            }
            catch (StorageException ex)
            {
                // Log
            }
            return fileUrl;
        }

        /// <summary>
        /// Download file from blob
        /// </summary>
        /// <param name="blobContainerName"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task<MemoryStream> DownloadBlobAsync(string blobContainerName, string fileName)
        {
            var memoryStream = new MemoryStream();
            try
            {
                var blobContainer = cloudBlobClient.GetContainerReference(blobContainerName);
                await blobContainer.CreateIfNotExistsAsync();
                var blob = blobContainer.GetBlockBlobReference(fileName);
                //bool blobExists = await blob.ExistsAsync();
                bool blobExists = await blobContainer.GetBlobReference(fileName).ExistsAsync();
                // Download blob's content to a stream
                if (blobExists) await blob.DownloadToStreamAsync(memoryStream);
                else memoryStream = null;

            }
            catch (StorageException ex)
            {
                // Log
                
            }

            return memoryStream;
        }

        public async Task<string> GetBlobUriAsync(string blobContainerName, string fileName)
        {
            var fileBlobUri = "";
            try
            {
                var blobContainer = cloudBlobClient.GetContainerReference(blobContainerName);
                await blobContainer.CreateIfNotExistsAsync();
                var blob = blobContainer.GetBlockBlobReference(fileName);
                bool blobExists = await blobContainer.GetBlobReference(fileName).ExistsAsync();
                // Download blob's content to a stream
                if (blobExists)
                {
                    fileBlobUri = blob.SnapshotQualifiedStorageUri.PrimaryUri.AbsoluteUri;
                }
                else fileBlobUri = null;

            }
            catch (StorageException ex)
            {
                // Log

            }

            return fileBlobUri;
        }
        /// <summary>
        /// Checks the existance of blob
        /// </summary>
        /// <param name="blobContainerName"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task<bool> DoesBlobExistsAsync(string blobContainerName, string fileName)
        {
            try
            {
                var blobContainer = cloudBlobClient.GetContainerReference(blobContainerName);
                bool reponse = await blobContainer.GetBlobReference(fileName).ExistsAsync();
                if (reponse)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                //Log+		blobContainer	{Microsoft.WindowsAzure.Storage.Blob.CloudBlobContainer}	Microsoft.WindowsAzure.Storage.Blob.CloudBlobContainer

                return false;
            }
        }

        /// <summary>
        /// Delete the blob from the  blobContainer
        /// </summary>
        /// <param name="blobContainerName"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task<bool> DeleteBlobAsync(string blobContainerName, string fileName)
        {
            bool result = false;
            try
            {
                var blobContainer = cloudBlobClient.GetContainerReference(blobContainerName);
                await blobContainer.CreateIfNotExistsAsync();
                var blob = blobContainer.GetBlockBlobReference(fileName);
                // Download blob's content to a stream
                result = await blob.DeleteIfExistsAsync();
            }
            catch (StorageException ex)
            {
                // Log
            }
            return result;
        }
    }
}

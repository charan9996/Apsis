using System.IO;
using System.Threading.Tasks;

namespace Apsis.AzureServices
{
    /// <summary>
    /// Blob interface which contains methods to implement in the BlobHelper
    /// </summary>
    public interface IBlobHelper
    {
        Task<string> UploadFileToBlobAsync(string blobContainerName, string fileName, Stream fileStream);
        Task<MemoryStream> DownloadBlobAsync(string blobContainerName, string fileName);
        Task<string> GetBlobUriAsync(string blobContainerName, string fileName);
        Task<bool> DoesBlobExistsAsync(string blobContainerName, string fileName);
        Task<bool> DeleteBlobAsync(string blobContainerName, string fileName);
    }
}

using Apsis.Abstractions.Business;
using Apsis.Abstractions.Repository;
using Apsis.AzureServices;
using Apsis.Models.Entities;
using Apsis.Models.Response;
using Apsis.Models.ViewModel;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Apsis.Business
{
   public  class UploadHelper:IUploadHelper
    {
        readonly IUploadRepository _uploadRepository;
        readonly IBlobHelper _blobHelper;
        /// <summary>
        /// Constructer
        /// </summary>
        /// <param name="uploadRepository"></param>
        /// <param name="blobHelper"></param>
        public UploadHelper(IUploadRepository uploadRepository, IRepository dataRepository, IBlobHelper blobHelper)
        {
            _uploadRepository=uploadRepository;
            _blobHelper =blobHelper; 
        }
        /// <summary>
        /// Upload the Assignment into Upload Respository
        /// </summary>
        /// <param name="file"></param>
        /// <param name="courseId"></param>
        /// <returns></returns>
        public async Task<Upload> UploadFileAsync(IFormFile file, string fileName)
        {
            Upload upload;
            try
            {
                using (Stream stream = new MemoryStream())
                {
                    string filePath = "";
                    file.OpenReadStream();
                    await file.CopyToAsync(stream);
                    stream.Seek(0, SeekOrigin.Begin);
                    bool doesBlockExitsAsync = await _blobHelper.DoesBlobExistsAsync("assignments", fileName);
                    if (doesBlockExitsAsync)
                    {
                        bool deleteBlobAsync = await _blobHelper.DeleteBlobAsync("assignments", fileName);
                        if (deleteBlobAsync)
                        {
                            filePath = await _blobHelper.UploadFileToBlobAsync("assignments", fileName, stream);
                        }
                    }
                    else
                    {
                        filePath = await _blobHelper.UploadFileToBlobAsync("assignments", fileName, stream);
                    }
                    upload = new Upload { Id = Guid.NewGuid(), FileName = fileName, FilePath = filePath };
                    await _uploadRepository.UploadFileAsync(upload);
                }
            }
            catch (Exception ex)
            {
                Logging.Logger.LogException(ex);
                return null ;
            }
            return upload;
        }        
    }
}

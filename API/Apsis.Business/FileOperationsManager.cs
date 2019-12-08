using Apsis.Abstractions.Business;
using Apsis.AzureServices;
using Apsis.Models.Entities;
using Apsis.Models.Response;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Net.Http.Headers;
using System.IO.Compression;
using Microsoft.AspNetCore.Http;

namespace Apsis.Business
{
    public class FileOperationsManager : IFileOperationsManager
    {
        readonly IBlobHelper _blobHelper;
        readonly IHostingEnvironment _hostingEnvironment;
        public FileOperationsManager(IBlobHelper blobHelper, IHostingEnvironment hostingEnvironment)
        {
            _blobHelper = blobHelper;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<FileDownloadResponse> DownloadFile(IEnumerable<Upload> uploads)
        {
            try
            {
                var fileDownloadResponse = new FileDownloadResponse();
                var fileDetails = uploads.FirstOrDefault();
                var blobFileUrl = await _blobHelper.GetBlobUriAsync("assignments", fileDetails.FileName.Trim());
                if(blobFileUrl != null)
                {
                    fileDownloadResponse.IsSuccess = true;
                    fileDownloadResponse.Message = "File Downloaded Successfully";
                    fileDownloadResponse.FileUrl = blobFileUrl;
                }
                else
                {
                    fileDownloadResponse = new FileDownloadResponse { Message = "No File Available to Download." };
                }
                return fileDownloadResponse;
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public async Task<FileDownloadResponse> DownloadZipFile(IEnumerable<Upload> uploads)
        {
            try
            {
                var fileDownloadResponse = new FileDownloadResponse();
                string currentFormattedTime = DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss");
                string inputZipFileName = "AssignmentSolution_" + currentFormattedTime;
                string inputZipFileNameWithExtension = inputZipFileName + ".zip";
                string rootPath = _hostingEnvironment.WebRootPath;
                string fullPath = Path.Combine(rootPath, inputZipFileName);
                string zipPath = Path.Combine(rootPath, inputZipFileNameWithExtension);
                if (!Directory.Exists(fullPath))
                {
                    Directory.CreateDirectory(fullPath);
                }

                int count = 0;

                foreach (var file in uploads)
                {
                    var blobFileStream = await _blobHelper.DownloadBlobAsync("assignments", file.FileName.Trim());
                    if (blobFileStream != null)
                    {
                        string localFilePath = Path.Combine(fullPath, file.FileName);
                        using (var localFileStream = new FileStream(localFilePath, FileMode.OpenOrCreate))
                        {
                            blobFileStream.Position = 0;
                            blobFileStream.CopyTo(localFileStream);
                        }
                        count++;
                    }
                }
                if(count > 0)
                {
                    ZipFile.CreateFromDirectory(fullPath, zipPath);
                    var fileStream = File.OpenRead(zipPath);
                    var blobFileUrl = await _blobHelper.UploadFileToBlobAsync("zipped-files", inputZipFileNameWithExtension,fileStream);
                    if(blobFileUrl != null)
                    {
                        fileDownloadResponse.IsSuccess = true;
                        fileDownloadResponse.Message = "File Downloaded Successfully";
                        fileDownloadResponse.FileUrl = blobFileUrl;
                    }
                    else
                    {
                        fileDownloadResponse = new FileDownloadResponse { Message = "No File Available to Download." };
                    }
                }
                else
                {
                    fileDownloadResponse = new FileDownloadResponse { Message = "No Files Available to Download." };
                }
                Directory.Delete(fullPath, true);
                return fileDownloadResponse;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task<bool> IsZipFileEmpty(IFormFile file)
        {
            string folderName = "Upload";
            string webRootPath = _hostingEnvironment.WebRootPath;
            string newPath = Path.Combine(webRootPath, folderName);
            if (file.Length > 0)
            {
                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }

                string fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Value.Trim();
                string fullPath = Path.Combine(newPath, fileName);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                using (ZipArchive za = ZipFile.OpenRead(fullPath))
                {
                    if (za.Entries.Count == 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
            }
            return false;
        }

    }
}

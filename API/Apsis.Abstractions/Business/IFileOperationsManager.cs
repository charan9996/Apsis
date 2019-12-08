using Apsis.Models.Entities;
using Apsis.Models.Response;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Apsis.Abstractions.Business
{
    public interface IFileOperationsManager
    {
        Task<FileDownloadResponse> DownloadZipFile(IEnumerable<Upload> uploads);

        Task<FileDownloadResponse> DownloadFile(IEnumerable<Upload> uploads);
        Task<bool> IsZipFileEmpty(IFormFile file);
    }
}

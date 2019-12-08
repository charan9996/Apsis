using Apsis.Models.Entities;
using Apsis.Models.Response;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Apsis.Abstractions.Business
{
    public interface IUploadHelper
    {
        /// <summary>
        /// Uploads file to blob and adds it to Upload table
        /// </summary>
        /// <param name="upload"></param>
        /// <returns></returns>
        Task<Upload> UploadFileAsync(IFormFile file, string fileName);
    }
}

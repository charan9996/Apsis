using Apsis.Models.Entities;
using Apsis.Models.Response;
using Apsis.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Apsis.Models.Entities;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Apsis.Abstractions.Business
{
    public interface IUploadManager
    {
        /// <summary>
        /// To process yorbit input file coming to Apsis
        /// </summary>
        /// <param name="inputFile"></param>
        /// <returns></returns>
        Task<Response> ProcessYorbitInput(IFormFile inputFile);
        Task<Response> ProcessYorbitCourseInput(IFormFile inputFile);
    }
}
using Apsis.Abstractions.Business;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Apsis.Models.Authorization;

namespace Apsis.API.Controllers
{
    [Produces("application/json")]
    [Route("api/Upload")]
    public class UploadController : Controller
    {
        readonly IUploadManager _uploadManager;
        public UploadController(IUploadManager uploadManager)
        {
            _uploadManager = uploadManager;
        }

        /// <summary>
        /// To Upload the File Containing the Yorbit Data
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("yorbit-input")]
        [Authorize(Policy = "LearningOPMOnly")]
        public async Task<IActionResult> PostRequestInputFile()
        {
               var form = await Request.ReadFormAsync();
                if (form.Files == null)
                {
                    return BadRequest("No File Found");
                }
                var inputFile = form.Files[0];
                if (!(inputFile.FileName.EndsWith(".xls", StringComparison.OrdinalIgnoreCase)
                   || inputFile.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase)))
                {
                    return BadRequest("Input is not an Excel file");
                }
                var response = await _uploadManager.ProcessYorbitInput(inputFile);
                return Ok(response);
          
        }
        /// <summary>
        /// To Upload the File Containing the Yorbit Course Data
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("yorbitCourse-input")]
        [Authorize(Policy = "LearningOPMOnly")]
        public async Task<IActionResult> PostCourseInputFile()
        {
           
                var form = await Request.ReadFormAsync();
                if (form.Files == null)
                {
                    return BadRequest("No File Found");
                }
                var inputFile = form.Files[0];
                if (!(inputFile.FileName.EndsWith(".xls", StringComparison.OrdinalIgnoreCase)
                   || inputFile.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase)))
                {
                    return BadRequest("Input is not an Excel file");
                }
                var response = await _uploadManager.ProcessYorbitCourseInput(inputFile);
                return Ok(response);
        }

    }
}

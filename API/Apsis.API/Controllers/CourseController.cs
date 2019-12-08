using Apsis.Abstractions.Business;
using Apsis.Models.Common;
using Apsis.Models.Entities;
using Apsis.Models.Response;
using Apsis.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Apsis.API.Controllers
{
    /// <summary>
    /// Course Controller
    /// </summary>
    [Produces("application/json")]
    [Route("api/Course")]
    public class CourseController : Controller
    {
        readonly ICourseManager _courseManager;
        readonly IFileOperationsManager _fileOperationsManager;
        /// <summary>
        /// Constructer
        /// </summary>
        /// <param name="courseManager"></param>
        public CourseController(ICourseManager courseManager, IFileOperationsManager fileOperationsManager)
        {
            _courseManager = courseManager;
            _fileOperationsManager = fileOperationsManager;
        }

        /// <summary>
        /// To Upload Problem Statement by learning OPM
        /// </summary>
        /// <param name="id">Course Id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("{id}/assignment")]
        [Authorize(Policy = "LearningOPMOnly")]
        public async Task<IActionResult> PostAssignment(Guid id)
        {
            var response = new FileDownloadResponse();
            var file = HttpContext.Request.Form.Files.FirstOrDefault();
            if (id == Guid.Empty)
            {
                return BadRequest(new Response { IsSuccess = false, Message = "Invalid Input Type" });
            }
            if (file == null)
            {
                response.IsSuccess = false;
                response.Message = "No File Found";
                return StatusCode(200, response);
            }

           
            if (file.Length < 1024)
            {
                bool isZipCheck = await _fileOperationsManager.IsZipFileEmpty(file);
                if (isZipCheck)
                {
                    response.IsSuccess = false;
                    response.Message = "No Files are found inside the Zip File.";
                    return StatusCode(200, response);
                }
            }
            if(file.Length >104857600)
            {
                response.IsSuccess = false;
                response.Message = "File of the zip file is too large.";
                return StatusCode(200, response);
            }
            if (file.FileName.EndsWith(".zip"))
            {
                    response = await _courseManager.UpdateAssigmentAsync(id, file);
                    if (response.IsSuccess)
                        return StatusCode(201, response);
                    else
                        return StatusCode(200, response);
            }
            else
            {
                    return BadRequest(new Response { IsSuccess = false, Message = "Invalid file Format" });
            }

        } 

        /// <summary>
        /// Return Courses based on keyword
        /// </summary>
        /// <param name="courseFilter"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Policy = "LearningOPMOnly")]
        public async Task<IActionResult> GetCoursesFiltered([FromBody]CourseFilter courseFilter)
        {
            if (courseFilter != null && ModelState.IsValid)
            {
                var courses = await _courseManager.GetAllCoursesAsync(courseFilter);
                if (courses.Count() > 0)
                {
                    return Ok(courses);
                }
                return NoContent();
            }
            return BadRequest("Invalid Parameters");

        }

        /// <summary>
        /// To Return the Evaluators and Course Details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}/evaluator")]
        [HttpGet]
        [Authorize(Policy = "LearningOPMOnly")]
        public async Task<IActionResult> GetEvaluators(Guid id)
        {
            CourseModel courseModel = new CourseModel();
            if (id.Equals(Guid.Empty))
            {
                courseModel.Message = "Course ID not found.";
                return new BadRequestObjectResult(courseModel);
            }
            var courseEvaluatorDetails = await _courseManager.GetCourseEvaluatorDetails(id);
            if (courseEvaluatorDetails.IsSuccess == true)
            {
                return Ok(courseEvaluatorDetails);
            }
            else
            {
                return StatusCode(500, courseModel);
            }
        }

        /// <summary>
        /// To Add the Evaluator for a course
        /// </summary>
        /// <param name="id">Course Id</param>
        /// <param name="mid">Evaluator Mid</param>
        /// <returns></returns>
        [Route("{id}/evaluator/{mid}")]
        [HttpPut]
        [Authorize(Policy = "LearningOPMOnly")]
        public async Task<IActionResult> AddEvaluator(Guid id, string mid)
        {
            User user = new User();
            if ((id.Equals(Guid.Empty)) && (string.IsNullOrEmpty(mid)))
            {
                return StatusCode(404, "CourseId and Mid not found.");
            }
            user = await _courseManager.AddEvaluator(id, mid);
            if (user != null)
            {
                return Ok(user);
            }
            else
            {
                return StatusCode(500);
            }
        }

        /// <summary>
        /// To delete the evaluator for a course
        /// </summary>
        /// <param name="id">Course Id</param>
        /// <param name="evaluatorId">Evaluator Id</param>
        /// <returns></returns>
        [Route("{id}/evaluator/{evaluatorId}")]
        [HttpDelete]
        [Authorize(Policy = "LearningOPMOnly")]
        public async Task<IActionResult> DeleteEvaluator(Guid id, Guid evaluatorId)
        {
            CourseModel courseModel = new CourseModel();
            if ((id.Equals(Guid.Empty)) && (evaluatorId.Equals(Guid.Empty)))
            {
                courseModel.Message = "CourseId and evaluatorId not found.";
                return new BadRequestObjectResult(courseModel);
            }
            courseModel = await _courseManager.DeleteEvaluator(id, evaluatorId);
            return Ok(courseModel);
        }
    }
}

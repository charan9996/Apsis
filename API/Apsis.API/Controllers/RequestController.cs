using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apsis.Abstractions.Business;
using Microsoft.AspNetCore.Mvc;
using Apsis.Models.Common;
using Apsis.Models.Response;
using System.IO.Compression;
using Apsis.Models.ViewModel;
using Apsis.Models.Entities;
using Apsis.Models.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Authorization;
using Apsis.Abstractions;
using Apsis.Models.Authorization;

namespace Apsis.API.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Produces("application/json")]
    [Route("api/Request")]
    public class RequestController : Controller
    {
        readonly IRequestManager _requestManager;
        readonly IFileOperationsManager _fileOperationsManager;
        readonly ApplicationContext _context;        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestManager"></param>
        /// <param name="uploadManager"></param>
        /// <param name="fileOperationsManager"></param>
        public RequestController(IContextProvider contextProvider, IRequestManager requestManager, IFileOperationsManager fileOperationsManager)
        {
            _context = contextProvider.Context;
            _requestManager = requestManager;
            _fileOperationsManager = fileOperationsManager;
        }


        /// <summary>
        /// 
        /// </summary>Returns the desired Learner details 
        /// <returns></returns>
        [HttpGet]
        [Route("my-requests")]
        [Authorize(Policy = "Learner")]
        public async Task<IActionResult> Get()
        {

            // My requests page is not accessible for external users
            if (_context.CurrentUser.IsExternal) return Unauthorized();
            return Ok(await _requestManager.GetLearnerRequests());

        }

        /// <summary>
        /// To Return All the Requests based on a filter
        /// </summary>
        /// <returns>List of request views</returns>
        [HttpPost]
        [Authorize(Policy = "EvaluatorsAndLearningOPM")]
        public async Task<IActionResult> GetRequestsFiltered([FromBody]RequestSearchFilter requestSearchFilter , bool IsExport)
        {
            //IsExport = true;
            if (requestSearchFilter == null || !ModelState.IsValid)
                return BadRequest("Invalid parameters");

            var response = await _requestManager.ListAllRequests(requestSearchFilter);

            if (IsExport)
            {
               var ExcelReportStream = await _requestManager.ListRequestDetailsForReport(requestSearchFilter);  
                try
                {
                    FileStreamResult fr = File(ExcelReportStream, "application/vnd.openxmlformats");
                    return fr;
                }
                catch(Exception e)
                {
                    return NotFound();
                }
            }
            if (response.IsSuccess)
                return Ok(response.requestViews);
            return NoContent();
        }
        /// <summary>
        /// To Upload the Score Card against the Request
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("scorecard")]
        public IActionResult PostScoreCard([FromBody]string value)
        {
            return NotFound();
        }

        /// <summary>
        /// To Upload the Desired Assignment/Solution/Project against the Request
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("assignment")]
        public IActionResult PostAssignment([FromBody]string value)
        {
            return NotFound();
        }

        /// <summary>
        /// Download assignment solutions as a zip file for given requests
        /// </summary>
        /// <param name="requestIds">RequestIds</param>
        /// <returns></returns>
        [HttpPost]
        [Route("assignment-solution")]
        [Authorize(Policy = "EvaluatorsAndLearningOPM")]
        public async Task<IActionResult> DownloadAssignmentSolution([FromBody]IEnumerable<Guid> requestIds)
        {
            var response = new Response();
            if (requestIds == null || !requestIds.Any())
            {
                response.Message = "RequestIds not found.";
                return new BadRequestObjectResult(response);
            }
            try
            {
                var fileDownloadResponse = await _requestManager.DownloadAssignment(requestIds);
                return Ok(fileDownloadResponse);
            }
            catch (Exception ex)
            {
                Logging.Logger.LogException(ex);
                response.Message = "Error downloading file.";
                return StatusCode(500, response);
            }
        }

        /// <summary>
        /// To Upload the Error Details against the Request
        /// </summary>
        /// <param name="id"></param>
        /// <param name="errorModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("{id}/error")]
        [Authorize(Policy = "EvaluatorsAndLearningOPM")]
        public async Task<IActionResult> PostErrorAsync([FromRoute]Guid id, [FromForm]ErrorModel errorModel)
        {

            if (errorModel != null)
            {
                try
                {

                    if (_context.CurrentUser.Mid != errorModel.Mid)
                    {
                        var comment = errorModel.Comment.Trim();
                        var file = errorModel.File;
                        if (id == Guid.Empty)
                        {
                            return BadRequest(new Response { IsSuccess = false, Message = "Invalid Id" });
                        }
                        if (file == null || !file.FileName.EndsWith(".zip"))
                        {
                            return BadRequest(new Response { IsSuccess = false, Message = "Invalid File" });
                        }
                        if (file.Length < 25)
                        {
                            bool isZipCheck = await _fileOperationsManager.IsZipFileEmpty(file);
                            if (isZipCheck)
                            {
                                Response responseInner = new Response();
                                responseInner.IsSuccess = false;
                                responseInner.Message = "No Files found inside the Zip File.";
                                return StatusCode(200, responseInner);
                            }
                        }
                        var response = await _requestManager.UploadErrorAsync(file, comment, id);
                        if (response.IsSuccess)
                        {
                            return StatusCode(200, response);
                        }


                    }
                    return Unauthorized();
                }
                catch (Exception)
                {

                    return BadRequest(new Response { IsSuccess = false, Message = "Error File Upload" });
                }
            }
            return BadRequest(new Response { IsSuccess = false, Message = "Empty fields" });

        }

        /// <summary>
        /// To Upload the Result Details against the Request
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("result")]
        public IActionResult PostResult([FromBody]string value)
        {
            return NotFound();
        }

        /// <summary>
        /// To Update the Resubmission Date of the Assignment/Project/Solution against the Request
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="newAssignmentDueDate"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{requestId}/resubmission-date")]
        [Authorize(Policy = "LearningOPMOnly")]
        public async Task<IActionResult> PutResubmissionDate(Guid requestId, [FromBody]DateTime newAssignmentDueDate)
        {
            Response response = new Response();
            if (requestId == Guid.Empty || newAssignmentDueDate == null || newAssignmentDueDate.Date < DateTime.Now.Date)
            {
                return BadRequest();
            }
            else
            {
                try
                {
                    response = await _requestManager.UpdateAssignmentDueDate(requestId, newAssignmentDueDate);
                    return StatusCode(200, response);
                }
                catch (Exception ex)
                {
                    Logging.Logger.LogException(ex);
                    response.IsSuccess = false;
                    response.Message = "Error Occured... Try Again";
                    return StatusCode(200, response);
                }
            }
        }
        /// <summary>
        /// Uploads assignment solution for a request
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("{id}/assignment-solution")]
        [Authorize(Policy = "Learner")]
        public async Task<IActionResult> PostAssignmentSolution(Guid id)
        {
            Response response = new Response();
            var file = Request.Form.Files[0];
            if (id == Guid.Empty)
            {
                return StatusCode(200, new Response { IsSuccess = false, Message = "Invalid Id" });
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
            if (file.Length > 104857600)
            {
                response.IsSuccess = false;
                response.Message = "File of the zip file is too large.";
                return StatusCode(200, response);
            }
            if (!file.FileName.EndsWith(".zip"))
            {
                return StatusCode(200, new Response { IsSuccess = false, Message = "Invalid File" });
            }
            try
            {
                response = await _requestManager.UploadAssignmentSolutionAsync(file, id);
                return StatusCode(200, response);
            }
            catch(Exception ex)
            {
                Logging.Logger.LogException(ex);
                response.IsSuccess = false;
                response.Message = "Error in uploading assignment solution";
                return StatusCode(400, response.Message);
            }
            
        }

        /// <summary>
        /// To Get all the Request Details
        /// </summary>
        /// <param name="requestId">Request Id (Guid)</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{requestId}")]
        [Authorize(Policy = "Learner")]
        public async Task<IActionResult> GetRequestDetails(Guid requestId)
        {
            RequestDetailsModel requestDetailsModel = new RequestDetailsModel();
            if (requestId.Equals(Guid.Empty))
            {
                requestDetailsModel.Message = "Request ID not found.";
                return new BadRequestObjectResult(requestDetailsModel);
            }
            try
            {
                var requestDetails = await _requestManager.GetRequestDetails(requestId);
                if (requestDetails.IsSuccess == true)
                {
                    return Ok(requestDetails);
                }
                else
                {
                    return StatusCode(500, requestDetails);
                }
            }
            catch (Exception ex)
            {
                Logging.Logger.LogException(ex);
                requestDetailsModel.Message = "Error Getting Evaluators List";
                return StatusCode(500, requestDetailsModel.Message);
            }
        }
        /// <summary>
        /// To Publish Result By Learning OPM
        /// </summary>
        /// <param name="requestIds"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("result-publish")]
        [Authorize(Policy = "LearningOPMOnly")]
        public async Task<IActionResult> PublishResult([FromBody]List<Guid> requestIds)
        {
            Response response = new Response();
            try
            {
                if ((requestIds == null) || (!requestIds.Any()))
                {
                    response.IsSuccess = false;
                    response.Message = "RequestIds Not Found";
                    return StatusCode(400, response);
                }
                else
                {
                    response = await _requestManager.PublishResult(requestIds);
                    return StatusCode(200, response);
                }
            }
            catch (Exception ex)
            {
                Logging.Logger.LogException(ex);
                response.IsSuccess = false;
                response.Message = "Error occured in publishing result";
                return StatusCode(200, response);
            }

        }

        /// <summary>
        /// Used to update the evaluated result
        /// </summary>
        /// <param name="id">Request Id</param>
        /// <param name="evaluation">Evaluation Model</param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id}/evaluation")]
        [Authorize(Policy = "EvaluatorsAndLearningOPM")]
        public async Task<IActionResult> UpdateEvaluationResult(Guid id,[FromForm]Evaluation evaluation)
        {
            Response response = new Response();
            if (_context.CurrentUser.Mid != evaluation.Mid)
            {
                if (id.Equals(Guid.Empty)
                    || evaluation == null
                    || !(evaluation.ResultId.Equals(RequestResultCodes.Project_Cleared) || evaluation.ResultId.Equals(RequestResultCodes.Project_Not_Cleared))
                    || !CommonHelper.IsValidScore(evaluation.Score)
                    || (evaluation.ScoreCardFile != null && !evaluation.ScoreCardFile.FileName.EndsWith(".zip")))
                {
                    return new BadRequestObjectResult(response);
                }

            if (!string.IsNullOrEmpty(evaluation.Comments) && evaluation.Comments.ToString().Length > 500)
            {
                response.Message = "Invalid Data.";
                return new BadRequestObjectResult(response);
            }

            //To check file if it is empty inside
            if (evaluation.ScoreCardFile != null && evaluation.ScoreCardFile.Length < 25)
            {
                bool isZipCheck = await _fileOperationsManager.IsZipFileEmpty(evaluation.ScoreCardFile);
                if (isZipCheck)
                {
                    response.IsSuccess = false;
                    response.Message = "No Files found inside the Zip File.";
                    return StatusCode(200, response);
                }
            }

            try
            {
                response = await _requestManager.UpdateEvaluationResult(id, evaluation);
               if(response.Message != null && response.Message.Equals("Not For Evaluation"))
                    {
                        return Unauthorized();
                    }
                    else
                    {
                        return Ok(response); 
                    }
            }   
            catch (Exception ex)
            {
                Logging.Logger.LogException(ex);
                response.Message = "Error in Doing Evaluation";
                return StatusCode(500, response);
            }
            }

            else
            {
                return Unauthorized();
            }

        }

        /// <summary>
        /// Used to update the Download Date by Evaluator
        /// </summary>
        /// <param name="requestIds">Request Id</param>
        /// <returns></returns>
        [HttpPut]
        [Route("DownloadDate")]
        public async Task<IActionResult> UpdateAssignmentDownloadDate([FromBody]IEnumerable<Guid> requestIds)
        {
            Response response = new Response();
            if (requestIds == null || !requestIds.Any())
            {
                return new BadRequestObjectResult(response);
            }
            try
            {
                response = await _requestManager.UpdateAssignmentDownloadDate(requestIds);
                return Ok(response);
            }
            catch(Exception ex)
            {
                Logging.Logger.LogException(ex);
                return StatusCode(500, response);
            }
        }
    }
}

using Apsis.Models;
using Apsis.Models.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Apsis.Models.Common;
using Apsis.Models.ViewModel;
using Microsoft.AspNetCore.Http;
using Apsis.Models.Entities;
using System.IO;

namespace Apsis.Abstractions.Business
{
    public interface IRequestManager
    {
        Task<List<RequestModel>> GetLearnerRequests();

        /// <summary>
        /// To Download The Assignment-Solution
        /// </summary>
        /// <param name="requestId">List of Request Ids for which downloading of assignment-solution to perform</param>
        /// <returns></returns>
        Task<FileDownloadResponse> DownloadAssignment(IEnumerable<Guid> requestIds);

        /// <summary>
        /// This method gets list of all requests based on the filters specified. Can be sorting, keyword etc.
        /// </summary>
        /// <param name="requestSearchFilter">Filters to be applied for the listing of requests.</param>
        /// <returns>A response with IEnumerable<RequestView> having list of views</returns>
        Task<RequestListResponse> ListAllRequests(RequestSearchFilter requestSearchFilter);
        /// <summary>
        /// Add Yorbit Input list to Request table
        /// </summary>
        /// <param name="requests"></param>
        /// <returns></returns>
        Task<Response> AddRequests(List<RequestModel> requests);
        /// <summary>
        /// Method to update AssignmentDueDate by Learning OPM
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="newAssignmentDueDate"></param>
        /// <returns></returns>
        Task<Response> UpdateAssignmentDueDate(Guid requestId, DateTime newAssignmentDueDate);

        /// <summary>
        /// This method uploads the assignment solution by learner
        /// </summary>
        /// <param name="file, requestId">File and RequestId</param>
        /// <returns>A response with IEnumerable<RequestView> having list of views</returns>
        Task<Response> UploadAssignmentSolutionAsync(IFormFile file, Guid requestId);

        /// <summary>
        /// Used to get all the request details
        /// </summary>
        /// <param name="requestId">Request ID (GUID)</param>
        /// <returns></returns>
        Task<RequestDetailsModel> GetRequestDetails(Guid requestId);
        /// <summary>
        /// Method to publish result by OPM
        /// </summary>
        /// <param name="requestIds"></param>
        /// <returns></returns>
        Task<Response> PublishResult(List<Guid> requestIds);

        /// <summary>
        /// Uploading the error files by evaluator
        /// </summary>
        /// <param name="file"></param>
        /// <param name="requestId"></param>
        /// <returns></returns>
        Task<FileDownloadResponse> UploadErrorAsync(IFormFile file, string comment, Guid requestId);

        /// <summary>
        /// Used to update the evaluated result details
        /// </summary>
        /// <param name="id">Request Id</param>
        /// <param name="evaluation">Evaluation Model</param>
        /// <returns></returns>
        Task<Response> UpdateEvaluationResult(Guid id, Evaluation evaluation);

        Task<MemoryStream> ListRequestDetailsForReport(RequestSearchFilter requestSearchFilter);

        /// <summary>
        /// To Download the Assignment Solution 
        /// </summary>
        /// <param name="requestIds"></param>
        /// <returns></returns>
        Task<Response> UpdateAssignmentDownloadDate(IEnumerable<Guid> requestIds); 
    }
}

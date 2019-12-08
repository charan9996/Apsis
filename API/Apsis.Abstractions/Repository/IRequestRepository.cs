using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Apsis.Models.Common;
using Apsis.Models.Entities;
using Apsis.Models.ViewModel;
using Apsis.Models.Response;

namespace Apsis.Abstractions.Repository
{
    public interface IRequestRepository
    {

        Task<List<RequestModel>> GetLearnerRequests(Guid notClearedId, Guid clearedId);

        /// <summary>
        /// To get all the details of the Assignment Attempts table against requested request ids
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        Task<IEnumerable<Guid>> GetAssignmentAttemptsDetails(IEnumerable<Guid> requestId);

        /// <summary>
        /// To get all the details of the Uploads table against requested Uploaded File ids
        /// </summary>
        /// <param name="UploadedFileId"></param>
        /// <returns></returns>
        Task<IEnumerable<Upload>> GetUploadsDetails(IEnumerable<Guid> UploadedFileId);
        /// <summary>
        /// Get requests against a search filter from DB 
        /// </summary>
        /// <param name="requestSearchFilter">The request filter to pass</param>
        /// <returns>RequestView containing required properties</returns>
        Task<IEnumerable<RequestView>> GetRequests(RequestSearchFilter requestSearchFilter);
        /// <summary>
        /// Add Yorbit input to Request table
        /// </summary>
        /// <param name="requests"></param>
        /// <returns></returns>
        Task<Response> AddRequests(List<RequestModel> requests);
        /// <summary>
        /// To update AssignmentDuedate by learning Opm
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="newAssignmentDueDate"></param>
        /// <returns></returns>
        Task<bool> UpdateAssignmentDueDate(Guid requestId, DateTime newAssignmentDueDate);
        /// <summary>
        /// To check whether AssignmentDuedate can be updated by Learning OPM
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>    
        Task<IEnumerable<AssignmentAttemptModel>> GetAssignmentAttemptsAsync(Guid requestId);
        Task<Request> GetRequestDetailsAsync(Guid requestId);
        Task<int> AddAssignmentAttemptAsync(AssignmentAttempt assignmentAttempt);
        Task<int> UpdateAssignmentAttemptForUploadedFileAsync(Guid requestId, Guid resultId, Guid latestResultId, Guid fileId);
        Task<int> UploadFileAsync(Upload upload);
        Task<bool> CheckCriteriaUpdateAssignmentDueDate(Guid requestId);

        /// <summary>
        /// Updating assignmentattempt with error file id and comment
        /// </summary>
        /// <param name="assignmentId"></param>
        /// <param name="comment"></param>
        /// <param name="fileId"></param>
        /// <returns></returns>
        Task<int> UpdateAssignmenAttemptForErrorFileUpload(Guid assignmentId, string comment, Guid fileId);

        /// <summary>
        /// Obtaining current AssignmentAttempt
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        Task<AssignmentAttemptModel> GetCurrentAssignmentAttemptsAsync(Guid requestId);

        /// <summary>
        /// Obtaining the user's Mid
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        Task<string> GetUserMidAsync(Guid requestId);


        /// <summary>
        /// To get the Request Details
        /// </summary>
        /// <param name="requestId">Request Id</param>
        /// <returns></returns>
        Task<RequestDetailsModel> GetRequestDetails(Guid requestId);

        /// <summary>
        /// To get the ResultId & Id of Recent Assignment Attempt
        /// </summary>
        /// <param name="id">Request Id</param>
        /// <returns></returns>
        Task<AssignmentAttempt> GetRecentAssignmentAttemptDetails(Guid id);

        /// <summary>
        /// To update the Evaluation Result Details
        /// </summary>
        /// <param name="id">AssignmentAttempt Id</param>
        /// <param name="evaluation">Evaluation Model</param>
        /// <returns></returns>
        Task<int> UpdateEvaluationResult(Guid id, Evaluation evaluation, Guid scoreCardId);
        Task<List<Guid>> GetRequestIdsForResultPublish(List<Guid> requestIds);
        Task<bool> UpdatePublishedStatus(List<Guid> requestIds, bool isPublished);
        Task<List<RequestEmailModel>> GetRequestDetailsForEmailAsync(List<Guid> requestIds);

        /// <summary>
        /// Used to get all the attempts details of the request
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        Task<List<AttemptsLogModel>> GetAttemptsLog(Guid requestId);
        /// <summary>
        /// To generate excel request report
        /// </summary>
        /// <param name="requestSearchFilter"></param>
        /// <returns></returns>
        Task<List<RequestReportModel>> GetRequestDetailsForReport(RequestSearchFilter requestSearchFilter);
        
        /// <summary>
        /// To update the date of downloading on download of Assignment Solution 
        /// </summary>
        /// <param name="requestIds"></param>
        /// <returns></returns>
        Task<int> UpdateAssignmentDownloadDate(IEnumerable<Guid> requestIds, string currentLoggedInMid);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<List<Guid>> GetPendingSubmissionRequestsId();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<List<Guid>> GetPendingEvaluationRequestId();
    }
}

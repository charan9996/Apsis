using Apsis.Abstractions;
using Apsis.Abstractions.Repository;
using Apsis.Logging;
using Apsis.Models.Authorization;
using Apsis.Models.Common;
using Apsis.Models.Constants;
using Apsis.Models.Entities;
using Apsis.Models.Response;
using Apsis.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apsis.Repository
{
    public class RequestRepository : IRequestRepository
    {
        /// <summary>
        /// 
        /// </summary>
        readonly IRepository _dataRepository;

        readonly IContextProvider _contextProvidor;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataRepository"></param>
        public RequestRepository(IRepository dataRepository, IContextProvider contextProvidor)
        {
            _dataRepository = dataRepository;
            _contextProvidor = contextProvidor;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<List<RequestModel>> GetLearnerRequests(Guid notClearedId, Guid clearedId)
        {
            ApplicationContext context = _contextProvidor.Context;
            try
            {
                Guid learnerId = context.CurrentUser.Id;
                return (await _dataRepository.QueryAsync<RequestModel>(SqlQueries.GetLearnerRequests, new { learnerId , notClearedId, clearedId}))?.ToList();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
               return null;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Guid>> GetAssignmentAttemptsDetails(IEnumerable<Guid> requestId)
        {
            try
            {
                return (await _dataRepository.QueryAsync<Guid>(SqlQueries.GetAssignmentAttemptsByRequestId, new { id = requestId }))?.ToList();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                return null;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uploadedFileId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Upload>> GetUploadsDetails(IEnumerable<Guid> uploadedFileId)
        {
            try
            {
                return (await _dataRepository.QueryAsync<Upload>(SqlQueries.GetMultipleUploadsByUploadedFileId, new { id = uploadedFileId }))?.ToList();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                return null;
            }
        }

        public async Task<int> AddAssignmentAttemptAsync(AssignmentAttempt assignmentAttempt)
        {
            try
            {
                return await _dataRepository.ExecuteAsync(SqlQueries.AddAssignmentAttempt, new { id = assignmentAttempt.Id, requestId = assignmentAttempt.RequestId, uploadedFileId = assignmentAttempt.UploadedFileId, resultId = assignmentAttempt.ResultId });
            }
            catch (Exception ex)
            {
                Logging.Logger.LogException(ex);
                return 0;
            }
        }

        public async Task<Request> GetRequestDetailsAsync(Guid requestId)
        {
            try
            {
                return (await _dataRepository.QueryAsync<Request>(SqlQueries.GetRequestDetails, new { id = requestId }))?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Logging.Logger.LogException(ex);
                return null;
            }
        }

        public async Task<List<RequestEmailModel>> GetRequestDetailsForEmailAsync(List<Guid> requestIds)
        {
            try
            {
                return (await _dataRepository.QueryAsync<RequestEmailModel>(SqlQueries.GetRequestCourseDetailsForEmail, new { ids= requestIds }))?.ToList();
            }
            catch (Exception ex)
            {
                Logging.Logger.LogException(ex);
                return null;
            }
        }

        public async Task<IEnumerable<AssignmentAttemptModel>> GetAssignmentAttemptsAsync(Guid requestId)
        {
            try
            {
                return (await _dataRepository.QueryAsync<AssignmentAttemptModel>(SqlQueries.GetAssignmentAttempts, new { id = requestId }))?.ToList();
            }
            catch (Exception ex)
            {
                Logging.Logger.LogException(ex);
                return null;
            }
        }

        /// <summary>
        /// Add Yorbit input to Request table 
        /// </summary>
        /// <param name="requests"></param>
        /// <returns></returns>
        public async Task<Response> AddRequests(List<RequestModel> requests)
        {
            Response response = new Response();
            response.IsSuccess = false;
            try
            {
                foreach (var request in requests)
                {
                    await _dataRepository.ExecuteAsync(SqlQueries.AddToRequest, request);
                }
                response.IsSuccess = true;
                response.Message = "Successfully posted to Yorbit Status Table";
                return response;
            }
            catch (Exception ex)
            {
                Logging.Logger.LogException(ex);
                response.Message = "File upload failed : Insert query to Request not working";
                return response;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchFilter"></param>
        /// <returns></returns>
        public async Task<IEnumerable<RequestView>> GetRequests(RequestSearchFilter requestSearchFilter)
        {
            try
            {
                string query = PrepareRequestQueryWithFilters(SqlQueries.GetRequestList, requestSearchFilter);
                return await _dataRepository.QueryAsync<RequestView>(query);
            }
            catch (Exception ex)
            {
                Logging.Logger.LogException(ex);
                return Enumerable.Empty<RequestView>();
            }
        }

        /// <summary>
        ///  To check whether AssignmentDuedate can be updated by Learning OPM
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public async Task<bool> CheckCriteriaUpdateAssignmentDueDate(Guid requestId)
        {
            bool canUpdateDueDate = false;
            try
            {
                List<AssignmentAttempt> _assignmentAttempts =
                    (await _dataRepository.QueryAsync<AssignmentAttempt>(SqlQueries.GetAssignmentAttempts, new { id = requestId }))?.ToList();
                if (_assignmentAttempts.Count == 1 && _assignmentAttempts[0].ResultId==Guid.Empty)
                {
                 
                    Guid yorbitStatusId = (await _dataRepository.QueryAsync<Guid>(SqlQueries.GetYorbitStatusIdByRequestId, new { id = requestId })).Single();
                    canUpdateDueDate = yorbitStatusId == Constants.YetToSubmitYorbitStatusId || yorbitStatusId == Constants.YetToResubmitYorbitStatusId;
                }
                else if (_assignmentAttempts.Count != 0)
                {
                    canUpdateDueDate = _assignmentAttempts[0].ResultId == Constants.ErrorId || _assignmentAttempts[0].ResultId == Constants.NotClearedId;
                }
                return canUpdateDueDate;
            }
            catch (Exception ex)
            {
                Logging.Logger.LogException(ex);
                return false;
            }
        }

        /// <summary>
        ///  To update AssignmentDuedate by learning Opm
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="newAssignmentDueDate"></param>
        /// <returns></returns>
        public async Task<bool> UpdateAssignmentDueDate(Guid requestId, DateTime newAssignmentDueDate)
        {
            try
            {
                int count = await _dataRepository.ExecuteAsync(SqlQueries.UpdateAssignmentDueDate,
                      new { id = requestId, NewAssignmentDueDate = newAssignmentDueDate});
                return count == 1;
            }
            catch (Exception ex)
            {
                Logging.Logger.LogException(ex);
                return false;
            }
        }
        private string PrepareRequestQueryWithFilters(string query, RequestSearchFilter requestSearchFilter, bool isExport = false)
        {
            ApplicationContext context = _contextProvidor.Context;
            Guid EvaluatorId = context.CurrentUser.Id;
            StringBuilder dynamicQuery = new StringBuilder();
            dynamicQuery.Append(query);
            dynamicQuery.Append(" ");
            if(context.CurrentUser.RoleId == Apsis.Models.Constants.Constants.EvaluatorRoleId)
                dynamicQuery.Append($"AND [A].ResultId = '7002ED9E-47D4-4C68-86FF-39EEE95ACEB4'  AND [R].EvaluatorId = '{EvaluatorId}'");

            dynamicQuery.Append($" AND [U].[Mid] != '{context.CurrentUser.Mid}' ");


            //====================== Where clause - based on Status for Assignment ===================
            dynamicQuery.Append(" ");
            if (requestSearchFilter.Filter == E_RESULT_FILTER.Project_Not_Cleared)
                dynamicQuery.Append($"AND [A].[ResultId] = '{RequestResultCodes.Project_Not_Cleared}'");
            else if (requestSearchFilter.Filter == E_RESULT_FILTER.Result_Awaited)
                dynamicQuery.Append($"AND [A].[ResultId] = '{RequestResultCodes.Result_Awaited}'");
            else if (requestSearchFilter.Filter == E_RESULT_FILTER.Project_Cleared)
                dynamicQuery.Append($"AND [A].[ResultId] = '{RequestResultCodes.Project_Cleared}'");
            else if (requestSearchFilter.Filter == E_RESULT_FILTER.ERROR)
                dynamicQuery.Append($"AND [A].[ResultId] = '{RequestResultCodes.ERROR}'");
            else if (requestSearchFilter.Filter == E_RESULT_FILTER.YET_TO_SUBMIT)
                dynamicQuery.Append($"AND [A].[SubmissionDate] IS NULL");
            else if (requestSearchFilter.Filter == E_RESULT_FILTER.YET_TO_PUBLISHED)
                dynamicQuery.Append($"AND [R].[isPublished] =0 AND [A].[ResultId] IN('{RequestResultCodes.Project_Cleared}','{RequestResultCodes.Project_Not_Cleared}')");

            //=============================== Keyword ======================================
            if (requestSearchFilter.Keyword != null && requestSearchFilter.Keyword.Length > 0)
            {
                StringBuilder subKeywordSearchConditions = new StringBuilder();
                String escapedKeyword = EscapeCharacters(requestSearchFilter.Keyword.Trim());

                DateTime dateTime;
                if (DateTime.TryParse(escapedKeyword, out dateTime))
                {
                    subKeywordSearchConditions.Append($" CONVERT(NVARCHAR(50), [A].[SubmissionDate], 126)  LIKE '%{dateTime.ToString("yyyy-MM-dd")}%' ESCAPE '\\' ");
                    subKeywordSearchConditions.Append(" OR ");
                    subKeywordSearchConditions.Append($" CONVERT(NVARCHAR(50), [R].[AssignmentDueDate], 126)  LIKE '%{dateTime.ToString("yyyy-MM-dd")}%' ESCAPE '\\' ");
                }
                else
                {
                    string keyword = $"'%{escapedKeyword}%'";

                    subKeywordSearchConditions.Append($" [R].[YorbitRequestId] LIKE {keyword}  ESCAPE '\\' OR ");
                    subKeywordSearchConditions.Append($" [U].[Name] LIKE {keyword} ESCAPE '\\' OR ");
                    subKeywordSearchConditions.Append($" [U].[MID] LIKE {keyword} ESCAPE '\\' OR ");
                    subKeywordSearchConditions.Append($" [C].[Academy] LIKE {keyword} ESCAPE '\\' OR ");
                    subKeywordSearchConditions.Append($" [C].[Name] LIKE {keyword} ESCAPE '\\' OR ");
                    //subKeywordSearchConditions.Append($" [C].[YorbitCourseId] LIKE {keyword} ESCAPE '\\' OR ");
                    subKeywordSearchConditions.Append($" CAST([A].[SubmissionDate] as Date) LIKE {keyword} ESCAPE '\\' OR ");
                    subKeywordSearchConditions.Append($" CAST([R].[AssignmentDueDate] as Date) LIKE {keyword} ESCAPE '\\'");
                }
                dynamicQuery.Append(" ");
                dynamicQuery.Append($"AND ({subKeywordSearchConditions.ToString()})");
            }

            //=========================== SORT ORDER ================================
            
            string order;
            switch (requestSearchFilter.Sort)
            {
                //---------------YORBIT REQUEST ID-----------------------
                case Models.Constants.E_SORT_ORDER.YORBIT_ID_ASC:
                    order = "[R].[YorbitRequestId] ASC";
                    break;
                case Models.Constants.E_SORT_ORDER.YORBIT_ID_DESC:
                    order = "[R].[YorbitRequestId] DESC";
                    break;

                //---------------COURSE_NAME-----------------------
                case Models.Constants.E_SORT_ORDER.COURSE_NAME_ASC:
                    order = "[C].[Name] ASC";
                    break;
                case Models.Constants.E_SORT_ORDER.COURSE_NAME_DESC:
                    order = "[C].[Name] DESC";
                    break;

                //---------------COURSE_NAME-----------------------
                case Models.Constants.E_SORT_ORDER.LEARNER_NAME_ASC:
                    order = "[U].[Name] ASC";
                    break;
                case Models.Constants.E_SORT_ORDER.LEARNER_NAME_DESC:
                    order = "[U].[Name] DESC";
                    break;

                //---------------COURSE_NAME-----------------------
                case Models.Constants.E_SORT_ORDER.LEARNER_MID_ASC:
                    order = "[U].[MID] ASC";
                    break;
                case Models.Constants.E_SORT_ORDER.LEARNER_MID_DESC:
                    order = "[U].[MID] DESC";
                    break;

                //---------------ACADEMY_NAME-----------------------
                case Models.Constants.E_SORT_ORDER.ACADEMY_NAME_ASC:
                    order = "[C].[Academy] ASC";
                    break;
                case Models.Constants.E_SORT_ORDER.ACADEMY_NAME_DESC:
                    order = "[C].[Academy] DESC";
                    break;

                //---------------EVALUATION_DATE-----------------------
                case Models.Constants.E_SORT_ORDER.EVALUATION_DATE_ASC:
                    order = "[R].[AssignmentDueDate] ASC";
                    break;
                case Models.Constants.E_SORT_ORDER.EVALUATION_DATE_DESC:
                    order = "[R].[AssignmentDueDate] DESC";
                    break;
                //---------------SUBMISSION_DATE-----------------------
                case Models.Constants.E_SORT_ORDER.SUBMISSION_DATE_ASC:
                    order = "[A].[SubmissionDate] ASC";
                    break;
                case Models.Constants.E_SORT_ORDER.SUBMISSION_DATE_DESC:
                    order = "[A].[SubmissionDate] DESC";
                    break;
                //---------------SUBMISSION_DATE-----------------------
                default:
                    order = "[R].[AssignmentDueDate] DESC";
                    break;
            }

            order = "ORDER BY " + order;
            dynamicQuery.Append(" ");
            dynamicQuery.Append(order);
            dynamicQuery.Append(" ");

            if (!isExport)
            {
                dynamicQuery.Append($"offset {requestSearchFilter.CurrentPage} rows fetch next {requestSearchFilter.PageSize} rows only;");
            }
            return dynamicQuery.ToString();
        }

        public async Task<int> UpdateAssignmentAttemptForUploadedFileAsync(Guid requestId, Guid resultId, Guid latestResultId, Guid fileId)
        {
            try
            {
                return await _dataRepository.ExecuteAsync(SqlQueries.UpdateAssignmentAttemptForUploadedFile, new { id = requestId, resultId, latestResultId, uploadedFileId = fileId });
            }
            catch (Exception ex)
            {
                Logging.Logger.LogException(ex);
                return 0;
            }
        }

        public async Task<int> UploadFileAsync(Upload upload)
        {
            try
            {
                return await _dataRepository.ExecuteScalarAsync<int>(SqlQueries.AddUpload
                    , new { id = upload.Id, filename = upload.FileName, filepath = upload.FilePath });
            }
            catch (Exception ex)
            {
                Logging.Logger.LogException(ex);
                return 0;
            }

        }

        private string EscapeCharacters(string input)
        {
            input = input.Replace(@"\", @"\\");
            input = input.Replace(@"%", @"\%");
            input = input.Replace(@"[", @"\[");
            input = input.Replace(@"]", @"\]");
            input = input.Replace(@"_", @"\_");
            return input;
        }

        /// <summary>
        /// Updating AssignmentAttempt with fileId and comment
        /// </summary>
        /// <param name="assignmentAttemptId"></param>
        /// <param name="comment"></param>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public async Task<int> UpdateAssignmenAttemptForErrorFileUpload(Guid assignmentAttemptId, string comment, Guid fileId)
        {
            try
            {
                return await _dataRepository.ExecuteAsync(SqlQueries.UpdateAssignmenAttemptForErrorFileUpload,
                    new { id = assignmentAttemptId, comment = comment, fileId = fileId, evaluationDate = DateTime.Now });
            }
            catch (Exception ex)
            {
                Logging.Logger.LogException(ex);
                return 0;
            }
        }

        /// <summary>
        /// Obtaining current AssignmentAttempt
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public async Task<AssignmentAttemptModel> GetCurrentAssignmentAttemptsAsync(Guid requestId)
        {
            try
            {
                return (await _dataRepository.QueryAsync<AssignmentAttemptModel>(SqlQueries.GetCurrentAssignmentAttempts, new { id = requestId }))?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Logging.Logger.LogException(ex);
                return null;
            }
        }

        public async Task<string> GetUserMidAsync(Guid requestId)
        {
            try
            {
                var currentUser = (await _dataRepository.QueryAsync<User>(SqlQueries.GetCurrentUser, new { id = requestId }))?.FirstOrDefault();
                return currentUser.Mid;
            }
            catch (Exception ex)
            {
                Logging.Logger.LogException(ex);
                return null;
            }
        }


        public async Task<RequestDetailsModel> GetRequestDetails(Guid requestId)
        {
            try
            {
                return (await _dataRepository.QueryAsync<RequestDetailsModel>(SqlQueries.GetAllRequestDetails, new { id = requestId }))?.FirstOrDefault();
            }
            catch(Exception ex)
            {
                Logging.Logger.LogException(ex);
                return null;
            }
        }
        /// <summary>
        /// Method to get email parameter for publishing result
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public async Task<List<Guid>> GetRequestIdsForResultPublish(List<Guid> requestIds)
        {
            try
            {
               List<Guid> requestIds_ResultPublish= (await _dataRepository.QueryAsync<Guid>(SqlQueries.GetRequestId_ResultPublish, new { ids = requestIds,
               Clear=RequestResultCodes.Project_Cleared,
                   notClear=RequestResultCodes.Project_Not_Cleared
               }))?.ToList();
               return requestIds_ResultPublish;
            }
            catch (Exception ex)
            {
                Logging.Logger.LogException(ex);
                return null;
            }

        }
        /// <summary>
        /// method to update is Published in Request Table
        /// </summary>
        /// <param name="requestIds"></param>
        /// <returns></returns>
        public async Task<bool> UpdatePublishedStatus(List<Guid> requestIds, bool isPublished)
        {
            try
            {
                int count = await _dataRepository.ExecuteAsync(SqlQueries.UpdatePublishedStatus, new { ids = requestIds, isPublished });
                return true;
            }
            catch (Exception ex)
            {
                Logging.Logger.LogException(ex);
                return false;
            }
        }

        public async Task<AssignmentAttempt> GetRecentAssignmentAttemptDetails(Guid id)
        {
            try
            {
                return (await _dataRepository.QueryAsync<AssignmentAttempt>(SqlQueries.GetRecentAssignmentAttemptDetails, new { id }))?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Logging.Logger.LogException(ex);
                return null;
            }
        }

        public async Task<int> UpdateEvaluationResult(Guid id, Evaluation evaluation, Guid scoreCardId)
        {
            try
            {
                if(!(scoreCardId.Equals(Guid.Empty)))
                {
                    return (await _dataRepository.ExecuteAsync(SqlQueries.UpdateEvaluationResult, new { aId = id, comments = evaluation.Comments, score = evaluation.Score, resultId = evaluation.ResultId, evaluationDate = evaluation.EvaluationDate, attemptScoreCardId = scoreCardId }));
                }
                else
                {
                    return (await _dataRepository.ExecuteAsync(SqlQueries.UpdateEvaluationResultNonFile, new { aId = id, comments = evaluation.Comments, score = evaluation.Score, resultId = evaluation.ResultId, evaluationDate = evaluation.EvaluationDate}));
                }
                
            }
            catch (Exception ex)
            {
                Logging.Logger.LogException(ex);
                return -1;
            }
        }

        public async Task<List<AttemptsLogModel>> GetAttemptsLog(Guid requestId)
        {
            try
            {
                return (await _dataRepository.QueryAsync<AttemptsLogModel>(SqlQueries.GetAttemptsLog, new { id = requestId }))?.ToList();
            }
            catch (Exception ex)
            {
                Logging.Logger.LogException(ex);
                return null;
            }
        }

        public async Task<List<RequestReportModel>> GetRequestDetailsForReport(RequestSearchFilter requestSearchFilter)
        {
            //List<RequestReportModel> RequestDetailsList = new List<RequestReportModel>();
            try
            {
                string query = PrepareRequestQueryWithFilters(SqlQueries.GetRequestDetailsForReport, requestSearchFilter, isExport: true);
                /*string append = @") [GetRID]
                                                    LEFT OUTER JOIN
            (SELECT[Rq].[YorbitRequestId] FROM[Request] as [Rq] INNER JOIN

                  [AssignmentAttempt] as [asa] ON
                               [Rq].[Id] = [asa].[RequestId])
                  [AA]
                  ON
                  [AA].[YorbitRequestId] = [GetRID].[RequestId]";*/

                //string ExecutableQuery = query+append; 
                List<RequestReportModel> RequestDetailsList = (await _dataRepository.QueryAsync<RequestReportModel>(query))?.ToList();

                return RequestDetailsList;
            }
            catch (Exception e)
            {
                return null;
            }
        } 
        
        public async Task<int> UpdateAssignmentDownloadDate(IEnumerable<Guid> requestIds, string currentLoggedInMid)
        {
            try
            {
                return (await _dataRepository.ExecuteAsync(SqlQueries.UpdateAssignmentDownloadDate, new { id = requestIds, mid = currentLoggedInMid }));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                return -1;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<List<Guid>> GetPendingSubmissionRequestsId()
        {
            //x.SubmissionDate == null
            //           || x.ResultId == Constants.ErrorId
            //           || x.ResultId == Constants.NotClearedId)
            StringBuilder conditions = new StringBuilder();
            conditions.Append($" [A].[SubmissionDate] IS NULL ");
            conditions.Append($" OR [A].[ResultId] = '{Constants.ErrorId}' ");
            conditions.Append($" OR [A].[ResultId] = '{Constants.NotClearedId}' ");

            string query = $"{SqlQueries.GetRequestList} AND ({conditions.ToString()})";
            List<Guid> requestIds = (await _dataRepository.QueryAsync<RequestView>(query)).Select(x => x.RequestId).ToList();
            return requestIds;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<List<Guid>> GetPendingEvaluationRequestId()
        {
            StringBuilder conditions = new StringBuilder();
            conditions.Append($" [A].[ResultId]='{Constants.ResultAwaited}' ");
            conditions.Append($" AND [A].[EvaluatorDownloadedDate] < DateAdd(day, -5, GETDATE()) ");
            string query = $"{SqlQueries.GetRequestList} AND ({conditions.ToString()})";
            List<Guid> requestIds = (await _dataRepository.QueryAsync<RequestView>(query)).Select(x => x.RequestId).ToList();
            return requestIds;
        }
    }
}

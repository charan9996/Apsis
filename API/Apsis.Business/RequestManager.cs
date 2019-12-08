using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Apsis.Abstractions.Business;
using Apsis.Abstractions.Repository;
using Apsis.Models.Common;
using Apsis.Models.Response;
using Apsis.Models.ViewModel;
using Apsis.Models.Constants;
using Apsis.Models.Entities;
using Microsoft.AspNetCore.Http;
using Apsis.AzureServices;
using Apsis.Notification;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using Apsis.Models.Authorization;
using Apsis.Abstractions;

namespace Apsis.Business
{
    public class RequestManager : IRequestManager
    {
        readonly IRequestRepository _requestRepository;
        readonly IFileOperationsManager _fileOperationsManager;
        readonly IBlobHelper _blobHelper;
        readonly IEmailManager _emailManager;
        readonly ApplicationContext _context;

        public RequestManager(
            IRequestRepository requestRepository,
            IFileOperationsManager fileOperationsManager,
            IBlobHelper blobHelper,
            IEmailManager emailManager,
            IContextProvider contextProvider)
        {
            _requestRepository = requestRepository;
            _fileOperationsManager = fileOperationsManager;
            _blobHelper = blobHelper;
            _emailManager = emailManager;
            _context = contextProvider.Context;
        }

        public async Task<FileDownloadResponse> DownloadAssignment(IEnumerable<Guid> requestIds)
        {
            FileDownloadResponse fileDownloadResponse;
            var listFileIds = await _requestRepository.GetAssignmentAttemptsDetails(requestIds);
            if (listFileIds != null && listFileIds.Any())
            {
                var fileDetails = await _requestRepository.GetUploadsDetails(listFileIds);
                if (fileDetails != null)
                {
                    if (fileDetails.Count() == 1)
                    {
                        fileDownloadResponse = await _fileOperationsManager.DownloadFile(fileDetails);
                    }
                    else
                    {
                        fileDownloadResponse = await _fileOperationsManager.DownloadZipFile(fileDetails);
                    }
                }
                else
                {
                    fileDownloadResponse = new FileDownloadResponse { Message = "No files to download." };
                }
            }
            else
            {
                fileDownloadResponse = new FileDownloadResponse { Message = "No files to download." };
            }
            return fileDownloadResponse;
        }

        public async Task<List<RequestModel>> GetLearnerRequests()
        {
            return await _requestRepository.GetLearnerRequests(RequestResultCodes.Project_Not_Cleared, RequestResultCodes.Project_Cleared);
        }

        public async Task<RequestListResponse> ListAllRequests(RequestSearchFilter requestSearchFilter)
        {
            RequestListResponse response = new RequestListResponse();
            try
            {
                requestSearchFilter.CurrentPage = (requestSearchFilter.CurrentPage - 1) * requestSearchFilter.PageSize;
                if (requestSearchFilter.CurrentPage <= 0)
                    requestSearchFilter.CurrentPage = 0;
                response.requestViews = await _requestRepository.GetRequests(requestSearchFilter);
                response.IsSuccess = response.requestViews.Any();
                if (!response.IsSuccess)
                    response.Message = "No requests found retrieved from given filters / query.";
            }
            catch (Exception e)
            {
                response.Message = "Error in retreiving request list with filters.";
                response.IsSuccess = false;
            }
            return response;
        }

        public async Task<Response> UploadAssignmentSolutionAsync(IFormFile file, Guid requestId)
        {
            var response = new Response();
            try
            {
                var requestList = await _requestRepository.GetAssignmentAttemptsAsync(requestId);
                if (requestList == null)
                {
                    response.Message = "Invalid Id.";
                    return response;
                }
                AssignmentAttemptModel latestAttempt = requestList.FirstOrDefault();
                string fileName = latestAttempt.Mid + "_" + latestAttempt.YorbitRequestId + "_" + DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss") + ".zip";
                Guid latestResult = latestAttempt.ResultId;
                // No attempts yet
                if (latestResult == Guid.Empty)
                {
                    // Upload file to blob and add a record in Upload table
                    var upload = await UploadFileAsync(file, fileName);

                    // Add a record in AssignmentAttempt table
                    await AddAssignmentAttemptAsync(requestId, upload.Id);
                    response.IsSuccess = true;
                }
                else
                {
                    int countAssignmentAttempt = requestList.Count();
                    // Resubmission for first and second attempt due to Not cleared/Error
                    if (countAssignmentAttempt < 3)
                    {
                        if (latestResult == RequestResultCodes.Project_Not_Cleared)
                        {
                            // Upload file to blob and add a record in Upload table
                            var upload = await UploadFileAsync(file, fileName);

                            // Add a record in AssignmentAttempt table
                            await AddAssignmentAttemptAsync(requestId, upload.Id);
                            response.IsSuccess = true;
                        }
                        else if (latestResult == RequestResultCodes.ERROR)
                        {
                            // Upload file to blob and add a record in Upload table
                            var upload = await UploadFileAsync(file, fileName);
                            await _requestRepository.UpdateAssignmentAttemptForUploadedFileAsync(requestList.FirstOrDefault().RequestId, RequestResultCodes.Result_Awaited, RequestResultCodes.ERROR, upload.Id);
                            response.IsSuccess = true;
                        }
                        else
                        {
                            response.Message = "Previous assignment's evaluation is pending.";
                        }
                    }
                    // Resubmission for third attempt due to Error
                    else if (countAssignmentAttempt == 3 && latestResult == RequestResultCodes.ERROR)
                    {
                        // Upload file to blob and add a record in Upload table
                        var upload = await UploadFileAsync(file, fileName);
                        await _requestRepository.UpdateAssignmentAttemptForUploadedFileAsync(requestList.FirstOrDefault().RequestId, RequestResultCodes.Result_Awaited, RequestResultCodes.ERROR, upload.Id);

                        response.IsSuccess = true;
                    }
                    // All 3 attempts not cleared
                    else
                    {
                        response.Message = "Upload failed: Attempts exhausted.";
                        return response;
                    }

                }
                if (response.IsSuccess)
                    _emailManager.SendMail(NotificationType.LearnerAssignmentUpload, new EmailDetails { RequestIds = new List<Guid> { requestId } });
            }
            catch (Exception ex)
            {
                Logging.Logger.LogException(ex);
                response.Message = "Cannot upload assignment solution.";
            }

            return response;

        }

        /// <summary>
        /// Add Yorbit Input to Request table
        /// </summary>
        /// <param name="requests"></param>
        /// <returns></returns>
        public async Task<Response> AddRequests(List<RequestModel> requests)
        {
            return await _requestRepository.AddRequests(requests);
        }
        /// <summary>
        ///  To update AssignmentDuedate by learning Opm
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="newAssignmentDueDate"></param>
        /// <returns></returns>
        public async Task<Response> UpdateAssignmentDueDate(Guid requestId, DateTime newAssignmentDueDate)
        {
            Response response = new Response();
            try
            {
                bool flag = await _requestRepository.CheckCriteriaUpdateAssignmentDueDate(requestId);
                if (flag != true)
                {
                    response.IsSuccess = false;
                    response.Message = "Due date cannot be updated.";
                    return response;
                }
                else
                {
                    bool result = await _requestRepository.UpdateAssignmentDueDate(requestId, newAssignmentDueDate);
                    if (result != true)
                    {
                        response.IsSuccess = false;
                        response.Message = "Error updating due date.";
                        return response;
                    }
                    else
                    {
                        response.IsSuccess = true;
                        response.Message = "Due date updated.";
                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Logger.LogException(ex);
                response.IsSuccess = false;
                response.Message = "Error Occured..Try Again";
                return response;
            }
        }

        private async Task<Upload> UploadFileAsync(IFormFile file, String fileName)
        {

            Upload upload = null;
            string fileUri = string.Empty;
            using (Stream stream = new MemoryStream())
            {
                file.OpenReadStream();
                await file.CopyToAsync(stream);
                stream.Seek(0, SeekOrigin.Begin);
                fileUri = await _blobHelper.UploadFileToBlobAsync("assignments", fileName, stream);
            }

            if (fileUri != string.Empty)
            {
                upload = new Upload { Id = Guid.NewGuid(), FileName = fileName, FilePath = fileUri };
                int uploadResponseByLearner = await _requestRepository.UploadFileAsync(upload);
            }
            return upload;
        }

        private async Task AddAssignmentAttemptAsync(Guid requestId, Guid uploadId)
        {
            var assignmentAttempt = new AssignmentAttempt { Id = Guid.NewGuid(), RequestId = requestId, UploadedFileId = uploadId, ResultId = RequestResultCodes.Result_Awaited };
            await _requestRepository.AddAssignmentAttemptAsync(assignmentAttempt);
        }

        public async Task<RequestDetailsModel> GetRequestDetails(Guid requestId)
        {
            var requestDetailsModel = new RequestDetailsModel();
            try
            {
                requestDetailsModel = await _requestRepository.GetRequestDetails(requestId);
    
                if(requestDetailsModel.AssignmentSolutionFileName != null)
                {
                    requestDetailsModel.AssignmentSolutionUrl = await _blobHelper.GetBlobUriAsync("assignments", requestDetailsModel.AssignmentSolutionFileName.Trim());
                }
                if(requestDetailsModel.ScoreCardFileName != null)
                {
                    requestDetailsModel.ScoreCardUrl = await _blobHelper.GetBlobUriAsync("assignments", requestDetailsModel.ScoreCardFileName.Trim());
                }
                if (requestDetailsModel.ErrorFileName != null)
                {
                    requestDetailsModel.ErrorFileUrl = await _blobHelper.GetBlobUriAsync("assignments", requestDetailsModel.ErrorFileName.Trim());
                }

                requestDetailsModel.AttemptsLog = await _requestRepository.GetAttemptsLog(requestId);

                requestDetailsModel.IsSuccess = true;
            }
            catch(Exception ex)
            {
                requestDetailsModel.Message = "Error Getting Request Details";
            }
            return requestDetailsModel;
        }

        /// <summary>
        /// Updating AssignmentAttempt with ErrorFile and comment
        /// </summary>
        /// <param name="file"></param>
        /// <param name="comment"></param>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public async Task<FileDownloadResponse> UploadErrorAsync(IFormFile file, string comment, Guid requestId)
        {
            var response = new FileDownloadResponse();
            try
            {
                var currentAssignmentAttempt = await _requestRepository.GetCurrentAssignmentAttemptsAsync(requestId); 
                
                if (currentAssignmentAttempt.ResultId == RequestResultCodes.Result_Awaited)
                {
                    string fileName = currentAssignmentAttempt.YorbitRequestId + "_" + currentAssignmentAttempt.Mid + "_ErrorFile_" + DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss") + ".zip";
                    var upload = await UploadFileAsync(file, fileName);

                    //Update comment and errorfileid in AssignmentAttempt
                    var updateResponse = await _requestRepository.UpdateAssignmenAttemptForErrorFileUpload(
                        currentAssignmentAttempt.Id, comment, upload.Id);
                    if(updateResponse>0)
                    {
                        _emailManager.SendMail(NotificationType.EvaluatorErrorFileUpload, new EmailDetails { RequestIds = new List<Guid> { requestId } });
                    }
                    response.FileUrl = upload.FilePath;
                    response.Message = "Succesfully Uploaded";
                    response.IsSuccess = true;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Assignment not in 'Yet to Evaluate ' status";
                }
                
            }
            catch (Exception ex)
            {
                Logging.Logger.LogException(ex);
                response.Message = "Cannot upload error file";
            }

            return response;
        }
       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestIds"></param>
        /// <returns></returns>
        public async Task<Response> PublishResult(List<Guid> requestIds)
        {
            Response response = new Response();
            List<Guid> _requestIds = new List<Guid>();
            List<Guid> _requestIds1= new List<Guid>();
            try
            {
                if (requestIds.Count() == 1 && (requestIds[0] == Guid.Empty || requestIds[0] == null))
                {
                    response.IsSuccess = false;
                    response.Message = "Invalid Input";
                }
                else if (requestIds.Count() >= 1 && (requestIds[0] != Guid.Empty && requestIds[0] != null))
                {
                    _requestIds = (await _requestRepository.GetRequestIdsForResultPublish(requestIds))?.ToList();
                    if (_requestIds.Any())
                    {

                        Response _response = await _emailManager.SendMail(NotificationType.OPMPublishResult, new EmailDetails { RequestIds = _requestIds });
                        bool status = await _requestRepository.UpdatePublishedStatus(_requestIds, isPublished: true);
                        response.IsSuccess = true;
                        response.Message = "Result Published";

                    }
                    else
                    {
                        response.Message = "Result Not Available for publish";
                        response.IsSuccess = false;
                    }
                }
                return response;
            }
            catch (Exception ex)
            {
                Logging.Logger.LogException(ex);
                response.IsSuccess = false;
                response.Message = "Error Occured.Please Try Again";
                return response;
            }
        }
        /// <summary>
        /// To generate Excel request report
        /// </summary>
        /// <param name="requestSearchFilter"></param>
        /// <returns></returns>
        public async Task<MemoryStream> ListRequestDetailsForReport(RequestSearchFilter requestSearchFilter)
        {
            List<RequestReportModel> requestReportModelList;
            //List<RequestView> requestCurrentStatusModel;
            RequestListResponse response = new RequestListResponse();
            try
            {
                requestSearchFilter.CurrentPage = (requestSearchFilter.CurrentPage - 1) * requestSearchFilter.PageSize;
                //if (requestSearchFilter.CurrentPage <= 0)
                requestSearchFilter.CurrentPage = 0;
                requestSearchFilter.PageSize = 900;
                requestSearchFilter.Sort = E_SORT_ORDER.SUBMISSION_DATE_ASC;
                var currentRequestStatus = await _requestRepository.GetRequests(requestSearchFilter);
                var currentRequestStatusList = currentRequestStatus.ToList();
                requestSearchFilter.CurrentPage = 0;           
                requestSearchFilter.Filter = E_RESULT_FILTER.ALL;
                requestReportModelList = await _requestRepository.GetRequestDetailsForReport(requestSearchFilter);
                List<RequestReportModel> finalRequestReportModelList = new List<RequestReportModel>();

                if (requestReportModelList == null)
                {
                    return null;
                }

                var requestReportList = new List<RequestReportExcelModel>();

                foreach (var filteredRequest in requestReportModelList)
                {
                    foreach(var checkRequest in currentRequestStatusList)
                    {
                        if (checkRequest.YorbitRequestId.Equals(filteredRequest.RequestID))
                        {
                            finalRequestReportModelList.Add(filteredRequest);
                        }
                    }
                    
                }

                var groupedRequests = finalRequestReportModelList.GroupBy(x => x.RequestID);
                foreach (var request in groupedRequests)
                {
                    var requestReport = new RequestReportExcelModel();
                    //requestReport.RequestID = request[0].RequestID;
                    //requestReport.Mid = request[0].Mid;
                    int i = 0;
                    foreach (var attempt in request)
                    {
                        bool fillEvaluatorDownloadDate= false;
                        bool fillEvaluationResultDate= false;
                        bool fillSubmissionDate = false;
                        if (!attempt.SubmissionDate.ToString().Equals("1/1/0001 12:00:00 AM"))
                            fillSubmissionDate = true;
                        if (!attempt.EvaluatorAssignmentDownloadDate.ToString().Equals("1/1/0001 12:00:00 AM"))
                            fillEvaluatorDownloadDate = true;
                        if (!attempt.EvaluationResultDate.ToString().Equals("1/1/0001 12:00:00 AM"))
                            fillEvaluationResultDate = true;

                        if (i == 0)
                        {
                            requestReport.RequestID = attempt.RequestID;
                            requestReport.Mid = attempt.Mid;
                            requestReport.Name = attempt.Name;
                            requestReport.EmailID = attempt.EmailID;
                            requestReport.CourseID = attempt.CourseID;
                            requestReport.CourseName = attempt.CourseName;
                            requestReport.Academy = attempt.Academy;
                            requestReport.ApprovedDate = attempt.ApprovedDate.ToString("MM/dd/yyyy");
                            requestReport.AssignmentDueDate = attempt.AssignmentDueDate.Date.ToString("MM/dd/yyyy");
                            requestReport.EvaluatorMailId = attempt.EvaluatorMailId;
                            requestReport.EvaluatorType = attempt.EvaluatorType;
                            requestReport.Vendor = attempt.Vendor;
                            if(fillSubmissionDate)
                                requestReport.SubmissionDate = attempt.SubmissionDate.Date.ToString("MM/dd/yyyy");
                            requestReport.LearnerTAT = attempt.LearnerTAT;
                            if(fillEvaluatorDownloadDate)
                                requestReport.FirstEvaluatorAssignmentDownloadDate = attempt.EvaluatorAssignmentDownloadDate.Date.ToString("MM/dd/yyyy");
                            if (fillEvaluationResultDate)
                            {
                                requestReport.FirstEvaluationResultDate = attempt.EvaluationResultDate.Date.ToString("MM/dd/yyyy");
                                requestReport.FirstEvaluatorTAT = attempt.EvaluatorTAT;
                            }
                            requestReport.FirstScore = attempt.Score;
                            requestReport.FirstRequestStatus = attempt.RequestStatus;
                            requestReport.FirstEvaluatorComments = attempt.EvaluatorComments;
                        }
                        else if (i == 1)
                        {
                            requestReport.FirstResubmissionDate = attempt.SubmissionDate.Date.ToString("MM/dd/yyyy");
                            if (fillEvaluatorDownloadDate)
                                requestReport.SecondEvaluatorAssignmentDownloadDate = attempt.EvaluatorAssignmentDownloadDate.Date.ToString("MM/dd/yyyy");
                            if (fillEvaluationResultDate)
                            {
                                requestReport.SecondEvaluationResultDate = attempt.EvaluationResultDate.Date.ToString("MM/dd/yyyy");
                                requestReport.SecondEvaluatorTAT = attempt.EvaluatorTAT;
                            }
                            requestReport.SecondScore = attempt.Score;
                            requestReport.SecondRequestStatus = attempt.RequestStatus;
                            requestReport.SecondEvaluatorComments = attempt.EvaluatorComments;
                        }
                        else
                        {
                            requestReport.SecondResubmissionDate = attempt.SubmissionDate.Date.ToString("MM/dd/yyyy");
                            if (fillEvaluatorDownloadDate)
                                requestReport.ThirdEvaluatorAssignmentDownloadDate = attempt.EvaluatorAssignmentDownloadDate.Date.ToString("MM/dd/yyyy");
                            if (fillEvaluationResultDate)
                            {
                                requestReport.ThirdEvaluationResultDate = attempt.EvaluationResultDate.Date.ToString("MM/dd/yyyy");
                                requestReport.ThirdEvaluatorTAT = attempt.EvaluatorTAT;
                            }
                            requestReport.ThirdScore = attempt.Score;
                            requestReport.ThirdRequestStatus = attempt.RequestStatus;
                            requestReport.ThirdEvaluatorComments = attempt.EvaluatorComments;
                        }
                        i++;
                    }
                    requestReportList.Add(requestReport);
                }
                MemoryStream memS = new MemoryStream();
                ExcelPackage excel = new ExcelPackage();
                List<string> listOfHeader = new List<string> {
                    "Request ID",
                    "MID",
                    "Name",
                    "Email id",
                    "Course Name",
                    "Course ID",
                    "Academy",
                    "Approved date",
                    "Due date for submission",
                    "Evaluator e-mail ID",
                    "Internal / Extenal",
                    "Vendor",
                    "Assignment submitted date(Tool to capture)",
                    "TAT by learner",
                    "Assignment downloaded date by evaluator",
                    "Date of evaluation results submitted by evaluator",
                    "Turn around time for evaluation",
                    "Score",
                    "Status(Cleared / Not cleared)",
                    "Comment by evaluator",
                    "Resubmission date",
                    "Assignment downloaded date by evaluator",
                    "Date of evaluation results submitted by evaluator",
                    "Turn around time for evaluation",
                    "Score",
                    "Status(Cleared / Not cleared)",
                    "Comment by evaluator",
                    "Resubmission date",
                    "Assignment downloaded date by evaluator",
                    "Date of evaluation results submitted by evaluator",
                    "Turn around time for evaluation",
                    "Score",
                    "Status(Cleared / Not cleared)",
                    "Comment by evaluator"
                };

                var worksheet = excel.Workbook.Worksheets.Add("Requests");
                if (requestReportList.Any())
                {
                    worksheet.Column(20).Style.WrapText = true;
                    worksheet.Column(20).AutoFit();
                    worksheet.Column(27).Style.WrapText = true;
                    worksheet.Column(27).AutoFit();
                    worksheet.Column(34).Style.WrapText = true;
                    worksheet.Column(34).AutoFit();
                    worksheet.Cells["M1:T1"].Merge = true;
                    worksheet.Cells["M1"].Value = "1st Attempt";
                    worksheet.Cells["M1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells["M1"].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);
                    worksheet.Cells["M1"].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                    worksheet.Cells["U1:AA1"].Merge = true;
                    worksheet.Cells["U1"].Value = "2nd Attempt";
                    worksheet.Cells["U1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells["U1"].Style.Fill.BackgroundColor.SetColor(Color.SkyBlue);
                    worksheet.Cells["U1"].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                    worksheet.Cells["AB1:AH1"].Merge = true;
                    worksheet.Cells["AB1"].Value = "3rd Attempt";
                    worksheet.Cells["AB1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells["AB1"].Style.Fill.BackgroundColor.SetColor(Color.DeepSkyBlue);
                    worksheet.Cells["AB1"].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                    worksheet.Cells["A2"].LoadFromArrays(new List<string[]>(new[] { listOfHeader.ToArray() }));
                    worksheet.Cells["A3"].LoadFromCollection(requestReportList, false);

                    int colCount = worksheet.Dimension.End.Column;
                    int rowCount = worksheet.Dimension.End.Row;
                    worksheet.Cells[1, 13, 1, colCount].Style.Font.Bold = true;
                    worksheet.Cells[1, 13, 1, colCount].AutoFilter = false;
                    worksheet.Cells[1, 13, rowCount, colCount].AutoFitColumns();
                    worksheet.Cells[1, 13, 1, colCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    worksheet.Cells[2, 1, 2, colCount].Style.Font.Bold = true;
                    worksheet.Cells[2, 1, 2, colCount].AutoFitColumns();
                    worksheet.Cells[2, 1, 2, colCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[2, 1, 2, colCount].AutoFilter = false;
                    worksheet.Cells[2, 1, rowCount, colCount].AutoFitColumns();

                    excel.SaveAs(memS);
                    excel.Dispose();
                    memS.Position = 0;
                    return memS;
                }
                return null;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<Response> UpdateEvaluationResult(Guid id, Evaluation evaluation)
        {
            Response response = new Response();
            try
            {
                var currentAssignmentAttempt = await _requestRepository.GetCurrentAssignmentAttemptsAsync(id);
                if (currentAssignmentAttempt.Mid == evaluation.Mid && currentAssignmentAttempt.RequestId == id)
                {
                if (currentAssignmentAttempt.ResultId == RequestResultCodes.Result_Awaited)
                {
                    var updateEvaluationResult = -1;
                    if (evaluation.ScoreCardFile != null)
                    {
                        string fileName = currentAssignmentAttempt.YorbitRequestId + "_" + currentAssignmentAttempt.Mid + "_ScoreCard_" + DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss") + ".zip";
                        var upload = await UploadFileAsync(evaluation.ScoreCardFile, fileName);
                        evaluation.EvaluationDate = DateTime.Now;
                        updateEvaluationResult = await _requestRepository.UpdateEvaluationResult(currentAssignmentAttempt.Id, evaluation, upload.Id);
                    }
                    else
                    {
                        evaluation.EvaluationDate = DateTime.Now;
                        updateEvaluationResult = await _requestRepository.UpdateEvaluationResult(currentAssignmentAttempt.Id, evaluation, Guid.Empty);
                    }
           
                    if(updateEvaluationResult > 0)
                    {
                        await _requestRepository.UpdatePublishedStatus(new List<Guid> { id }, isPublished: false);
                        _emailManager.SendMail(NotificationType.EvaluatorCompleteEvaluation, new EmailDetails { RequestIds = new List<Guid> { id } });
                    }
                    response.IsSuccess = true;
                }
            }
             else
                {
                    response.IsSuccess = false;
                    response.Message = "Not For Evaluation";
                    return response;
                }
        }
            catch (Exception ex)
            {
                response.Message = "Error Updating Evaluation Result Details";
            }
            return response;
        }

        public async Task<Response> UpdateAssignmentDownloadDate(IEnumerable<Guid> requestIds)
        {
            Response response = new Response();
            var currentMid = _context.CurrentUser.Mid;
            var updateCount = await _requestRepository.UpdateAssignmentDownloadDate(requestIds, currentMid);
            return response;
        }
    }
}

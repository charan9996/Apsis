 using Apsis.Abstractions.Repository;
using Apsis.Models.Common;
using Apsis.Models.Constants;
using Apsis.Models.Response;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apsis.AzureServices;
using Apsis.Models.Constants;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Text;
using System;

namespace Apsis.Notification
{
    public class EmailManager : IEmailManager
    {
        readonly string _learningOPMEmail;
        readonly IEmailHelper _emailHelper;
        readonly IRequestRepository _requestRepository;
        readonly IBlobHelper _blobHelper;
        readonly IHostingEnvironment _hostingEnvironment;
        RequestResultCodes requestResultCodes = new RequestResultCodes();

        public EmailManager(IHostingEnvironment hostingEnvironment, IConfiguration configuration, IEmailHelper emailHelper, IRequestRepository requestRepository,IBlobHelper blobHelper)
        {
            _learningOPMEmail = configuration["AppSettings:LearningOPMEmail"];
            _emailHelper = emailHelper;
            _requestRepository = requestRepository;
            _blobHelper= blobHelper;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<Response> SendMail(NotificationType notificationType, EmailDetails emailDetails)
        {
            var response = new Response();
            try
            {
                if (emailDetails == null) return response;
                switch (notificationType)
                {
                    case NotificationType.LearnerAssignmentUpload:
                        response = await SendLearnerAssignmentUploadEmail(emailDetails);
                        break;
                    case NotificationType.LearnerReminder:
                        response = await SendLearnerReminderEmail(emailDetails);
                        break;
                    case NotificationType.EvaluatorErrorFileUpload:
                        response = await SendEvaluatorErrorFileUploadEmail(emailDetails);
                        break;
                    case NotificationType.EvaluatorCompleteEvaluation:
                        response = await SendEvaluatorScoreUploadEmail(emailDetails);
                        break;
                    case NotificationType.EvaluatorReminder:
                        response = await SendEvaluatorReminderEmail(emailDetails);
                        break;
                    case NotificationType.OPMAssignmentUpload:
                        response = await SendOPMAssignmentUploadEmail(emailDetails);
                        break;
                    case NotificationType.OPMPublishResult:
                        response = await SendOPMPublishResultEmail(emailDetails);
                        break;
                }
            }
            catch (Exception ex)
            {
                Logging.Logger.LogException(ex);
            }
            return response;
        }

        private async Task<Response> SendLearnerAssignmentUploadEmail(EmailDetails emailDetails)
        {
            var response = new Response();
            var emailEntity = new EmailEntity();
            if (emailDetails.RequestIds == null || !emailDetails.RequestIds.Any()) return response;
            var requestDetails = (await _requestRepository.GetRequestDetailsForEmailAsync(emailDetails.RequestIds))?.FirstOrDefault();
            emailEntity.Subject = string.Format("Yorbit: {0} Assignment submitted", requestDetails.CourseName);

            emailEntity.Body =ReadEmailTemplate("LearnerAssignmentUpload.html");
            emailEntity.Body = emailEntity.Body.Replace("{LearnerName}", requestDetails.LearnerName);
            emailEntity.Body = emailEntity.Body.Replace("{CourseName}", requestDetails.CourseName);
            emailEntity.ToRecipients = new List<string> { requestDetails.LearnerEmail };
            response = await _emailHelper.SendMail(emailEntity);

            emailEntity.Body = ReadEmailTemplate("EvaluatorAssignmentUpload.html");
            emailEntity.Body = emailEntity.Body.Replace("{EvaluatorName}", requestDetails.EvaluatorName);
            emailEntity.Body = emailEntity.Body.Replace("{YorbitRequestId}", requestDetails.YorbitRequestId);
            emailEntity.Body = emailEntity.Body.Replace(" {YorbitCourseId}", requestDetails.YorbitCourseId);
            emailEntity.Body = emailEntity.Body.Replace("{CourseName}", requestDetails.CourseName);
            emailEntity.Body = emailEntity.Body.Replace("{AssignmentFile}", string.IsNullOrEmpty(requestDetails.AssignmentFile) ? "-" : await _blobHelper.GetBlobUriAsync("assignments", requestDetails.AssignmentFile));
            emailEntity.ToRecipients = new List<string> { requestDetails.EvaluatorEmail };
            emailEntity.CCRecipients = new List<string> { _learningOPMEmail };
            response = await _emailHelper.SendMail(emailEntity);

            return response;
        }

        private async Task<Response> SendLearnerReminderEmail(EmailDetails emailDetails)
        {
            var response = new Response();
            var emailEntity = new EmailEntity();
            if (emailDetails.RequestIds == null || !emailDetails.RequestIds.Any()) return response;
            var requestDetails = (await _requestRepository.GetRequestDetailsForEmailAsync(emailDetails.RequestIds));
            foreach (var requestDetail in requestDetails)
            {
                emailEntity.Subject = "Yorbit: Assignment submission reminder";
                emailEntity.Body = ReadEmailTemplate("LearnerReminder.html");
                emailEntity.Body = emailEntity.Body.Replace("{LearnerName}", requestDetail.LearnerName);
                emailEntity.Body = emailEntity.Body.Replace("{CourseName}", requestDetail.CourseName);
                emailEntity.Body = emailEntity.Body.Replace("{AssignmentDueDate}", requestDetail.AssignmentDueDate.ToString("dd-MM-yyyy"));
                emailEntity.ToRecipients = new List<string> { requestDetail.LearnerEmail };
                await _emailHelper.SendMail(emailEntity);
            }
            return response;
        }

        private async Task<Response> SendEvaluatorErrorFileUploadEmail(EmailDetails emailDetails)
        {
            var response = new Response();
            var emailEntity = new EmailEntity();
            if (emailDetails.RequestIds == null || !emailDetails.RequestIds.Any()) return response;
            var requestDetails = (await _requestRepository.GetRequestDetailsForEmailAsync(emailDetails.RequestIds))?.FirstOrDefault();
            emailEntity.Subject = "Yorbit: Error found in assignment";
            emailEntity.Body =ReadEmailTemplate("EvaluatorErrorFileUpload.html");
            emailEntity.Body = emailEntity.Body.Replace("{LearnerName}", requestDetails.LearnerName);
            emailEntity.Body = emailEntity.Body.Replace("{CourseName}", requestDetails.CourseName);
            emailEntity.Body = emailEntity.Body.Replace("{Comments}", string.IsNullOrEmpty(requestDetails.Comments) ? "-" : requestDetails.Comments.Trim());
            emailEntity.Body = emailEntity.Body.Replace("{ErrorFile}", string.IsNullOrEmpty(requestDetails.ErrorFile) ? "-" : await _blobHelper.GetBlobUriAsync("assignments",requestDetails.ErrorFile));
            emailEntity.ToRecipients = new List<string> { requestDetails.LearnerEmail};
            emailEntity.CCRecipients = new List<string> { _learningOPMEmail };
            await _emailHelper.SendMail(emailEntity);
            return response;
        }

        private async Task<Response> SendEvaluatorScoreUploadEmail(EmailDetails emailDetails)
        {
            var response = new Response();
            var emailEntity = new EmailEntity();
            if (emailDetails.RequestIds == null || !emailDetails.RequestIds.Any()) return response;
            var requestDetails = (await _requestRepository.GetRequestDetailsForEmailAsync(emailDetails.RequestIds))?.FirstOrDefault();
            emailEntity.Subject = "Yorbit: Assignment score submitted";
            emailEntity.Body = ReadEmailTemplate("EvaluatorCompleteEvaluation.html");
            emailEntity.Body = emailEntity.Body.Replace("{YorbitRequestId}", requestDetails.YorbitRequestId);
            emailEntity.Body = emailEntity.Body.Replace("{YorbitCourseId}", requestDetails.YorbitCourseId);
            emailEntity.Body = emailEntity.Body.Replace("{CourseName}", requestDetails.CourseName);
            emailEntity.ToRecipients = new List<string> { _learningOPMEmail };
            await _emailHelper.SendMail(emailEntity);
            return response;
        }

        private async Task<Response> SendEvaluatorReminderEmail(EmailDetails emailDetails)
        {
            var response = new Response();
            var emailEntity = new EmailEntity();
            if (emailDetails.RequestIds == null || !emailDetails.RequestIds.Any()) return response;
            var requestDetails = (await _requestRepository.GetRequestDetailsForEmailAsync(emailDetails.RequestIds));
            foreach (var requestDetail in requestDetails)
            {
                emailEntity.Subject = "Yorbit: Evaluation reminder";
                emailEntity.Body = ReadEmailTemplate("EvaluatorReminder.html");
                emailEntity.Body = emailEntity.Body.Replace("{EvaluatorName}", requestDetail.EvaluatorName);
                emailEntity.Body = emailEntity.Body.Replace("{CourseName}", requestDetail.CourseName);
                emailEntity.Body = emailEntity.Body.Replace("{YorbitCourseId}", requestDetail.YorbitCourseId);
                emailEntity.ToRecipients = new List<string> { requestDetail.EvaluatorEmail };
                await _emailHelper.SendMail(emailEntity);
            }
            return response;
        }

        private async Task<Response> SendOPMAssignmentUploadEmail(EmailDetails emailDetails)
        {
            var response = new Response();
            var emailEntity = new EmailEntity();
            if (emailDetails.RequestIds == null || !emailDetails.RequestIds.Any()) return response;
            var requestDetails = (await _requestRepository.GetRequestDetailsForEmailAsync(emailDetails.RequestIds))?.FirstOrDefault();
            emailEntity.Subject = "Yorbit: Assignment submitted";
            emailEntity.Body = ReadEmailTemplate("OPMAssignmentUpload.html");
            emailEntity.Body = emailEntity.Body.Replace("{LearnerName}", requestDetails.LearnerName);
            emailEntity.Body = emailEntity.Body.Replace("{CourseName}", requestDetails.CourseName);
            emailEntity.ToRecipients = new List<string> { requestDetails.LearnerEmail};
            emailEntity.CCRecipients = new List<string> { _learningOPMEmail };
            await _emailHelper.SendMail(emailEntity);

            emailEntity.Body = ReadEmailTemplate("EvaluatorAssignmentUpload.html"); 
            emailEntity.Body = emailEntity.Body.Replace("{EvaluatorName}", requestDetails.EvaluatorName);
            emailEntity.Body = emailEntity.Body.Replace("{YorbitRequestId}", requestDetails.YorbitRequestId);
            emailEntity.Body = emailEntity.Body.Replace(" {YorbitCourseId}", requestDetails.YorbitCourseId);
            emailEntity.Body = emailEntity.Body.Replace("{CourseName}", requestDetails.CourseName);
            emailEntity.Body = emailEntity.Body.Replace("{AssignmentFile}", string.IsNullOrEmpty(requestDetails.AssignmentFile) ? "-" : await _blobHelper.GetBlobUriAsync("assignments", requestDetails.AssignmentFile));
            emailEntity.ToRecipients = new List<string> { requestDetails.EvaluatorEmail};
            emailEntity.CCRecipients = new List<string> { _learningOPMEmail };
            await _emailHelper.SendMail(emailEntity);
            return response;
        }

        private async Task<Response> SendOPMPublishResultEmail(EmailDetails emailDetails)
        {
            var response = new Response();
            var emailEntity = new EmailEntity();
            if (emailDetails.RequestIds == null || !emailDetails.RequestIds.Any()) return response;
            var requestDetails = await _requestRepository.GetRequestDetailsForEmailAsync(emailDetails.RequestIds);
            foreach (var request in requestDetails)
            {
                emailEntity.Subject = "Yorbit: Assignment result published";
                emailEntity.Body = ReadEmailTemplate("OPMPublishResult.html");
                emailEntity.Body = emailEntity.Body.Replace("{LearnerName}", request.LearnerName);
                emailEntity.Body = emailEntity.Body.Replace("{CourseName}", request.CourseName);
                emailEntity.Body = emailEntity.Body.Replace("{Result}",requestResultCodes.GetStatusName(request.ResultId));
                emailEntity.Body = emailEntity.Body.Replace("{Score}", request.Score == null ? "-": request.Score.ToString());
                emailEntity.Body = emailEntity.Body.Replace("{Comments}", string.IsNullOrEmpty(request.Comments) ? "-" : request.Comments.Trim());
                emailEntity.Body = emailEntity.Body.Replace("{ScoreCardFile}", 
                    string.IsNullOrEmpty(request.ScoreCardFile)
                    ? "-"
                    : "Click <a href="+ await _blobHelper.GetBlobUriAsync("assignments", request.ScoreCardFile) + ">&nbsp;<span><b>here</b></span></a>");
                emailEntity.ToRecipients = new List<string> { request.LearnerEmail };
                emailEntity.CCRecipients = new List<string> { _learningOPMEmail };
                await _emailHelper.SendMail(emailEntity);
            }
            return response;
        }
        public string ReadEmailTemplate(string filename)
        {
            string emailContent = "";
            string filePath = _hostingEnvironment.WebRootPath + "\\EmailTemplates\\" + filename;
            using (var streamReader = new StreamReader(filePath, Encoding.UTF8))
            {
                emailContent = streamReader.ReadToEnd();
            }
            return emailContent;
        }

        private string LearnerAssignmentUpload = @"
            Dear {LearnerName},
                Your assignment solution for {CourseName} course has been submitted successfully.
            Regards,
            Yorbit
        ";

        private string EvaluatorAssignmentUpload = @"
            Dear {EvaluatorName},
                The assignment solution for {YorbitRequestId} request for {YorbitCourseId} - {CourseName} course has been submitted and is ready for evaluation.
                Click here to download the file - {AssignmentFile}
            Regards,
            Yorbit
        ";

        private string EvaluatorErrorFileUpload = @"
            Dear {LearnerName},
                There is an error in the assignment solution that you submitted for {CourseName} course. Please rectify the same and resubmit your solution.
                Evaluation remarks - {Comments}
                For more information on the error, check here - {ErrorFile}
            Regards,
            Yorbit
        ";

        private string EvaluatorCompleteEvaluation = @"
            Dear team,
                The evaluation for {YorbitRequestId} request for {YorbitCourseId} - {CourseName} course has been completed.
            Regards,
            Yorbit
        ";

        private string LearnerReminder = @"
            Dear {LearnerName},
                This is a reminder mail regarding your pending assignment for {CourseName} course. The due date is {AssignmentDueDate}.
            Regards,
            Yorbit
        ";

        private string EvaluatorReminder = @"
            Dear {EvaluatorName},
                This is a reminder mail regarding your pending evaluation for {YorbitCourseId} - {CourseName} course.
            Regards,
            Yorbit
        ";

        private string OPMAssignmentUpload = @"
            Dear {LearnerName},
                Your assignment solution for {CourseName} course has been submitted successfully by the Learning OPM team.
            Regards,
            Yorbit
        ";

        private string OPMPublishResult = @"
            Dear {LearnerName},
                The evaluation for your {CourseName} course has been completed.

                    Result : {Result}
                    Score: {Score}
                    Evaluation remarks: {Comments}
                    Evaluation sheet/Score card : {ScoreCardFile}

            Regards,
            Yorbit
        ";
    }
}

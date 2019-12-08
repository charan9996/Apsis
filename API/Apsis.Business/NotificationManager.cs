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
using System.Diagnostics;

namespace Apsis.Business
{
    /// <summary>
    /// 
    /// </summary>
    public class NotificationManager : INotificationManager
    {
        readonly IRequestRepository _requestRepository;
        readonly IEmailManager _emailManager;

        public NotificationManager(
            IRequestRepository requestRepository,
            IEmailManager emailManager)
        {
            _requestRepository = requestRepository;
            _emailManager = emailManager;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<Response> GetReminderLearnerRequestId()
        {
            Response response = new Response();
            List<Guid> requestIds = await _requestRepository.GetPendingSubmissionRequestsId();
            
            if (requestIds.Count != 0)
            {
                _emailManager.SendMail(NotificationType.LearnerReminder, new EmailDetails { RequestIds = requestIds });
            }
            response.IsSuccess = true;
            response.Message = "Mail sent to " + requestIds.Count;
            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<Response> GetReminderEvaluatorIds()
        {
            Response response = new Response(); 
            List<Guid> requestIds = await _requestRepository.GetPendingEvaluationRequestId();
            if (requestIds.Count != 0)
            {
                _emailManager.SendMail(NotificationType.EvaluatorReminder,new EmailDetails { RequestIds = requestIds });
            }
            response.IsSuccess = true;
            response.Message = "Mail sent to " + requestIds.Count;
            return response;
        }
    }
}

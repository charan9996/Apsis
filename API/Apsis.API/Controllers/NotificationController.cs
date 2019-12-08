using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc; 
using Apsis.Abstractions;
using Apsis.Models.Authorization;
using Apsis.Abstractions.Business;
using Microsoft.AspNetCore.Authorization;

namespace Apsis.API.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Produces("application/json")]
    [Route("api/Notification")]
    [AllowAnonymous]
    public class NotificationController : Controller
    { 
        readonly INotificationManager _notificationManager;
        /// <summary>
        /// 
        /// </summary> 
        public NotificationController(INotificationManager notificationManager)
        {
            _notificationManager = notificationManager;
        }

        /// <summary>
        /// To send reminder to learner to submit project/assignment
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Learner-Reminder")]
        public async Task SendReminderLearner()
        {
            await _notificationManager.GetReminderLearnerRequestId();
        }

        /// <summary>
        ///  To send reminder to evaluator to evaluate project/assignment
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Evaluator-Reminder")]
        public async Task SendReminderEvaluator()
        {
            await _notificationManager.GetReminderEvaluatorIds();
        }
    }
}

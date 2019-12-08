using Apsis.Models.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Apsis.Abstractions.Business
{
    public interface INotificationManager
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<Response> GetReminderLearnerRequestId();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<Response> GetReminderEvaluatorIds();
    }
}

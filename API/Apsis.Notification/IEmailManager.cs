using Apsis.Models.Common;
using Apsis.Models.Constants;
using Apsis.Models.Response;
using System.Threading.Tasks;

namespace Apsis.Notification
{
    public interface IEmailManager
    {
        Task<Response> SendMail(NotificationType notificationType, EmailDetails emailDetails);
    }
}

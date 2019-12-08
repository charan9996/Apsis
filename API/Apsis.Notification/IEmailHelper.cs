using Apsis.Models.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Apsis.Notification
{
    public interface IEmailHelper
    {
        Task<Response> SendMail(EmailEntity emailEntity);
    }
}

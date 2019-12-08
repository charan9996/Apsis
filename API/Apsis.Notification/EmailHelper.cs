using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;
using Apsis.Models.Response;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace Apsis.Notification
{
    public class EmailHelper : IEmailHelper
    {
        readonly string smtpHost;
        readonly int smtpPort;
        readonly string smtpUserName;
        readonly string smtpPassword;

        public EmailHelper(IConfiguration configuration)
        {
            smtpHost = configuration["SmtpSettings:Host"];
            smtpPort = configuration["SmtpSettings:Port"] != null ? int.Parse(configuration["SmtpSettings:Port"]) : 0;
            smtpUserName =configuration["SmtpSettings:UserName"];
            smtpPassword =configuration["SmtpSettings:Password"];
        }

        /// <summary>
        /// Sends mail using SMTP
        /// </summary>
        /// <param name="emailEntity"></param>
        /// <returns></returns>
        public async Task<Response> SendMail(EmailEntity emailEntity)
        {
            var response = new Response();
            try
            {
                if (emailEntity.ToRecipients == null || !emailEntity.ToRecipients.Any())
                {
                    response.Message = "No recipents found.";
                    return response;
                }

                MailMessage message = new MailMessage();
                foreach (var email in emailEntity.ToRecipients)
                {
                    if (!string.IsNullOrEmpty(email)) message.To.Add(email);
                }

                message.From = new MailAddress(smtpUserName, "Yorbit Notification");
                message.Subject = emailEntity.Subject;
                message.Body = emailEntity.Body;
                if (emailEntity.CCRecipients != null)
                {
                    foreach (var email in emailEntity.CCRecipients)
                    {
                        if (!string.IsNullOrEmpty(email)) message.CC.Add(email);
                    }
                }
                message.IsBodyHtml = true;

                using (var client = new SmtpClient())
                {
                    client.UseDefaultCredentials = false;
                    client.Credentials = new System.Net.NetworkCredential(smtpUserName, smtpPassword);
                    client.Port = smtpPort;
                    client.Host = smtpHost;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.EnableSsl = true;

                    await client.SendMailAsync(message);
                }
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                Logging.Logger.LogException(ex);
            }
            return response;
        }
    }
}

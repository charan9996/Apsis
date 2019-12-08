using System;
using System.Collections.Generic;
using System.Text;

namespace Apsis.Notification
{
    /// <summary>
    /// Captures all the information needed to send an email
    /// </summary>
    public class EmailEntity
    {
        /// <summary>
        /// Email subject
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// Email body
        /// </summary>
        public string Body { get; set; }
        /// <summary>
        /// Email addresses of people to whom the mail needs to be send
        /// </summary>
        public List<string> ToRecipients { get; set; }
        /// <summary>
        /// Email addresses of people who have to be in the CC
        /// </summary>
        public List<string> CCRecipients { get; set; }
    }
}

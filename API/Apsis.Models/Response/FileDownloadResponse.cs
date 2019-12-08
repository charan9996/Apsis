using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Apsis.Models.Response
{
    /// <summary>
    /// Holding File Download Responses
    /// </summary>
    public class FileDownloadResponse : Response
    {
        /// <summary>
        /// File Url
        /// </summary>
        public string FileUrl { get; set; }
    }
}

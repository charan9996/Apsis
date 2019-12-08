using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apsis.Models.ViewModel
{
    public class ErrorModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IFormFile File { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Mid { get; set; }
    }
}

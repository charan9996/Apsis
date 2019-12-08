using Apsis.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apsis.Models.Response
{
    /// <summary>
    /// A response class for Request List.
    /// </summary>
    public class RequestListResponse : Response
    {
        /// <summary>
        /// List of requests based on RequestView
        /// </summary>
        public IEnumerable<RequestView> requestViews { get; set; }
    }
}

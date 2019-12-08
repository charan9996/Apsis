using Apsis.Models.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apsis.Models.Common
{ 
    /// <summary>
    /// Model to pass search parameters based on which the "Request Data" will be retrieved from API/DB
    /// </summary>
    public class RequestSearchFilter: SearchFilter
    {
        /// <summary>
        /// From filter menu decide which group of ResultIds to show including:
        /// Cleared, Not cleared, Yet to evaluate, Error, Yet to evaluate
        /// </summary>
        public E_RESULT_FILTER Filter { get; set; }
        /// <summary>
        /// Sorting the list based on either of:
        /// Evaluation Date, Submission Date, Academy, Learner Name
        /// </summary>
        public E_SORT_ORDER Sort { get; set; }
    }
}

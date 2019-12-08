using System;
using System.Collections.Generic;
using System.Text;

namespace Apsis.Models.ViewModel
{
    /// <summary>
    /// Attempts Log Model
    /// </summary>
    public class AttemptsLogModel
    {
        /// <summary>
        /// Submission Date
        /// </summary>
        public DateTime SubmissionDate { get; set; }

        /// <summary>
        /// Evaluation Date
        /// </summary>
        public DateTime EvaluationDate { get; set; }

        /// <summary>
        /// Result Name
        /// </summary>
        public string ResultName { get; set; }
    }
}

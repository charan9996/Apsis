using System;
using System.Collections.Generic;
using System.Text;

namespace Apsis.Models.ViewModel
{
    /// <summary>
    /// Evaluator and Course Table Entity
    /// </summary>
    public class EvaluatorCourseModel
    {
        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Evaluator Id
        /// </summary>
        public Guid EvaluatorId { get; set; }

        /// <summary>
        /// Course Id
        /// </summary>
        public Guid CourseId { get; set; }
    }
}

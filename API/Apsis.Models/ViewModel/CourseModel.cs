using Apsis.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apsis.Models.ViewModel
{
    /// <summary>
    /// 
    /// </summary>
    public class CourseModel : Response.Response
    {
        /// <summary>
        /// Yorbit Course Id
        /// </summary>
        public string YorbitCourseId { get; set; }

        /// <summary>
        /// Course Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Course Name
        /// </summary>
        public string CourseName { get; set; }

        /// <summary>
        /// Academy
        /// </summary>
        public string Academy { get; set; }

        /// <summary>
        /// Batch Type
        /// </summary>
        public string BatchType { get; set; }

        /// <summary>
        /// Course Type
        /// </summary>
        public string CourseType { get; set; }

        /// <summary>
        /// Total number of count
        /// </summary>
        public int RowsCount { get; set; }
        public int Count { get; set; }

        /// <summary>
        /// Evaluators (List)
        /// </summary>
        public List<User> Evaluators { get; set; }

        /// <summary>
        /// Course Problem Statement Url
        /// </summary>
        public string CourseProblemStatementUrl { get; set; }
        public Guid AssignmentFileId { get; set; }
    }
}

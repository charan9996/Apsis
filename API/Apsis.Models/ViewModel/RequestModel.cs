using System;
using System.Collections.Generic;
using System.Text;

namespace Apsis.Models.ViewModel
{
    public class RequestModel : BaseEntity
    {
        public Guid Id { get; set; }
        public string Mid { get; set; }
        public Guid LearnerId { get; set; }
        public Guid CourseId { get; set; }
        public Guid EvaluatorId { get; set; }
        public Guid YorbitStatusId { get; set; }
        /// <summary>
        /// Request Id that is enrolled in yorbit
        /// </summary>
        public string YorbitRequestId { set; get; }
        /// <summary>
        /// The Course Id which is enrolled in yorbit
        /// </summary>
        public string YorbitCourseId { set; get; }
        /// <summary>
        /// Name of Course Which the learner is undergoing
        /// </summary>
        public string CourseName { set; get; }
        /// <summary>
        /// Due Date of assignment which should be submitted by Learner
        /// </summary>
        public DateTime AssignmentDueDate { set; get; }
        /// <summary>
        /// File id which is required to Download the assignment
        /// </summary>
        public Guid AssignmentFileId { get; set; }
        public string FilePath { get; set; }
        public DateTime ApprovedDate { get; set; }
        public bool isPublished { get; set; }
        public Guid ResultId { get; set; }
        public int Attempts { get; set; }
    }
}

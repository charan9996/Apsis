using System;
using System.Collections.Generic;
using System.Text;

namespace Apsis.Models.ViewModel
{
    public class RequestEmailModel
    {
        public Guid RequestId { get; set; }
        public string YorbitRequestId { get; set; }
        public DateTime AssignmentDueDate { get; set; }
        public string YorbitCourseId { get; set; }
        public string CourseName { get; set; }
        public string Academy { get; set; }
        public string BatchType { get; set; }
        public string CourseType { get; set; }
        public string LearnerName { get; set; }
        public string LearnerMID { get; set; }
        public string LearnerEmail { get; set; }
        public string EvaluatorName { get; set; }
        public string EvaluatorEmail { get; set; }
        public Guid ResultId { get; set; }
        public float? Score { get; set; }
        public string Comments { get; set; }
        public string AssignmentFile { get; set; }
        public string ScoreCardFile { get; set; }
        public string ErrorFile { get; set; }
    }
}

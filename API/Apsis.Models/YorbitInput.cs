using System;
using System.Collections.Generic;
using System.Text;

namespace Apsis.Models
{
    public class YorbitInput
    {
        public string RequestId { get; set; }
        public string Mid { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Location { get; set; }
        public string CourseId { get; set; }
        public string CourseName { get; set; }
        public string Academy { get; set; }
        public string CourseType { get; set; }
        public string BatchType { get; set; }
        public string CourseStatus { get; set; }
        public string SubmissionStatus { get; set; }
        public string Remarks { get; set; }
        public DateTime SubmissionDate { get; set; }
        public DateTime ApprovedDate { get; set; }
        public string EvaluatorMID { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Apsis.Models.ViewModel
{
    public class RequestView
    {
        public int TotalRows { get; set; }
        public Guid RequestId { get; set; }
        public DateTime? SubmissionDate { get; set; }
        public Guid ResultId { get; set; }
        public DateTime AssignmentDueDate { get; set; }
        public string YorbitRequestId { get; set; }
        public string Name { get; set; }
        public string MID { get; set; }
        public string YorbitCourseId { get; set; }
        public string Academy { get; set; }
        public string CourseName { get; set; }
        public bool isPublished { get; set; }

    }
}

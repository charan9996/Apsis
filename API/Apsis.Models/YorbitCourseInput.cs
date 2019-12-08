using System;
using System.Collections.Generic;
using System.Text;

namespace Apsis.Models
{
   public class YorbitCourseInput
    {
        public string CourseId { get; set; }
        public string CourseName { get; set; }
        public string Academy { get; set; }
        public string CourseType { get; set; }
        public string BatchType { get; set; }
        public string EvaluatorMID { get; set; }
        public string EvaluatorName { get; set; }
        public string EvaluatorEmail { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}

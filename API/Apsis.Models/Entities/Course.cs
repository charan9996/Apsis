using System;
using System.Collections.Generic;
using System.Text;

namespace Apsis.Models.Entities
{
    public class Course :BaseEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string YorbitCourseId { get; set; }
        public string BatchType { get; set; }
        public string CourseType { get; set; }
        public string Academy { get; set; }
        public Guid AssignmentFileId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}

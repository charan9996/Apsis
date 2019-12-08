using System;
using System.Collections.Generic;
using System.Text;

namespace Apsis.Models.Entities
{
    public class Request :BaseEntity
    {
        public Guid LearnerId { get; set; }
        public string YorbitRequestId { get; set; }
        public Guid CourseId { get; set; }
        public Guid  EvaluatorId { get; set; }
        public Guid YorbitStatusId { get; set; }
        public DateTime AssignmentDueDate { get; set; }
        public DateTime ApprovedDate { get; set; }
        public bool IsPublished { get; set; }
    }
}

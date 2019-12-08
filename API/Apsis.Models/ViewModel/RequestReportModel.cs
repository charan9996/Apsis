using System;
using System.Collections.Generic;
using System.Text;

namespace Apsis.Models.ViewModel
{
    public class RequestReportModel
    {
        public string RequestID { get; set; }
        public string Mid { get; set; }
        public string Name { get; set; }
        public string EmailID { get; set; }
        public string CourseName { get; set; }
        public string CourseID { get; set; }
        public string Academy { get; set; }
        public DateTime ApprovedDate { get; set; }
        public DateTime AssignmentDueDate { get; set; }
        public string EvaluatorMailId { get; set; }
        public string EvaluatorType { get; set; }
        public string Vendor { get; set; }
        public DateTime SubmissionDate { get; set; }
        public int LearnerTAT { get; set; }
        public DateTime EvaluatorAssignmentDownloadDate { get; set; }
        public DateTime EvaluationResultDate { get; set; }
        public int EvaluatorTAT { get; set; }
        public double Score { get; set; }
        public string RequestStatus { get; set; }
        public string EvaluatorComments { get; set; }


    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Apsis.Models.ViewModel
{
    public class RequestReportExcelModel
    {
        public string RequestID { get; set; }
        public string Mid { get; set; }
        public string Name { get; set; }
        public string EmailID { get; set; }
        public string CourseName { get; set; }
        public string CourseID { get; set; }
        public string Academy { get; set; }
        public string ApprovedDate { get; set; }
        public string AssignmentDueDate { get; set; }
        public string EvaluatorMailId { get; set; }
        public string EvaluatorType { get; set; }
        public string Vendor { get; set; }
        public string SubmissionDate { get; set; }
        public int LearnerTAT { get; set; }
        public string FirstEvaluatorAssignmentDownloadDate { get; set; }
        public string FirstEvaluationResultDate { get; set; }
        public int FirstEvaluatorTAT { get; set; }
        public double FirstScore { get; set; }
        public string FirstRequestStatus { get; set; }
        public string FirstEvaluatorComments { get; set; }
        public string FirstResubmissionDate { get; set; }
        public string SecondEvaluatorAssignmentDownloadDate { get; set; }
        public string SecondEvaluationResultDate { get; set; }
        public int SecondEvaluatorTAT { get; set; }
        public double SecondScore { get; set; }
        public string SecondRequestStatus { get; set; }
        public string SecondEvaluatorComments { get; set; }
        public string SecondResubmissionDate { get; set; }
        public string ThirdEvaluatorAssignmentDownloadDate { get; set; }
        public string ThirdEvaluationResultDate { get; set; }
        public int ThirdEvaluatorTAT { get; set; }
        public double ThirdScore { get; set; }
        public string ThirdRequestStatus { get; set; }
        public string ThirdEvaluatorComments { get; set; }
    }
}

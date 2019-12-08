using Apsis.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apsis.Models.ViewModel
{
    /// <summary>
    /// Request Details View Model
    /// </summary>
    public class RequestDetailsModel : Response.Response
    {
        /// <summary>
        /// Learner MID
        /// </summary>
        public string LearnerMid { get; set; }

        /// <summary>
        /// Learner Name
        /// </summary>
        public string LearnerName { get; set; }

        /// <summary>
        /// Yorbit Request Id
        /// </summary>
        public string YorbitRequestId { get; set; }

        /// <summary>
        /// Location
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Yorbit Course Id
        /// </summary>
        public string YorbitCourseId { get; set; }

        /// <summary>
        /// Course Name
        /// </summary>
        public string CourseName { get; set; }

        /// <summary>
        /// Academy
        /// </summary>
        public string Academy { get; set; }

        /// <summary>
        /// Assignment Attempt Id (GUID)
        /// </summary>
        public Guid AssignmentAttemptId { get; set; }

        /// <summary>
        /// Score
        /// </summary>
        public float Score { get; set; }

        /// <summary>
        /// Comments
        /// </summary>
        public string Comments { get; set; }

        /// <summary>
        /// Result
        /// </summary>
        public Guid ResultId { get; set; }

        /// <summary>
        /// Result Name
        /// </summary>
        public string ResultName { get; set; }

        /// <summary>
        /// Assignment Solution File Name
        /// </summary>
        public string AssignmentSolutionFileName { get; set; }

        /// <summary>
        /// Score Card File Name
        /// </summary>
        public string ScoreCardFileName { get; set; }

        /// <summary>
        /// Assignment Solution URL
        /// </summary>
        public string AssignmentSolutionUrl { get; set; }

        /// <summary>
        /// Score Card URL
        /// </summary>
        public string ScoreCardUrl { get; set; }

        /// <summary>
        /// Error file name
        /// </summary>
        public string ErrorFileName { get; set; }

        /// <summary>
        /// Error File Url
        /// </summary>
        public string ErrorFileUrl { get; set; }

        /// <summary>
        /// Attempts
        /// </summary>
        public List<AttemptsLogModel> AttemptsLog { get; set; }

        /// <summary>
        /// Evaluator Id
        /// </summary>
        public Guid EvaluatorId { get; set; }


    }
}

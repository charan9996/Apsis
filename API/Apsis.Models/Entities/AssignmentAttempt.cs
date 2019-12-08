using System;
using System.Collections.Generic;
using System.Text;

namespace Apsis.Models.Entities
{
    /// <summary>
    /// Assignment Attempts table entity
    /// </summary>
    public class AssignmentAttempt : BaseEntity
    {
        /// <summary>
        /// RequestID
        /// </summary>
        public Guid RequestId { get; set; }

        /// <summary>
        /// YorbitRequestId
        /// </summary>
        public string YorbitRequestId { get; set; }

        /// <summary>
        /// UploadedFileId
        /// </summary>
        public Guid UploadedFileId { get; set; }

        /// <summary>
        /// ResultId
        /// </summary>
        public Guid ResultId { get; set; }

        /// <summary>
        /// Comments
        /// </summary>
        public string Comments { get; set; }

        /// <summary>
        /// Score
        /// </summary>
        public double Score { get; set; }

        /// <summary>
        /// Score
        /// </summary>
        public DateTime SubmissionDate { get; set; }

        /// <summary>
        /// EvaluatorDownloadedDate
        /// </summary>
        public DateTime EvaluatorDownloadedDate { get; set; }

        /// <summary>
        /// ScoreCardId
        /// </summary>
        public Guid ScoreCardId { get; set; }

        /// <summary>
        /// ErrorFileID
        /// </summary>
        public Guid ErrorFieldId { get; set; }

        /// <summary>
        /// EvaluationDate
        /// </summary>
        public DateTime EvaluationDate { get; set; }
    }
}

using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apsis.Models.Entities
{
    /// <summary>
    /// Evaluation Result Details 
    /// </summary>
    public class Evaluation
    {
        /// <summary>
        /// Comments
        /// </summary>
        public string Comments { get; set; }

        /// <summary>
        /// Score
        /// </summary>
        public float Score { get; set; }

        /// <summary>
        /// RequestId
        /// </summary>
        public Guid ResultId { get; set; }

        /// <summary>
        /// ScoreCardFile
        /// </summary>
        public IFormFile ScoreCardFile { get; set; }

        /// <summary>
        /// Evaluation Date
        /// </summary>
        public DateTime EvaluationDate { get; set; }

        /// <summary>
        /// MID
        /// </summary>
        public string Mid { get; set; }
    }
}

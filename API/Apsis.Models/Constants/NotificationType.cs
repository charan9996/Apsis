namespace Apsis.Models.Constants
{
    /// <summary>
    /// Scenarios where notifications will be triggered
    /// </summary>
    public enum NotificationType
    {
        /// <summary>
        /// Learner uploads completed assignment
        /// </summary>
        LearnerAssignmentUpload = 1,
        /// <summary>
        /// Fortnightly reminder to learner to complete assignment
        /// </summary>
        LearnerReminder,
        /// <summary>
        /// Evaluator uploads error file
        /// </summary>
        EvaluatorErrorFileUpload,
        /// <summary>
        /// Evaluator completes evaluation by providing score, comment and scorecard
        /// </summary>
        EvaluatorCompleteEvaluation,
        /// <summary>
        /// Reminder to evaluator every two days post SLA
        /// </summary>
        EvaluatorReminder,
        /// <summary>
        /// OPM uploads completed assignment on behalf of the learner
        /// </summary>
        OPMAssignmentUpload,
        /// <summary>
        /// OPM publishes the result
        /// </summary>
        OPMPublishResult
    }
}

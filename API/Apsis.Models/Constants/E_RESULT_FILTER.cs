 namespace Apsis.Models.Constants
{
    /// <summary>
    /// Request
    /// </summary>
    public enum E_RESULT_FILTER
    {
        /// <summary>
        /// Show all kind of assignment attempts/requests.
        /// </summary>
        ALL,
        /// <summary>
        /// Show assignment attempts/requests with result as "Error"
        /// </summary>
        ERROR,
        /// <summary>
        /// Show assignment attempts/requests with result as "Yet to evaluate"
        /// </summary>
        Result_Awaited,
        /// <summary>
        /// Show assignment attempts/requests with result as "Cleared"
        /// </summary>
        Project_Cleared,
        /// <summary>
        /// Show assignment attempts/requests with result as "Not Cleared"
        /// </summary>
        Project_Not_Cleared,
        /// <summary>
        /// Show assignment attempts who are never submitted i.e. "Submission Date is null"
        /// </summary>
        YET_TO_SUBMIT,
        /// <summary>
        /// show requests whose Results have been not published
        /// </summary>
        YET_TO_PUBLISHED

    } 
}

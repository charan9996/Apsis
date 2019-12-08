namespace Apsis.Models.Common
{
    /// <summary>
    /// Model to pass search parameters based on which the data will be retrieved from API/DB
    /// </summary>
    public abstract class SearchFilter
    {
        /// <summary>
        /// Keyword supporting: 
        /// Request Id, Learner name, MID, Academy, Name, Yorbit Course Id etc..
        /// </summary>
        public string Keyword { get; set; }        
        /// <summary>
        /// Paginated view: Current page e.g: 2/10 means second page
        /// </summary>
        public int CurrentPage { get; set; }
        /// <summary>
        /// Total number of items in row to be displayed.
        /// </summary>
        public int PageSize { get; set; } 
    } 
}
